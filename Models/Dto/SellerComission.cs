namespace McpaApi.Models.Dto
{
    public class SellerComission
    {
        public required int SellerId { get; set; }
        public required string SellerName { get; set; }
        public required double Comission { get; set; }
    }
}