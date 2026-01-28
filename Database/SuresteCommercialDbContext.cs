using Microsoft.EntityFrameworkCore;

public class SuresteCommercialDbContext : DbContext
{
  public SuresteCommercialDbContext(DbContextOptions<SuresteCommercialDbContext> options) : base(options) {}
}