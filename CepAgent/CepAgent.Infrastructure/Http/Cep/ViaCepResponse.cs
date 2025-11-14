using System.Text.Json.Serialization;

namespace CepAgent.Infrastructure.Http.Cep
{
    /// <summary>
    /// Modelo que representa o contrato de resposta da API ViaCEP.
    /// Campos mapeados conforme retornados pela API: https://viacep.com.br
    /// Apenas os campos relevantes para montar o domínio `Endereco` foram incluídos.
    /// </summary>
    public sealed class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("ibge")]
        public string? Ibge { get; set; }

        [JsonPropertyName("gia")]
        public string? Gia { get; set; }

        [JsonPropertyName("ddd")]
        public string? Ddd { get; set; }

        [JsonPropertyName("siafi")]
        public string? Siafi { get; set; }

        // ViaCEP retorna { "erro": true } quando o CEP não é encontrado.
        [JsonPropertyName("erro")]
        public bool? Erro { get; set; }
    }
}