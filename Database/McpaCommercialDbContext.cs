using McpaApi.Models.Commercial;
using Microsoft.EntityFrameworkCore;

public class McpaCommercialDbContext : DbContext
{
  public McpaCommercialDbContext(DbContextOptions<McpaCommercialDbContext> options) : base(options) { }

  public DbSet<Documents> Documents { get; set; }
  public DbSet<Movements> Movements { get; set; }
  public DbSet<Products> Products { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {

    modelBuilder.Entity<Documents>(entity =>
    {
      entity.HasKey(e => new { e.Id }).HasName("PK_admDocumentos");
      entity.Property(property => property.SerieDocumento).HasColumnType("varchar(11)");
      entity.Property(property => property.Folio).HasColumnType("float");
      entity.Property(property => property.RazonSocial).HasColumnType("varchar(60)");
      entity.Property(property => property.RFC).HasColumnType("varchar(20)");
      entity.Property(property => property.Referencia).HasColumnType("varchar(20)");
      entity.Property(property => property.Neto).HasColumnType("float");
      entity.Property(property => property.Impuesto1).HasColumnType("float");
      entity.Property(property => property.Total).HasColumnType("float");
      entity.Property(property => property.Pendiente).HasColumnType("float");
      entity.Property(property => property.TotalUnidades).HasColumnType("float");
      entity.Property(property => property.UnidadesPendientes).HasColumnType("float");

      entity.HasMany(d => d.Movements).WithOne(m => m.Document).HasForeignKey(d => d.IdDocumento);
    });

    modelBuilder.Entity<Movements>(entity =>
    {
      entity.HasKey(e => new { e.Id }).HasName("PK_admMovimientos");
      entity.Property(property => property.NumeroMovimiento).HasColumnType("float");
      entity.Property(property => property.Unidades).HasColumnType("float");
      entity.Property(property => property.Precio).HasColumnType("float");
      entity.Property(property => property.Neto).HasColumnType("float");
      entity.Property(property => property.Total).HasColumnType("float");

      entity.HasOne(m => m.Document).WithMany(d => d.Movements).HasForeignKey(m => m.IdDocumento);
    });

    base.OnModelCreating(modelBuilder);
  }
}