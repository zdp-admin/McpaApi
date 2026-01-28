namespace McpaApi.Models.Dto.Output
{
  public class ReportSales
  {
    public required DateOnly Date { get; set; }
    public required DateOnly InvoiceDate { get; set; }
    public required string Sale { get; set; }
    public required string Invoice { get; set; }
    public required string Saller { get; set; }
    public required string ProductName { get; set; }
    public required string ProductCode { get; set; }
    public required double AmountWitOutVat { get; set; }
    public required string Client { get; set; }
    public required string RazonSocial { get; set; }
    public required string VehicleModel { get; set; }
    public required string VehicleBrand { get; set; }
    public required string VehicleColor { get; set; }
    public required string VehicleSerie { get; set; }
    public required string Observation { get; set; }
    public double AmountComercial { get; set; }
  }
}