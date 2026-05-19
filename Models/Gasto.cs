using System;

namespace MuebleriaProfe.Models // Tu namespace real
{
    // 1. Le decimos que hereda de BaseEntity (¡Esto soluciona el error!)
    public class Gasto : BaseEntity
    {
        // 2. Solo declaramos lo exclusivo del Gasto. 
        // (No pongas el Id, CreatedAt, etc., porque ya vienen incluidos mágicamente en BaseEntity)

        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}