namespace McpaApi.Models.Dto
{
    public class SellerReport : SellerComission
    {
        public required int TotalSales { get; set; }
        public required int TotalSalesCurrentDay { get; set; }
        public required double ComissionCurrentDay {get;set;}
        public required double AverageTiket { get; set; }
    }
}