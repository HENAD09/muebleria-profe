using MuebleriaProfe.Models;
using System;
using System.Collections.Generic;


namespace MuebleriaProfe.DTOs
{
    public class SyncPayload
    {
        public DateTime UltimaSincronizacion { get; set; }

        // Listas de datos que suben/bajan
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Venta> Ventas { get; set; } = new List<Venta>();
        public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
        public List<PlanPago> PlanesPago { get; set; } = new List<PlanPago>();
        public List<Abono> Abonos { get; set; } = new List<Abono>();

        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}