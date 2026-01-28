using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpaApi.Models.Commercial
{
    [Table("admClasificacionesValores")]
    public class ClasificacionesValores
    {
        [Key]
        [Column("CIDVALORCLASIFICACION")]
        public required int Id { get; set; }
        [Column("CVALORCLASIFICACION")]
        public required string Name { get; set; }
        [NotMapped]
        public IEnumerable<Products>? Products { get; set; }
    }
}