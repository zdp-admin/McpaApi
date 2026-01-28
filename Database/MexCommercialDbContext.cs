using Microsoft.EntityFrameworkCore;

public class MexCommercialDbContext : DbContext
{
  public MexCommercialDbContext(DbContextOptions<MexCommercialDbContext> options) : base(options) {}
}