using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpaApi.Models.Shop
{
  [Table("invoice")]
  public class Invoice
  {
    [Key]
    [Column("id")]
    public required int Id { get; set; }
    [Column("adminpaq_id")]
    public required int AdminpaqId { get; set; }
    [Column("serie")]
    public required string Serie { get; set; }
    [Column("folio")]
    public required int Folio { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
    [NotMapped]
    public IEnumerable<Sales>? Sales { get; set; }
  }
}