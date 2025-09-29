using System.Text.Json.Serialization;

namespace teste_pratico.Models.DTOs
{
    /// <summary>
    /// DTO para deserializar a resposta da API ViaCEP
    /// </summary>
    public class ViaCepResponseDto
    {
        /// <summary>
        /// CEP formatado
        /// </summary>
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        /// <summary>
        /// Logradouro
        /// </summary>
        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        /// <summary>
        /// Complemento do endereço
        /// </summary>
        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        /// <summary>
        /// Cidade/Localidade
        /// </summary>
        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        /// <summary>
        /// Unidade Federativa (Estado)
        /// </summary>
        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        /// <summary>
        /// Código IBGE da cidade
        /// </summary>
        [JsonPropertyName("ibge")]
        public string? Ibge { get; set; }

        /// <summary>
        /// Código GIA
        /// </summary>
        [JsonPropertyName("gia")]
        public string? Gia { get; set; }

        /// <summary>
        /// Código DDD da região
        /// </summary>
        [JsonPropertyName("ddd")]
        public string? Ddd { get; set; }

        /// <summary>
        /// Código SIAFI
        /// </summary>
        [JsonPropertyName("siafi")]
        public string? Siafi { get; set; }

        /// <summary>
        /// Indica se houve erro na busca do CEP
        /// </summary>
        [JsonPropertyName("erro")]
        public bool? Erro { get; set; }

        /// <summary>
        /// Verifica se o CEP é válido (não possui erro)
        /// </summary>
        public bool IsValid => !Erro.GetValueOrDefault(false) && !string.IsNullOrWhiteSpace(Cep);

        /// <summary>
        /// Verifica se possui dados de endereço completos
        /// </summary>
        public bool HasCompleteAddress =>
            !string.IsNullOrWhiteSpace(Logradouro) &&
            !string.IsNullOrWhiteSpace(Localidade) &&
            !string.IsNullOrWhiteSpace(Uf);
    }
}