using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Mysql.Shop;

namespace McpaApi.Models.Shop
{
  [Table("sales")]
  public class Sales
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("client_id")]
    public int? ClientId { get; set; }
    [Column("client_name")]
    public required string ClientName { get; set; }
    [Column("vehicle_brand")]
    public required string VehicleBrand { get; set; }
    [Column("vehicle_model")]
    public required string VehicleModel { get; set; }
    [Column("vehicle_serie")]
    public required string VehicleSerie { get; set; }
    [Column("vehicle_plates")]
    public string? VehiclePlates { get; set; }
    [Column("vehicle_color")]
    public string? VehicleColor { get; set; }
    [Column("vehicle_year")]
    public int? VehicleYear { get; set; }
    [Column("total")]
    public required double Total { get; set; }
    [Column("by_user_id")]
    public required int ByUserId { get; set; }
    [Column("invoice_id")]
    public int? InvoiceId { get; set; }
    [Column("status")]
    public required int Status { get; set; }
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }
    [Column("email")]
    public string? Email { get; set; }
    [Column("telephone")]
    public string? Telephone { get; set; }
    [Column("birthday")]
    public DateTime? Birthday { get; set; }
    [Column("parent_id")]
    public int? ParentId { get; set; }
    [Column("note")]
    public required string Note { get; set; }
    [Column("is_cotization")]
    public required byte IsCotization { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
    [NotMapped]
    public Invoice? Invoice { get; set; }
    [NotMapped]
    public required IEnumerable<SaleElement> SaleElements { get; set; }
    [NotMapped]
    public required User User { get; set; }
    [NotMapped]
    public Agency? Agency { get; set; }
    [NotMapped]
    public int VehicleBrandInt => int.TryParse(VehicleBrand, out var result) ? result : 0;
    [NotMapped]
    public int VehicleModelInt => int.TryParse(VehicleModel, out var result) ? result : 0;
    [NotMapped]
    public VehicleBrand? BrandVehicle { get; set; }
    [NotMapped]
    public VehicleModel? ModelVehicle { get; set; }
  }
}