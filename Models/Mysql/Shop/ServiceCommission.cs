using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpaApi.Models.Shop
{
    [Table("service_commissions")]
    public class ServiceCommissions
    {
        [Key]
        [Column("id")]
        public required int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("code")]
        public string? Code { get; set; }
        [NotMapped]
        public IEnumerable<SaleElement>? SaleElements { get; set; }
    }
}