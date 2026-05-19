using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuebleriaProfe.Models;
using MuebleriaProfe.Data;
using System.Threading.Tasks;

namespace MuebleriaAPI.Controllers // O MuebleriaProfe.Controllers según tu proyecto
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/usuarios/sincronizar
        // Aquí Flutter enviará el Usuario nuevo para que C# lo guarde en PostgreSQL
        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarUsuario([FromBody] Usuario nuevoUsuario)
        {
            var existe = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == nuevoUsuario.Id);

            if (existe == null)
            {
                existe = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nuevoUsuario.NombreUsuario);
            }

            if (existe == null && !string.IsNullOrEmpty(nuevoUsuario.CorreoElectronico))
            {
                existe = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == nuevoUsuario.CorreoElectronico);
            }

            if (existe != null)
            {
                existe.NombreUsuario = nuevoUsuario.NombreUsuario;
                existe.NombreCompleto = nuevoUsuario.NombreCompleto;
                existe.Password = nuevoUsuario.Password;
                existe.CorreoElectronico = nuevoUsuario.CorreoElectronico;
                existe.IsAdmin = nuevoUsuario.IsAdmin;
                existe.Permisos = nuevoUsuario.Permisos;
                existe.Activo = nuevoUsuario.Activo;
            }
            else
            {
                await _context.Usuarios.AddAsync(nuevoUsuario);
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario guardado exitosamente en PostgreSQL" });
        }
    }
}