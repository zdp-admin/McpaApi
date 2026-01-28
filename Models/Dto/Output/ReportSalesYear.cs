namespace McpaApi.Models.Dto.Output
{
  public class SellerSalesWithOutAdditional
  {
    public required string Seller { get; set; }
    public required int Orders { get; set; }
    public required double TotalSales { get; set; }
    public required double AvgTicket { get; set; }
  }

  public class SellerSalesWithAdditional
  {
    public required string Seller { get; set; }
    public required int Orders { get; set; }
    public required double ExtraProducts { get; set; }
    public required double TotalAdditional { get; set; }
    public required double AvgTicket { get; set; }
    public required double PercentPenetration { get; set; }
  }

  public class ReportSalesYear
  {
    public required IEnumerable<SellerSalesWithOutAdditional> SalesWithoutAdditionals { get; set; }
    public required IEnumerable<SellerSalesWithAdditional> SalesWithAdditionals { get; set; }
    public required int Orders { get; set; }
    public required int OrdersWithAdditionals { get; set; }
    public required double PercentPenetrationTotal { get; set; }
    public required double TotalSalesWithAdditionals { get; set; }
    public required double TotalSalesWithoutAdditionals { get; set; }
  }
}