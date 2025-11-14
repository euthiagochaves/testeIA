using System;
using System.Threading;
using System.Threading.Tasks;
using CepAgent.Application.DTOs;
using CepAgent.Application.UseCases;
using Microsoft.Extensions.Logging;
using CepAgent.McpServer.Mcp;

namespace CepAgent.McpServer.Tools
{
    /// <summary>
    /// Tools relacionados a consulta de CEP expostas via MCP.
    /// </summary>
    public sealed class CepTools
    {
        private readonly ConsultarCepUseCase _consultarCepUseCase;
        private readonly ILogger<CepTools> _logger;

        public CepTools(ConsultarCepUseCase consultarCepUseCase, ILogger<CepTools> logger)
        {
            _consultarCepUseCase = consultarCepUseCase ?? throw new ArgumentNullException(nameof(consultarCepUseCase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta um CEP e retorna um DTO amigavel.
        /// </summary>
        /// <param name="cep">CEP no formato livre (com ou sem mascara).</param>
        /// <param name="ct">Token de cancelamento.</param>
        /// <returns>Resultado da consulta.</returns>
        [McpTool("BuscarCepAsync")]
        public async Task<ConsultaCepResponse> BuscarCepAsync(string cep, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                _logger.LogWarning("CEP nao informado na chamada a BuscarCepAsync.");
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "CEP nao informado.",
                    Cep = string.Empty,
                    CepFormatado = string.Empty
                };
            }

            var request = new ConsultaCepRequest(cep);
            var response = await _consultarCepUseCase.ExecuteAsync(request, ct).ConfigureAwait(false);
            return response;
        }
    }
}
