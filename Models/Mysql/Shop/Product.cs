using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Shop;

namespace McpaApi.Models.Mysql.Shop
{
  [Table("product")]
  public class Product
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("adminpaq_id")]
    public required int AdminpaqId { get; set; }
    [Column("code")]
    public required string Code { get; set; }
    [Column("name")]
    public required string Name { get; set; }
    [Column("price")]
    public required double Price { get; set; }
    [NotMapped]
    public IEnumerable<SaleElement>? SaleElements { get; set; }
  }
}