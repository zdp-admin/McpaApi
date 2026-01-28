namespace McpaApi.Models.Dto.Output
{
    public class ReportSocialSale
    {
        public required string Seller { get; set; }
        public required double Amount { get; set; }
        public required WebSite Portal { get; set; }
    }
}