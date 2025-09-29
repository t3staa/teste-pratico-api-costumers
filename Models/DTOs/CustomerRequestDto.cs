using System.ComponentModel.DataAnnotations;

namespace teste_pratico.Models.DTOs
{
    /// <summary>
    /// DTO para requisições de criação e atualização de clientes
    /// </summary>
    public class CustomerRequestDto
    {
        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João Silva</example>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email do cliente
        /// </summary>
        /// <example>joao.silva@example.com</example>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "O email não pode ter mais de 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endereço do cliente (apenas números)
        /// </summary>
        /// <example>01310100</example>
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "O CEP deve conter exatamente 8 dígitos numéricos")]
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Valida e normaliza o CEP removendo caracteres não numéricos
        /// </summary>
        public string GetNormalizedCep()
        {
            if (string.IsNullOrWhiteSpace(Cep))
                return string.Empty;

            // Remove todos os caracteres não numéricos
            return System.Text.RegularExpressions.Regex.Replace(Cep, @"\D", "");
        }
    }
}