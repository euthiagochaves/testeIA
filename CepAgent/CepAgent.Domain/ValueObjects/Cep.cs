using System;
using System.Text.RegularExpressions;

namespace CepAgent.Domain.ValueObjects
{
    /// <summary>
    /// Representa um CEP brasileiro como Value Object.
    /// Normaliza (remove máscara) e valida o formato (8 dígitos).
    /// Imutável e leve para uso no domínio.
    /// </summary>
    public readonly record struct Cep
    {
        /// <summary>
        /// Valor do CEP normalizado (apenas dígitos, 8 caracteres).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Cria uma nova instância de <see cref="Cep"/> após normalizar e validar o valor informado.
        /// </summary>
        /// <param name="value">CEP informando — pode conter máscara (traço, ponto, espaços).</param>
        /// <exception cref="ArgumentNullException">Quando <paramref name="value"/> for nulo ou vazio.</exception>
        /// <exception cref="ArgumentException">Quando o valor normalizado não possuir exatamente 8 dígitos.</exception>
        public Cep(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "CEP não pode ser nulo ou vazio.");

            // Remover quaisquer caracteres não numéricos (máscaras, espaços, etc.)
            var normalized = Regex.Replace(value, "[^0-9]", string.Empty);

            if (normalized.Length != 8)
                throw new ArgumentException("CEP inválido. O CEP deve conter exatamente 8 dígitos.", nameof(value));

            Value = normalized;
        }

        /// <summary>
        /// Retorna o CEP no formato normalizado (apenas dígitos).
        /// </summary>
        /// <returns>String com 8 dígitos.</returns>
        public override string ToString() => Value;

        /// <summary>
        /// Retorna o CEP formatado no padrão "00000-000".
        /// </summary>
        public string ToMaskedString()
        {
            if (string.IsNullOrEmpty(Value) || Value.Length != 8)
                return Value;

            return Value.Substring(0, 5) + "-" + Value.Substring(5, 3);
        }
    }
}
