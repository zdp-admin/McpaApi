using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using McpaApi.Models.Shop;

namespace McpaApi.Models.Mysql.Shop
{
    [Table("vehicle_brand")]
    public class VehicleBrand
    {
        [Key]
        [Column("id")]
        public required int Id { get; set; }
        [Column("name")]
        public required string Name { get; set; }
        [NotMapped]
        public IEnumerable<VehicleModel>? VehicleModels { get; set; }
        [NotMapped]
        public IEnumerable<Sales>? Sales { get; set; }
    }
}