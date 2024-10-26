using System;
using System.Collections.Generic;

namespace WebPruebaAfecor.Models;

public partial class OrdenCompra
{
    public int IdOrden { get; set; }

    public string NombreCliente { get; set; } = null!;

    public string NombreProducto { get; set; } = null!;

    public DateTime FechaOrden { get; set; }

    public decimal TotalOrden { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }
}
