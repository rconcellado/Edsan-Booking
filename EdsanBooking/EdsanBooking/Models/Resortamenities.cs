using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    [Table("resortamenities")]
    public class Resortamenities
    {
        [Key]
        [Column("amenityid")]
        public string AmenityId { get; set; }
        [Column("pkgname")]
        public string PkgName { get; set; }
        [Column("amenity")]
        public string Amenity { get; set; }
    }
}
