using System;
using System.Threading;
using System.Threading.Tasks;
using CepAgent.Application.DTOs;
using CepAgent.Domain.Services;
using CepAgent.Domain.ValueObjects;

namespace CepAgent.Application.UseCases
{
    /// <summary>
    /// Caso de uso responsável por orquestrar a consulta de CEP.
    /// - Recebe uma requisição com o CEP (pode ter máscara).
    /// - Constrói o Value Object <see cref="Cep"/> para validação/normalização.
    /// - Chama <see cref="ICepLookupService"/> para buscar o endereço.
    /// - Retorna um <see cref="ConsultaCepResponse"/> apropriado.
    /// </summary>
    public sealed class ConsultarCepUseCase
    {
        private readonly ICepLookupService _cepLookupService;

        /// <summary>
        /// Cria uma instância do caso de uso.
        /// </summary>
        /// <param name="cepLookupService">Serviço de lookup de CEP implementado pela camada de infraestrutura.</param>
        public ConsultarCepUseCase(ICepLookupService cepLookupService)
        {
            _cepLookupService = cepLookupService ?? throw new ArgumentNullException(nameof(cepLookupService));
        }

        /// <summary>
        /// Executa o caso de uso de consulta de CEP.
        /// </summary>
        /// <param name="request">Requisição contendo o CEP (com ou sem máscara).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>DTO com o resultado da consulta.</returns>
        /// <exception cref="ArgumentNullException">Quando <paramref name="request"/> for nulo.</exception>
        public async Task<ConsultaCepResponse> ExecuteAsync(ConsultaCepRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            // Tentar criar o Value Object Cep, que fará a normalização e validação.
            Cep cep;
            try
            {
                cep = new Cep(request.Cep);
            }
            catch (ArgumentNullException ex)
            {
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "CEP não informado.",
                    Cep = string.Empty,
                    CepFormatado = string.Empty
                };
            }
            catch (ArgumentException ex)
            {
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "CEP inválido. O CEP deve conter exatamente 8 dígitos.",
                    Cep = string.Empty,
                    CepFormatado = string.Empty
                };
            }

            // Chamar o serviço de lookup do domínio.
            try
            {
                var endereco = await _cepLookupService.BuscarPorCepAsync(cep, cancellationToken).ConfigureAwait(false);
                return ConsultaCepResponse.FromDomain(endereco);
            }
            catch (OperationCanceledException)
            {
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "Operação cancelada.",
                    Cep = cep.Value,
                    CepFormatado = cep.ToMaskedString()
                };
            }
            catch (Exception ex)
            {
                // Em caso de erro inesperado (rede, timeout, etc.) retornamos mensagem amigável.
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "Erro ao consultar o CEP. Tente novamente mais tarde.",
                    Cep = cep.Value,
                    CepFormatado = cep.ToMaskedString()
                };
            }
        }
    }
}
