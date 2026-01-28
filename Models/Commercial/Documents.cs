using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace McpaApi.Models.Commercial
{
  [Table("admDocumentos")]
  public class Documents
  {
    [Key]
    [Column("CIDDOCUMENTO")]
    public required int Id { get; set; }
    [Column("CIDDOCUMENTODE")]
    public required int IdDocumentoDe { get; set; }
    [Column("CIDCONCEPTODOCUMENTO")]
    public required int IdConceptoDocumento { get; set; }
    [Column("CSERIEDOCUMENTO")]
    public required string SerieDocumento { get; set; }
    [Column("CIDCLIENTEPROVEEDOR")]
    public required int IdClientProveedor { get; set; } 
    [Column("CFOLIO")]
    public required double Folio { get; set; }
    [Column("CFECHA")]
    public required DateTime Fecha { get; set; }
    [Column("CRAZONSOCIAL")]
    public required string RazonSocial { get; set; }
    [Column("CRFC")]
    public required string RFC { get; set; }
    [Column("CREFERENCIA")]
    public required string Referencia { get; set; }
    [Column("COBSERVACIONES")]
    public string? Observaciones { get; set; }
    [Column("CIDDOCUMENTOORIGEN")]
    public required int IdDocumentoOrigen { get; set; }
    [Column("CCANCELADO")]
    public required int Cancelado { get; set; }
    [Column("CNETO")]
    public required double Neto { get; set; }
    [Column("CIMPUESTO1")]
    public required double Impuesto1 { get; set; }
    [Column("CTOTAL")]
    public required double Total { get; set; }
    [Column("CPENDIENTE")]
    public required double Pendiente { get; set; }
    [Column("CTOTALUNIDADES")]
    public required double TotalUnidades { get; set; }
    [Column("CTEXTOEXTRA1")]
    public required string TextoExtra1 { get; set; }
    [Column("CDESTINATARIO")]
    public required string Destinatario { get; set; }
    [Column("CUNIDADESPENDIENTES")]
    public required double UnidadesPendientes { get; set; }
    [Column("CGUIDDOCUMENTO")]
    public required string GuidDocumento { get; set; }
    [Column("CUSUARIO")]
    public required string Usuario { get; set; }
    [NotMapped]
    public required IEnumerable<Movements> Movements { get; set; }
    [NotMapped]
    [JsonIgnore]
    public required Clients Client { get; set; }
  }
}