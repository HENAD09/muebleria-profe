using System.ComponentModel.DataAnnotations;

namespace MuebleriaProfe.Models
{
    public class PlanPago : BaseEntity
    {
        public Guid VentaId { get; set; }
        public Venta? Venta { get; set; }

        public Guid ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ArticuloResumen { get; set; }

        // --- TUS CAMPOS ORIGINALES ---
        public decimal Enganche { get; set; }
        public decimal SaldoInicial { get; set; } // Total Venta - Enganche

        [MaxLength(20)]
        public string FrecuenciaPago { get; set; } = "semanal"; // Ajustado a semanal por la mueblería

        // --- CAMPOS DE FLUTTER ---
        public decimal TotalCredito { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal PagoSemanalAcordado { get; set; }
        public decimal SaldoVencido { get; set; }
        public DateTime? ProximoCobro { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo"; // Activo, Atrasado, Pagado
    }
}