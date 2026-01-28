namespace McpaApi.Models.Dto.Output
{
    public class ItemLealtad
    {
        public required string Name { get; set; }
        public string? Cellphone { get; set; }
        public required string Email { get; set; }
        public required string VehicleBrand { get; set; }
        public required string VehicleModel { get; set; }
        public required string VehicleVim { get; set; }
        public required string VehiclePlates { get; set; }
        public required int VehicleYear { get; set; }
        public required string Seller { get; set; }
        public required string Invoice { get; set; }
        public required double Amount { get; set; }
        public required string Concept { get; set; }
        public required string Sucursal { get; set; }
        public required string Category { get; set; }
        public string? Birthday { get; set; }
    }
}