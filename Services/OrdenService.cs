using Microsoft.EntityFrameworkCore;
using WebPruebaAfecor.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebPruebaAfecor.Services
{
    public class OrdenService
    {
        private readonly OrdenCompraManagementDbContext _context;

        public OrdenService(OrdenCompraManagementDbContext context)
        {
            _context = context;
        }


        public async Task<OrdenCompra?> ObtenerOrdenPorIdAsync(int id)
        {
            return await _context.OrdenCompras.FirstOrDefaultAsync(o => o.IdOrden == id && o.Estado != "I");
        }


        public void AsignarValoresCreacion(OrdenCompra orden)
        {
            orden.FechaCreacion = DateTime.Now;
            orden.UsuarioCreacion = ObtenerUsuarioAutenticado();
            orden.Estado = "A";
        }

        public void AsignarValoresModificacion(OrdenCompra orden)
        {
            orden.FechaModificacion = DateTime.Now;
            orden.UsuarioModificacion = ObtenerUsuarioAutenticado();
        }

        public void CambiarEstadoOrden(OrdenCompra orden, char estado)
        {
            orden.Estado = estado.ToString();
            orden.FechaModificacion = DateTime.Now;
            orden.UsuarioModificacion = ObtenerUsuarioAutenticado();
        }

        public string ObtenerUsuarioAutenticado()
        {
            return  "Sistema";
        }

        public async Task<bool> OrdenExistsAsync(int id)
        {
            return await _context.OrdenCompras.AnyAsync(e => e.IdOrden == id && e.Estado != "I");
        }

        public async Task<List<OrdenCompra>> ObtenerOrdenesActivasAsync(string? searchTerm = null)
        {
            var query = _context.OrdenCompras.Where(o => o.Estado != "I");

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                DateTime parsedDate;
                bool isDate = DateTime.TryParseExact(searchTerm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate);

                query = query.Where(o =>
                    o.NombreCliente.ToLower().Contains(searchTerm) ||
                    o.NombreProducto.ToLower().Contains(searchTerm) ||
                    (isDate && o.FechaOrden.Date == parsedDate.Date));
            }

            return await query.ToListAsync();
        }

    }
}
