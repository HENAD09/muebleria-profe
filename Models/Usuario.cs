using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuebleriaProfe.Models // (Asegúrate de que este namespace coincida con tu proyecto)
{
    [Table("usuarios")]
    public class Usuario : BaseEntity // 👈 LA MAGIA: Heredamos del molde principal
    {
        // ❌ Borramos Id, Uuid, CreatedAt, UpdatedAt e IsDeleted
        // porque BaseEntity ya se encarga de inyectarlas automáticamente.

        [Required]
        [MaxLength(50)]
        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("correo_electronico")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Column("is_admin")]
        public bool IsAdmin { get; set; } = false;

        // 🚀 PostgreSQL: Soporta arreglos de texto nativos
        [Column("permisos")]
        public string[] Permisos { get; set; } = Array.Empty<string>();

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}