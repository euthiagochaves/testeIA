using System;
using CepAgent.Domain.Entities;

namespace CepAgent.Application.DTOs
{
    /// <summary>
    /// DTO de resposta para a consulta de CEP.
    /// Contém os principais campos retornados ao chamador (IA / MCP server / API).
    /// </summary>
    public sealed record ConsultaCepResponse
    {
        /// <summary>
        /// CEP no formato normalizado (apenas dígitos, 8 caracteres).
        /// </summary>
        public string Cep { get; init; }

        /// <summary>
        /// CEP formatado com máscara (5+3) ex.: "01001-000".
        /// </summary>
        public string CepFormatado { get; init; }

        /// <summary>
        /// Logradouro (rua, avenida, etc.).
        /// </summary>
        public string? Logradouro { get; init; }

        /// <summary>
        /// Complemento do endereço.
        /// </summary>
        public string? Complemento { get; init; }

        /// <summary>
        /// Bairro.
        /// </summary>
        public string? Bairro { get; init; }

        /// <summary>
        /// Localidade / cidade.
        /// </summary>
        public string? Localidade { get; init; }

        /// <summary>
        /// Unidade federativa (UF), ex.: SP, RJ.
        /// </summary>
        public string? Uf { get; init; }

        /// <summary>
        /// Indica se o CEP foi encontrado pela fonte externa.
        /// </summary>
        public bool Encontrado { get; init; }

        /// <summary>
        /// Mensagem descritiva em caso de erro ou CEP não encontrado.
        /// </summary>
        public string? Mensagem { get; init; }

        /// <summary>
        /// Cria um DTO de resposta a partir do domínio <see cref="Endereco"/>.
        /// </summary>
        /// <param name="endereco">Endereço do domínio (pode ser null se não encontrado).</param>
        /// <returns>Instância de <see cref="ConsultaCepResponse"/>.</returns>
        public static ConsultaCepResponse FromDomain(Endereco? endereco)
        {
            if (endereco is null)
            {
                return new ConsultaCepResponse
                {
                    Encontrado = false,
                    Mensagem = "CEP não encontrado",
                    Cep = string.Empty,
                    CepFormatado = string.Empty
                };
            }

            return new ConsultaCepResponse
            {
                Encontrado = true,
                Mensagem = null,
                Cep = endereco.Cep.Value,
                CepFormatado = endereco.Cep.ToMaskedString(),
                Logradouro = endereco.Logradouro,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Localidade = endereco.Localidade,
                Uf = endereco.Uf
            };
        }
    }
}
