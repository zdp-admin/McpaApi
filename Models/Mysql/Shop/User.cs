using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Shop;

namespace McpaApi.Models.Mysql.Shop
{
  [Table("users")]
  public class User
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("name")]
    public required string Name { get; set; }
    [Column("last_name")]
    public required string LastName { get; set; }
    [NotMapped]
    public IEnumerable<Sales>? Sales { get; set; }
  }
}