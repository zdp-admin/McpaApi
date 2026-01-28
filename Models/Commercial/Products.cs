using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpaApi.Models.Commercial
{
  [Table("admProductos")]
  public class Products
  {
    [Key]
    [Column("CIDPRODUCTO")]
    public required int Id { get; set; }
    [Column("CCODIGOPRODUCTO")]
    public required string Code { get; set; }
    [Column("CNOMBREPRODUCTO")]
    public required string Name { get; set; }
    [Column("CIDVALORCLASIFICACION5")]
    public required int ClasificacionId { get; set; }
    [NotMapped]
    public required ClasificacionesValores ClasificacionesValores { get; set; }
    [NotMapped]
    public IEnumerable<Movements>? Movements { get; set; }
  }
}