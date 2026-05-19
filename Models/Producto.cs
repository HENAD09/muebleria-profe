using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuebleriaProfe.Models
{
    public class Producto : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string CodigoSku { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Subfamilia { get; set; } = "Sin Categoría";

        // --- NUEVOS CAMPOS AGREGADOS ---

        [MaxLength(50)]
        public string Color { get; set; } = "Estándar"; // Para el submenú de variantes

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; } // Lo que te costó a ti

        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentajeGananciaContado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentajeGananciaCredito { get; set; }

        // ------------------------------

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioContado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCredito { get; set; }

        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
    }
}