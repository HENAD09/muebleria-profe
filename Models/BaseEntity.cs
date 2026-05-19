using System;

namespace MuebleriaProfe.Models
{
    public abstract class BaseEntity
    {
        // Usamos Guid (UUID) para que el móvil pueda generar IDs sin internet
        public Guid Id { get; set; } = Guid.NewGuid();

        // Control de tiempo para saber qué registro gana en caso de conflicto
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Soft Delete: Si se borra sin internet, se marca como true
        public bool IsDeleted { get; set; } = false;

        // Rastreo de auditoría (Quién hizo el movimiento)
        public Guid? UsuarioUuid { get; set; }
        public string? UsuarioNombre { get; set; }
    }
}