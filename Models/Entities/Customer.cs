using System.ComponentModel.DataAnnotations;

namespace teste_pratico.Models.Entities
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP must contain exactly 8 digits")]
        public string Cep { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}