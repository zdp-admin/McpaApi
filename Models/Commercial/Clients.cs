using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpaApi.Models.Commercial
{
    [Table("admClientes")]
    public class Clients
    {
        [Key]
        [Column("CIDCLIENTEPROVEEDOR")]
        public required int Id { get; set; }
        [Column("CDENCOMERCIAL")]
        public required string EnComercial { get; set; }
        [Column("CRFC")]
        public required string RFC { get; set; }
        [Column("CIDVALORCLASIFCLIENTE4")]
        public required int CIdValorClasifClient4 {get; set; }
        [NotMapped]
        public IEnumerable<Documents>? Documents { get; set; }
    }
}