using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuebleriaProfe.Data;
using MuebleriaProfe.DTOs;
using MuebleriaProfe.Models;

namespace MuebleriaProfe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SyncController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<SyncPayload>> SincronizarTodo([FromBody] SyncPayload datosDelMovil)
        {
            await SincronizarUsuarios(datosDelMovil.Usuarios);
            await SincronizarTabla(datosDelMovil.Productos, _context.Productos);
            await SincronizarTabla(datosDelMovil.Clientes, _context.Clientes);

            await SincronizarVentasConDetalles(datosDelMovil.Ventas);

            await SincronizarTabla(datosDelMovil.PlanesPago, _context.PlanesPago);
            await SincronizarTabla(datosDelMovil.Abonos, _context.Abonos);
            await SincronizarTabla(datosDelMovil.Gastos, _context.Gastos);

            await _context.SaveChangesAsync();

            var fechaSync = datosDelMovil.UltimaSincronizacion;

            var respuestaAlMovil = new SyncPayload
            {
                UltimaSincronizacion = DateTime.UtcNow,

                Usuarios = await _context.Usuarios.Where(x => x.UpdatedAt > fechaSync).ToListAsync(),
                Productos = await _context.Productos.Where(x => x.UpdatedAt > fechaSync).ToListAsync(),
                Clientes = await _context.Clientes.Where(x => x.UpdatedAt > fechaSync).ToListAsync(),

                Ventas = await _context.Ventas
                                .Include(v => v.Detalles)
                                .Where(x => x.UpdatedAt > fechaSync)
                                .ToListAsync(),

                PlanesPago = await _context.PlanesPago.Where(x => x.UpdatedAt > fechaSync).ToListAsync(),
                Abonos = await _context.Abonos.Where(x => x.UpdatedAt > fechaSync).ToListAsync(),
                Gastos = await _context.Gastos.Where(x => x.UpdatedAt > fechaSync).ToListAsync()
            };

            return Ok(respuestaAlMovil);
        }

        private async Task SincronizarUsuarios(List<Usuario> usuariosMovil)
        {
            if (usuariosMovil == null || !usuariosMovil.Any()) return;

            foreach (var usuarioMovil in usuariosMovil)
            {
                var usuarioServidor = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == usuarioMovil.Id);

                usuarioServidor ??= await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == usuarioMovil.NombreUsuario);

                if (usuarioServidor == null && !string.IsNullOrEmpty(usuarioMovil.CorreoElectronico))
                {
                    usuarioServidor = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.CorreoElectronico == usuarioMovil.CorreoElectronico);
                }

                if (usuarioServidor == null)
                {
                    await _context.Usuarios.AddAsync(usuarioMovil);
                }
                else
                {
                    bool esDelete = usuarioMovil.IsDeleted;
                    bool esMasNuevo = usuarioMovil.UpdatedAt > usuarioServidor.UpdatedAt.AddMilliseconds(10);

                    if (esDelete || esMasNuevo)
                    {
                        usuarioServidor.NombreUsuario = usuarioMovil.NombreUsuario;
                        usuarioServidor.NombreCompleto = usuarioMovil.NombreCompleto;
                        usuarioServidor.Password = usuarioMovil.Password;
                        usuarioServidor.CorreoElectronico = usuarioMovil.CorreoElectronico;
                        usuarioServidor.IsAdmin = usuarioMovil.IsAdmin;
                        usuarioServidor.Permisos = usuarioMovil.Permisos;
                        usuarioServidor.Activo = usuarioMovil.Activo;
                        usuarioServidor.IsDeleted = usuarioMovil.IsDeleted;
                        usuarioServidor.UpdatedAt = usuarioMovil.UpdatedAt;
                        usuarioServidor.UsuarioUuid = usuarioMovil.UsuarioUuid;
                        usuarioServidor.UsuarioNombre = usuarioMovil.UsuarioNombre;
                    }
                }
            }
        }

        private async Task SincronizarVentasConDetalles(List<Venta> ventasMovil)
        {
            if (ventasMovil == null || !ventasMovil.Any()) return;

            foreach (var ventaMovil in ventasMovil)
            {
                var ventaServidor = await _context.Ventas
                    .Include(v => v.Detalles)
                    .FirstOrDefaultAsync(v => v.Id == ventaMovil.Id);

                if (ventaServidor == null)
                {
                    await _context.Ventas.AddAsync(ventaMovil);
                }
                else
                {
                    bool esDelete = ventaMovil.IsDeleted;
                    bool esMasNuevo = ventaMovil.UpdatedAt > ventaServidor.UpdatedAt.AddMilliseconds(10);

                    if (esDelete || esMasNuevo)
                    {
                        _context.Entry(ventaServidor).CurrentValues.SetValues(ventaMovil);
                        ventaServidor.IsDeleted = ventaMovil.IsDeleted;
                        ventaServidor.UpdatedAt = ventaMovil.UpdatedAt;
                        ventaServidor.UsuarioUuid = ventaMovil.UsuarioUuid;
                        ventaServidor.UsuarioNombre = ventaMovil.UsuarioNombre;
                        _context.Entry(ventaServidor).State = EntityState.Modified;

                        if (ventaMovil.Detalles != null && ventaMovil.Detalles.Any())
                        {
                            _context.VentaDetalles.RemoveRange(ventaServidor.Detalles);

                            foreach (var detalleMovil in ventaMovil.Detalles)
                            {
                                detalleMovil.VentaId = ventaServidor.Id;
                                await _context.VentaDetalles.AddAsync(detalleMovil);
                            }
                        }
                    }
                }
            }
        }

        // --- EL MÉTODO GENÉRICO BLINDADO ---
        private async Task SincronizarTabla<T>(List<T> registrosMovil, DbSet<T> dbSetServidor) where T : BaseEntity
        {
            if (registrosMovil == null || !registrosMovil.Any()) return;

            foreach (var registroMovil in registrosMovil)
            {
                var registroServidor = await dbSetServidor.FindAsync(registroMovil.Id);

                if (registroServidor == null)
                {
                    await dbSetServidor.AddAsync(registroMovil);
                }
                else
                {
                    bool esDelete = registroMovil.IsDeleted;
                    bool esMasNuevo = registroMovil.UpdatedAt > registroServidor.UpdatedAt.AddMilliseconds(10);

                    if (esDelete)
                    {
                        registroServidor.IsDeleted = true;
                        registroServidor.UpdatedAt = registroMovil.UpdatedAt;
                        registroServidor.UsuarioUuid = registroMovil.UsuarioUuid;
                        registroServidor.UsuarioNombre = registroMovil.UsuarioNombre;
                        _context.Entry(registroServidor).State = EntityState.Modified;
                    }
                    else if (esMasNuevo)
                    {
                        _context.Entry(registroServidor).CurrentValues.SetValues(registroMovil);
                        registroServidor.IsDeleted = registroMovil.IsDeleted;
                        registroServidor.UpdatedAt = registroMovil.UpdatedAt;
                        registroServidor.UsuarioUuid = registroMovil.UsuarioUuid;
                        registroServidor.UsuarioNombre = registroMovil.UsuarioNombre;
                        _context.Entry(registroServidor).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}