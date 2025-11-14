using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CepAgent.Domain.Entities;
using CepAgent.Domain.Services;
using Microsoft.Extensions.Logging;

namespace CepAgent.Infrastructure.Http.Cep
{
    /// <summary>
    /// Implementa o contrato de domínio `ICepLookupService` usando a API ViaCEP.
    /// Utiliza `HttpClient` injetado via DI (recomendado através de `IHttpClientFactory` / `AddHttpClient`).
    /// </summary>
    public sealed class HttpCepLookupService : ICepLookupService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCepLookupService> _logger;

        /// <summary>
        /// Cria uma instância de <see cref="HttpCepLookupService"/>.
        /// </summary>
        /// <param name="httpClient">HttpClient configurado para chamar a API de CEP.</param>
        /// <param name="logger">Logger para registrar eventos e erros.</param>
        public HttpCepLookupService(HttpClient httpClient, ILogger<HttpCepLookupService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta o CEP usando a API externa e mapeia o resultado para o domínio `Endereco`.
        /// Retorna null quando o CEP não é encontrado ou em caso de erro ao processar a resposta.
        /// </summary>
        /// <param name="cep">CEP a ser consultado (Value Object do domínio).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Endereco quando encontrado; caso contrário, null.</returns>
        public async Task<Endereco?> BuscarPorCepAsync(CepAgent.Domain.ValueObjects.Cep cep, CancellationToken cancellationToken = default)
        {
            if (cep.Value is null)
                throw new ArgumentNullException(nameof(cep));

            try
            {
                // ViaCEP endpoint: GET https://viacep.com.br/ws/{cep}/json/
                var relative = $"ws/{cep.Value}/json/";
                var response = await _httpClient.GetAsync(relative, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao consultar ViaCEP. StatusCode={StatusCode} para CEP={Cep}", response.StatusCode, cep.Value);
                    return null;
                }

                var dto = await response.Content.ReadFromJsonAsync<ViaCepResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (dto is null)
                {
                    _logger.LogWarning("Resposta vazia ao consultar ViaCEP para CEP={Cep}", cep.Value);
                    return null;
                }

                if (dto.Erro == true)
                {
                    _logger.LogInformation("CEP não encontrado na ViaCEP: {Cep}", cep.Value);
                    return null;
                }

                // Mapear para Endereco do domínio
                var endereco = new Endereco(
                    new CepAgent.Domain.ValueObjects.Cep(dto.Cep ?? cep.Value),
                    logradouro: dto.Logradouro,
                    complemento: dto.Complemento,
                    bairro: dto.Bairro,
                    localidade: dto.Localidade,
                    uf: dto.Uf,
                    ibge: dto.Ibge,
                    gia: dto.Gia
                );

                return endereco;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Consulta de CEP cancelada pelo usuário. CEP={Cep}", cep.Value);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CEP {Cep}", cep.Value);
                return null;
            }
        }
    }
}