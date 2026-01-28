using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Shop;

namespace McpaApi.Models.Mysql.Shop
{
  [Table("agency")]
  public class Agency
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("adminpaq_id")]
    public required int AdminpaqId { get; set; }
    [Column("code")]
    public required string Code { get; set; }
    [Column("business_name")]
    public required string BusinessName { get; set; }
    [Column("rfc")]
    public required string RFC { get; set; }
    [NotMapped]
    public IEnumerable<Sales>? Sales { get; set; }
  }
}