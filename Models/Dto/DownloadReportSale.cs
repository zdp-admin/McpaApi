namespace McpaApi.Models.Dto
{
  public class DownloadReportSale
  {
    public required WebSite WebSite { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public int ByUser { get; set; }
  }
}