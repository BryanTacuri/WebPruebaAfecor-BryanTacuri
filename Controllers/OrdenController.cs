using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPruebaAfecor.Models;
using WebPruebaAfecor.Services;


namespace WebPruebaAfecor.Controllers
{
    public class OrdenController : Controller
    {
        private readonly OrdenCompraManagementDbContext _context;
        private readonly ILogger<OrdenController> _logger;
        private readonly OrdenService _ordenService;

        private readonly List<string> _clientes = new List<string> { "Cliente1", "Cliente2", "Cliente3" , "Bryan Tacuri"};
        private readonly List<string> _productos = new List<string> { "Producto1", "POWERBANK", "TV ARVO", "REFRIGERADOR", };

        public OrdenController(OrdenCompraManagementDbContext context, ILogger<OrdenController> logger, OrdenService ordenService)
        {
            _context = context;
            _logger = logger;
            _ordenService = ordenService;
        }



        // GET: OrdenCompra/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var ordenes = await _ordenService.ObtenerOrdenesActivasAsync(searchTerm?.Trim());
                ViewData["searchTerm"] = searchTerm;
                return View(ordenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las órdenes.");
                return StatusCode(500, "Ocurrió un error al cargar las órdenes.");
            }
        }

        // GET: OrdenCompra/Crear
        public IActionResult Crear()
        {
            ViewBag.Clientes = _clientes;
            ViewBag.Productos = _productos;
            return View();
        }

        // POST: OrdenCompra/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenCompra orden)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _clientes;
                ViewBag.Productos = _productos;
                return View(orden);
            }

            try
            {
                _ordenService.AsignarValoresCreacion(orden);
                _context.Add(orden);
                await _context.SaveChangesAsync();

                _logger.LogInformation("OrdenCompra creada exitosamente: {IdOrden}", orden.IdOrden);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden.");
                ModelState.AddModelError("", "Ocurrió un error al crear la orden. Inténtalo nuevamente.");
                ViewBag.Clientes = _clientes;
                ViewBag.Productos = _productos;
                return View(orden);
            }
        }

        // GET: OrdenCompra/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var orden = await _ordenService.ObtenerOrdenPorIdAsync(id);
            if (orden == null)
            {
                _logger.LogWarning("OrdenCompra no encontrada o inactiva: {IdOrden}", id);
                return NotFound();
            }

            ViewBag.Clientes = _clientes;
            ViewBag.Productos = _productos;
            return View(orden);
        }

        // POST: OrdenCompra/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, OrdenCompra orden)
        {
            if (id != orden.IdOrden)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _clientes;
                ViewBag.Productos = _productos;
                return View(orden);
            }

            try
            {
                _ordenService.AsignarValoresModificacion(orden);
                _context.Update(orden);
                await _context.SaveChangesAsync();

                _logger.LogInformation("OrdenCompra actualizada exitosamente: {IdOrden}", orden.IdOrden);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _ordenService.OrdenExistsAsync(id))
                    return NotFound();

                _logger.LogError(ex, "Error de concurrencia al actualizar la orden: {IdOrden}", orden.IdOrden);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar la orden: {IdOrden}", orden.IdOrden);
                ModelState.AddModelError("", "Ocurrió un error al actualizar la orden. Inténtalo nuevamente.");
                ViewBag.Clientes = _clientes;
                ViewBag.Productos = _productos;
                return View(orden);
            }
        }

       

        // GET: Orden/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var orden = await _ordenService.ObtenerOrdenPorIdAsync(id);
            if (orden == null)
            {
                _logger.LogWarning("Orden no encontrada o inactiva: {IdOrden}", id);
                return NotFound();
            }

            return View(orden);
        }

        // POST: Orden/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var orden = await _ordenService.ObtenerOrdenPorIdAsync(id);
            if (orden == null)
            {
                _logger.LogWarning("Orden no encontrada para eliminación: {IdOrden}", id);
                return NotFound();
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                _ordenService.CambiarEstadoOrden(orden, 'I');
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Orden marcada como inactiva exitosamente: {IdOrden}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar como inactiva la orden: {IdOrden}", id);
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar la orden.");
                return View(orden);
            }
        }

    }
}
