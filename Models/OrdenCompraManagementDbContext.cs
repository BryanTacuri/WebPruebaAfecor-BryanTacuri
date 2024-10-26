using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebPruebaAfecor.Models;

public partial class OrdenCompraManagementDbContext : DbContext
{
    public OrdenCompraManagementDbContext()
    {
    }

    public OrdenCompraManagementDbContext(DbContextOptions<OrdenCompraManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrdenCompra> OrdenCompras { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrdenCompra>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__OrdenCom__C38F300DD4FC8104");

            entity.ToTable("OrdenCompra");

            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaOrden)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCliente).HasMaxLength(100);
            entity.Property(e => e.NombreProducto).HasMaxLength(100);
            entity.Property(e => e.TotalOrden).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Sistema");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
