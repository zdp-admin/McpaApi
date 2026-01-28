using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace McpaApi.Models.Commercial
{
  [Table("admMovimientos")]
  public class Movements
  {
    [Key]
    [Column("CIDMOVIMIENTO")]
    public required int Id { get; set; }
    [Column("CIDDOCUMENTO")]
    public required int IdDocumento { get; set; }
    [Column("CNUMEROMOVIMIENTO")]
    public required double NumeroMovimiento { get; set; }
    [Column("CIDDOCUMENTODE")]
    public required int IdDocumentoDe { get; set; }
    [Column("CIDPRODUCTO")]
    public required int IdProducto { get; set; }
    [Column("CUNIDADES")]
    public required double Unidades { get; set; }
    [Column("CPRECIO")]
    public required double Precio { get; set; }
    [Column("CNETO")]
    public required double Neto { get; set; }
    [Column("CTOTAL")]
    public required double Total { get; set; }
    [Column("CFECHA")]
    public required DateTime Fecha { get; set; }
    [NotMapped]
    [JsonIgnore]
    public required Documents Document { get; set; }
    [NotMapped]
    [JsonIgnore]
    public required Products Product { get; set; }
  }
}