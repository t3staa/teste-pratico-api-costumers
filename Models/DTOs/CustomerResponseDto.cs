namespace teste_pratico.Models.DTOs
{
    /// <summary>
    /// DTO para respostas contendo dados completos do cliente
    /// </summary>
    public class CustomerResponseDto
    {
        /// <summary>
        /// Identificador único do cliente
        /// </summary>
        /// <example>1</example>
        public long Id { get; set; }

        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João Silva</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email do cliente
        /// </summary>
        /// <example>joao.silva@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endereço
        /// </summary>
        /// <example>01310100</example>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Logradouro (rua, avenida, etc.)
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string? Street { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        /// <example>São Paulo</example>
        public string? City { get; set; }

        /// <summary>
        /// Estado (UF)
        /// </summary>
        /// <example>SP</example>
        public string? State { get; set; }

        /// <summary>
        /// Data e hora de criação do registro
        /// </summary>
        /// <example>2024-09-28T10:30:00Z</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última atualização
        /// </summary>
        /// <example>2024-09-28T15:45:00Z</example>
        public DateTime? UpdatedAt { get; set; }
    }
}