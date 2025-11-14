using System;
using System.ComponentModel.DataAnnotations;
using CepAgent.Domain.ValueObjects;

namespace CepAgent.Domain.Entities
{
    /// <summary>
    /// Representa um endereço retornado pela consulta de CEP.
    /// Contém campos essenciais usados pela aplicação e pelo MCP server.
    /// </summary>
    public sealed class Endereco
    {
        /// <summary>
        /// Cep do endereço (Value Object do domínio).
        /// </summary>
        public Cep Cep { get; init; }

        /// <summary>
        /// Logradouro (rua, avenida, praça, etc.).
        /// </summary>
        public string? Logradouro { get; init; }

        /// <summary>
        /// Complemento do endereço (se houver).
        /// </summary>
        public string? Complemento { get; init; }

        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        public string? Bairro { get; init; }

        /// <summary>
        /// Localidade / cidade.
        /// </summary>
        public string? Localidade { get; init; }

        /// <summary>
        /// Unidade federativa (UF), ex: SP, RJ.
        /// </summary>
        public string? Uf { get; init; }

        /// <summary>
        /// Código IBGE da localidade (se disponível pela API externa).
        /// </summary>
        public string? Ibge { get; init; }

        /// <summary>
        /// Código GIA (se disponível pela API externa).
        /// </summary>
        public string? Gia { get; init; }

        /// <summary>
        /// Cria uma instância de Endereco.
        /// </summary>
        /// <param name="cep">Cep do endereço. Não pode ser nulo.</param>
        /// <param name="logradouro">Logradouro (opcional).</param>
        /// <param name="complemento">Complemento (opcional).</param>
        /// <param name="bairro">Bairro (opcional).</param>
        /// <param name="localidade">Localidade / cidade (opcional).</param>
        /// <param name="uf">Unidade federativa (opcional).</param>
        /// <param name="ibge">Código IBGE (opcional).</param>
        /// <param name="gia">Código GIA (opcional).</param>
        /// <exception cref="ArgumentNullException">Quando <paramref name="cep"/> for nulo.</exception>
        public Endereco(Cep cep,
                        string? logradouro = null,
                        string? complemento = null,
                        string? bairro = null,
                        string? localidade = null,
                        string? uf = null,
                        string? ibge = null,
                        string? gia = null)
        {
            Cep = cep;
            Logradouro = string.IsNullOrWhiteSpace(logradouro) ? null : logradouro;
            Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento;
            Bairro = string.IsNullOrWhiteSpace(bairro) ? null : bairro;
            Localidade = string.IsNullOrWhiteSpace(localidade) ? null : localidade;
            Uf = string.IsNullOrWhiteSpace(uf) ? null : uf;
            Ibge = string.IsNullOrWhiteSpace(ibge) ? null : ibge;
            Gia = string.IsNullOrWhiteSpace(gia) ? null : gia;
        }

        /// <summary>
        /// Retorna uma representação amigável do endereço.
        /// </summary>
        /// <returns>String contendo os principais campos do endereço.</returns>
        public override string ToString()
        {
            return $"{Logradouro ?? ""}{(string.IsNullOrEmpty(Logradouro) || string.IsNullOrEmpty(Bairro) ? "" : ", ")}{Bairro ?? ""} - {Localidade ?? ""}/{Uf ?? ""} (CEP: {Cep.ToMaskedString()})";
        }
    }
}
