namespace McpaApi.Models.Dto.Output
{
    public class DashboardReport
    {
        public double TotalSales { get; set; }
        public double TotalSalesWithoutInvoice { get; set; }
        public double PreviewTotalSales { get; set; }
        public double TotalSalesCurrentDay { get; set; }
        public double TotalSalesWithoutInvoiceCurrentDay { get; set; }
        public required IEnumerable<SellerReport> Sellers { get; set; }
        public required IEnumerable<ProductoReport> Products { get;set; }
        public double TotalNotasCredito { get; set; }
        public double AverageTotalSale { get; set; }
        public double AverageSales { get; set; }
    }
}