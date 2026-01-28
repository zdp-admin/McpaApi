using McpaApi.Models.Mysql.Shop;
using McpaApi.Models.Shop;
using Microsoft.EntityFrameworkCore;

public class ShopGarageDbContext : DbContext
{
  public ShopGarageDbContext(DbContextOptions<ShopGarageDbContext> options) : base(options) { }

  public DbSet<Sales> Sales { get; set; }
  public DbSet<SaleElement> SaleElements { get; set; }
  public DbSet<VehicleBrand> vehicleBrands { get; set; }
  public DbSet<VehicleModel> vehicleModels { get; set; }
  public DbSet<ServiceCommissions> serviceCommissions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Sales>(entity =>
    {
      entity.Property(property => property.IsCotization).HasColumnType("TINYINT");
      entity.HasOne(e => e.Invoice).WithMany(i => i.Sales).HasForeignKey(s => s.InvoiceId);
      entity.HasMany(e => e.SaleElements).WithOne(se => se.Sale).HasForeignKey(se => se.SaleId);
      entity.HasOne(e => e.User).WithMany(u => u.Sales).HasForeignKey(s => s.ByUserId);
      entity.HasOne(e => e.Agency).WithMany(a => a.Sales).HasForeignKey(s => s.ClientId);
    });

    modelBuilder.Entity<SaleElement>(entity =>
    {
      entity.HasOne(e => e.Product).WithMany(p => p.SaleElements).HasForeignKey(se => se.ProductId);
      entity.HasOne(e => e.ServiceCommissionCode).WithMany(s => s.SaleElements).HasForeignKey(se => se.ServiceCommissionId);
    });

    base.OnModelCreating(modelBuilder);
  }
}