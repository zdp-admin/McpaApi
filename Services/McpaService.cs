using McpaApi.Models.Commercial;

namespace McpaApi.Services
{
  public class McpaService
  {
    protected readonly McpaCommercialDbContext _context;
    public McpaService(McpaCommercialDbContext context)
    {
      _context = context;
    }

    public IEnumerable<Documents> GetDocuments()
    {
      var result = _context.Documents.Take(100).ToList();

      return result;
    }
  }
}