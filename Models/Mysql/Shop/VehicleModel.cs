using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Shop;

namespace McpaApi.Models.Mysql.Shop
{
    [Table("vehicle_model")]
    public class VehicleModel
    {
        [Key]
        [Column("id")]
        public required int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [Column("brand_id")]
        public required int BrandId { get; set; }
        [NotMapped]
        public VehicleBrand? VehicleBrand { get; set; }
        [NotMapped]
        public IEnumerable<Sales>? Sales { get; set; }
    }
}