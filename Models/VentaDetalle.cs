using System.Text.Json.Serialization;

namespace MuebleriaProfe.Models
{
    public class VentaDetalle : BaseEntity
    {
        public Guid VentaId { get; set; }

        [JsonIgnore] // Evita el error de "Referencia Circular" al enviar datos por red
        public Venta? Venta { get; set; }

        public Guid ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public string? ProductoNombre { get; set; }
        public string? Color { get; set; } // Fundamental para saber qué variante se llevó

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}