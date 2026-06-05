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
            // Capturar el momento del servidor ANTES de procesar, para estampar
            // todos los registros aceptados con el mismo reloj.
            var ahora = DateTime.UtcNow;

            await SincronizarUsuarios(datosDelMovil.Usuarios, ahora);
            await SincronizarTabla(datosDelMovil.Productos, _context.Productos, ahora);
            await SincronizarTabla(datosDelMovil.Clientes, _context.Clientes, ahora);

            await SincronizarVentasConDetalles(datosDelMovil.Ventas, ahora);

            await SincronizarTabla(datosDelMovil.PlanesPago, _context.PlanesPago, ahora);
            await SincronizarTabla(datosDelMovil.Abonos, _context.Abonos, ahora);
            await SincronizarTabla(datosDelMovil.Gastos, _context.Gastos, ahora);

            await _context.SaveChangesAsync();

            // FIX #4: Recalcular saldos de PlanesPago desde los Abonos reales.
            // Esto garantiza consistencia cuando 2 dispositivos procesan abonos
            // sobre el mismo plan entre syncs — ambos Abonos llegan al servidor,
            // y el saldo se recalcula desde la fuente de verdad.
            await RecalcularSaldosPlanesPago();

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

        /// <summary>
        /// FIX #4: Recalcula saldoPendiente de todos los PlanesPago activos
        /// a partir de la suma real de Abonos no eliminados.
        /// Esto resuelve el conflicto cuando 2 dispositivos procesan abonos
        /// sobre el mismo plan: ambos Abonos se sincronizan correctamente
        /// (tienen UUIDs distintos), y el saldo se deriva de la verdad.
        /// </summary>
        private async Task RecalcularSaldosPlanesPago()
        {
            var planesActivos = await _context.PlanesPago
                .Where(p => !p.IsDeleted && p.Estado != "Pagado")
                .ToListAsync();

            if (!planesActivos.Any()) return;

            var planIds = planesActivos.Select(p => p.Id).ToList();

            // Una sola query: suma de abonos agrupada por planPagoId
            var sumaAbonos = await _context.Abonos
                .Where(a => !a.IsDeleted && planIds.Contains(a.PlanPagoId))
                .GroupBy(a => a.PlanPagoId)
                .Select(g => new { PlanId = g.Key, Total = g.Sum(a => a.MontoPagado) })
                .ToDictionaryAsync(x => x.PlanId, x => x.Total);

            bool huboCambios = false;

            foreach (var plan in planesActivos)
            {
                sumaAbonos.TryGetValue(plan.Id, out var totalAbonado);
                decimal saldoCorrecto = (plan.TotalCredito - plan.Enganche) - totalAbonado;
                if (saldoCorrecto < 0) saldoCorrecto = 0;

                // Solo actualizar si hay discrepancia (tolerancia de 1 centavo)
                if (Math.Abs(plan.SaldoPendiente - saldoCorrecto) > 0.01m)
                {
                    plan.SaldoPendiente = saldoCorrecto;
                    plan.UpdatedAt = DateTime.UtcNow;
                    plan.Version++; // Incrementar para que los dispositivos descarguen la corrección

                    if (saldoCorrecto <= 0)
                    {
                        plan.Estado = "Pagado";
                        plan.SaldoVencido = 0;
                    }

                    huboCambios = true;
                }
            }

            if (huboCambios)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task SincronizarUsuarios(List<Usuario> usuariosMovil, DateTime ahoraServidor)
        {
            if (usuariosMovil == null || !usuariosMovil.Any()) return;

            // FIX #2: Carga batch
            var idsMovil = usuariosMovil.Select(u => u.Id).ToList();
            var nombresMovil = usuariosMovil.Select(u => u.NombreUsuario).ToList();
            var correosMovil = usuariosMovil
                .Where(u => !string.IsNullOrEmpty(u.CorreoElectronico))
                .Select(u => u.CorreoElectronico)
                .ToList();

            var servidorPorId = await _context.Usuarios
                .Where(u => idsMovil.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var servidorPorNombre = await _context.Usuarios
                .Where(u => nombresMovil.Contains(u.NombreUsuario))
                .ToDictionaryAsync(u => u.NombreUsuario);

            var servidorPorCorreo = correosMovil.Any()
                ? await _context.Usuarios
                    .Where(u => correosMovil.Contains(u.CorreoElectronico))
                    .ToDictionaryAsync(u => u.CorreoElectronico!)
                : new Dictionary<string, Usuario>();

            foreach (var usuarioMovil in usuariosMovil)
            {
                servidorPorId.TryGetValue(usuarioMovil.Id, out var usuarioServidor);
                if (usuarioServidor == null)
                    servidorPorNombre.TryGetValue(usuarioMovil.NombreUsuario, out usuarioServidor);
                if (usuarioServidor == null && !string.IsNullOrEmpty(usuarioMovil.CorreoElectronico))
                    servidorPorCorreo.TryGetValue(usuarioMovil.CorreoElectronico!, out usuarioServidor);

                if (usuarioServidor == null)
                {
                    usuarioMovil.UpdatedAt = ahoraServidor;
                    await _context.Usuarios.AddAsync(usuarioMovil);
                }
                else
                {
                    // FIX #4: Comparación por versión (independiente de reloj)
                    bool aceptar = usuarioMovil.IsDeleted
                        || usuarioMovil.Version > usuarioServidor.Version
                        || (usuarioMovil.Version == usuarioServidor.Version
                            && usuarioMovil.UpdatedAt > usuarioServidor.UpdatedAt);

                    if (aceptar)
                    {
                        usuarioServidor.NombreUsuario = usuarioMovil.NombreUsuario;
                        usuarioServidor.NombreCompleto = usuarioMovil.NombreCompleto;
                        usuarioServidor.Password = usuarioMovil.Password;
                        usuarioServidor.CorreoElectronico = usuarioMovil.CorreoElectronico;
                        usuarioServidor.IsAdmin = usuarioMovil.IsAdmin;
                        usuarioServidor.Permisos = usuarioMovil.Permisos;
                        usuarioServidor.Activo = usuarioMovil.Activo;
                        usuarioServidor.IsDeleted = usuarioMovil.IsDeleted;
                        usuarioServidor.UpdatedAt = ahoraServidor; // Reloj del servidor
                        usuarioServidor.Version = usuarioMovil.Version;
                        usuarioServidor.UsuarioUuid = usuarioMovil.UsuarioUuid;
                        usuarioServidor.UsuarioNombre = usuarioMovil.UsuarioNombre;
                    }
                }
            }
        }

        private async Task SincronizarVentasConDetalles(List<Venta> ventasMovil, DateTime ahoraServidor)
        {
            if (ventasMovil == null || !ventasMovil.Any()) return;

            // FIX #2: Carga batch
            var idsMovil = ventasMovil.Select(v => v.Id).ToList();
            var ventasServidorDict = await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => idsMovil.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id);

            foreach (var ventaMovil in ventasMovil)
            {
                ventasServidorDict.TryGetValue(ventaMovil.Id, out var ventaServidor);

                if (ventaServidor == null)
                {
                    ventaMovil.UpdatedAt = ahoraServidor;
                    await _context.Ventas.AddAsync(ventaMovil);
                }
                else
                {
                    // FIX #4: Comparación por versión
                    bool aceptar = ventaMovil.IsDeleted
                        || ventaMovil.Version > ventaServidor.Version
                        || (ventaMovil.Version == ventaServidor.Version
                            && ventaMovil.UpdatedAt > ventaServidor.UpdatedAt);

                    if (aceptar)
                    {
                        _context.Entry(ventaServidor).CurrentValues.SetValues(ventaMovil);
                        ventaServidor.IsDeleted = ventaMovil.IsDeleted;
                        ventaServidor.UpdatedAt = ahoraServidor; // Reloj del servidor
                        ventaServidor.Version = ventaMovil.Version;
                        ventaServidor.UsuarioUuid = ventaMovil.UsuarioUuid;
                        ventaServidor.UsuarioNombre = ventaMovil.UsuarioNombre;
                        _context.Entry(ventaServidor).State = EntityState.Modified;

                        // FIX #5: Upsert de detalles en vez de DELETE + INSERT
                        if (ventaMovil.Detalles != null && ventaMovil.Detalles.Any())
                        {
                            var detallesServidorDict = ventaServidor.Detalles
                                .ToDictionary(d => d.Id);

                            var idsDetallesMovil = new HashSet<Guid>();

                            foreach (var detalleMovil in ventaMovil.Detalles)
                            {
                                detalleMovil.VentaId = ventaServidor.Id;
                                idsDetallesMovil.Add(detalleMovil.Id);

                                if (detallesServidorDict.TryGetValue(detalleMovil.Id, out var detalleServidor))
                                {
                                    _context.Entry(detalleServidor).CurrentValues.SetValues(detalleMovil);
                                    detalleServidor.IsDeleted = detalleMovil.IsDeleted;
                                    detalleServidor.UpdatedAt = detalleMovil.UpdatedAt;
                                }
                                else
                                {
                                    await _context.VentaDetalles.AddAsync(detalleMovil);
                                }
                            }

                            foreach (var detalleServidor in ventaServidor.Detalles)
                            {
                                if (!idsDetallesMovil.Contains(detalleServidor.Id))
                                {
                                    detalleServidor.IsDeleted = true;
                                    detalleServidor.UpdatedAt = DateTime.UtcNow;
                                }
                            }
                        }
                    }
                }
            }
        }

        // --- MÉTODO GENÉRICO (FIX #2 batch + FIX #4 versión) ---
        private async Task SincronizarTabla<T>(List<T> registrosMovil, DbSet<T> dbSetServidor, DateTime ahoraServidor) where T : BaseEntity
        {
            if (registrosMovil == null || !registrosMovil.Any()) return;

            // FIX #2: UNA sola query en vez de N FindAsync
            var idsMovil = registrosMovil.Select(r => r.Id).ToList();
            var servidorDict = await dbSetServidor
                .Where(r => idsMovil.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id);

            foreach (var registroMovil in registrosMovil)
            {
                servidorDict.TryGetValue(registroMovil.Id, out var registroServidor);

                if (registroServidor == null)
                {
                    // Registro nuevo: estampar con reloj del servidor
                    registroMovil.UpdatedAt = ahoraServidor;
                    await dbSetServidor.AddAsync(registroMovil);
                }
                else
                {
                    // FIX #4: Comparación por versión (independiente de reloj)
                    // 1) Versión mayor → siempre gana
                    // 2) Versión igual + timestamp mayor → desempate (legacy con version=0)
                    // 3) IsDeleted → siempre se propaga
                    bool aceptar = registroMovil.IsDeleted
                        || registroMovil.Version > registroServidor.Version
                        || (registroMovil.Version == registroServidor.Version
                            && registroMovil.UpdatedAt > registroServidor.UpdatedAt);

                    if (aceptar)
                    {
                        _context.Entry(registroServidor).CurrentValues.SetValues(registroMovil);
                        registroServidor.IsDeleted = registroMovil.IsDeleted;
                        registroServidor.UpdatedAt = ahoraServidor; // Reloj del servidor
                        registroServidor.Version = registroMovil.Version;
                        registroServidor.UsuarioUuid = registroMovil.UsuarioUuid;
                        registroServidor.UsuarioNombre = registroMovil.UsuarioNombre;
                        _context.Entry(registroServidor).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}
