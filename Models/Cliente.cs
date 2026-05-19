using System.ComponentModel.DataAnnotations;

namespace MuebleriaProfe.Models
{
    public class Cliente : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        // Útil si quieres darle un límite de crédito a ciertos clientes
        public decimal LimiteCredito { get; set; }

        [MaxLength(20)]
        public string? Rfc { get; set; }

        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
    }
}