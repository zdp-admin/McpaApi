using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Mysql.Shop;

namespace McpaApi.Models.Shop
{
  [Table("sales_elements")]
  public class SaleElement
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("sale_id")]
    public required int SaleId { get; set; }
    [Column("product_id")]
    public required int ProductId { get; set; }
    [Column("service_commission_id")]
    public required int ServiceCommissionId { get; set; }
    [Column("quantity")]
    public required int Quantity { get; set; }
    [Column("price")]
    public required double Price { get; set; }
    [Column("service_commission")]
    public required double ServiceCommission { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
    [Column("unknown_product_name")]
    public string? UnknownProductName { get; set; }
    [NotMapped]
    public required Sales Sale { get; set; }
    [NotMapped]
    public required Product Product { get; set; }
    [NotMapped]
    public required ServiceCommissions ServiceCommissionCode { get; set; }
  }
}