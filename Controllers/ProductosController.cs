using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuebleriaProfe.Data;
using MuebleriaProfe.Models;

namespace MuebleriaProfe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inyectamos la base de datos en el controlador
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        // Este método devuelve todos los productos que NO han sido eliminados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos
                                          .Where(p => p.IsDeleted == false)
                                          .ToListAsync();
            return Ok(productos);
        }

        // POST: api/Productos
        // Este método recibe un producto desde el móvil/web y lo guarda en PostgreSQL
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Devuelve un estado 201 (Creado) y muestra cómo quedó el producto en BD
            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }
    }
}