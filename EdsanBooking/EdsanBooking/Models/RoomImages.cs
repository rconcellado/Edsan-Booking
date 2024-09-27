using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    [Table("roomimages")]
    public class RoomImages
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("featurename")]
        public string FeatureName { get; set; }
        [Column("typename")]
        public string TypeName { get; set; }
        [Column("imagepath1")]
        public string ImagePath1 { get; set; }
        [Column("imagepath2")]
        public string ImagePath2 { get; set; }
        [Column("imagepath3")]
        public string ImagePath3 { get; set; }
        [Column("imagepath4")]
        public string ImagePath4 { get; set; }
    }
}
