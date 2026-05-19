namespace MuebleriaProfe.Models
{
    public class Abono : BaseEntity
    {
        public Guid PlanPagoId { get; set; }
        public PlanPago? PlanPago { get; set; }

        public Guid ClienteId { get; set; }

        // --- TUS CAMPOS CONTABLES ESTRICTOS ---
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;
        public decimal MontoPagado { get; set; }

        // CRÍTICO: Guarda el saldo que quedaba en ese momento exacto
        public decimal SaldoRestante { get; set; }
        
    }
}