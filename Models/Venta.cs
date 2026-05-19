using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MuebleriaProfe.Models
{
    public class Venta : BaseEntity
    {
        public Guid ClienteId { get; set; }
        public Cliente? Cliente { get; set; } // Relación con la tabla Cliente

        // Guardamos el nombre por si en el futuro se borra el cliente, el ticket no quede vacío
        public string? ClienteNombre { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;
        public decimal TotalVenta { get; set; }

        [MaxLength(50)]
        public string TipoVenta { get; set; } = "Contado"; // "Contado" o "Híbrido/Crédito"

        [MaxLength(20)]
        public string Estado { get; set; } = "completada"; // "completada", "pendiente", "cancelada"

        // --- LA MAGIA: Esta lista recibirá todo el carrito de compras desde Flutter ---
        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}