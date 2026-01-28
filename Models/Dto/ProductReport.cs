namespace McpaApi.Models.Dto
{
    public class ProductoReport
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public required int Quantity { get; set; }
        public required double TotalSale { get; set; }
        public required double TotalSaleCurrentDay { get; set; }
        public required int QuantityCurrentDay {get; set;}
    }
}