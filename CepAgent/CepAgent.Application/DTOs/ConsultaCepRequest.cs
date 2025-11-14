using System;

namespace CepAgent.Application.DTOs
{
    /// <summary>
    /// Representa a requisição para consultar um CEP.
    /// Contém o CEP em formato livre (com ou sem máscara). A validação de formato
    /// é realizada na camada de domínio (Value Object <see cref="CepAgent.Domain.ValueObjects.Cep"/>)
    /// quando o caso de uso tenta criar o VO.
    /// </summary>
    public sealed record ConsultaCepRequest
    {
        /// <summary>
        /// CEP informado pelo chamador. Pode conter máscara (ex.: "01001-000") ou apenas dígitos (ex.: "01001000").
        /// </summary>
        public string Cep { get; init; }

        /// <summary>
        /// Cria uma nova instância de <see cref="ConsultaCepRequest"/>.
        /// </summary>
        /// <param name="cep">CEP informado (pode ser null/empty, será tratado pelo caso de uso).</param>
        /// <exception cref="ArgumentNullException">Quando <paramref name="cep"/> for nulo.</exception>
        public ConsultaCepRequest(string cep)
        {
            Cep = cep ?? throw new ArgumentNullException(nameof(cep));
        }
    }
}
