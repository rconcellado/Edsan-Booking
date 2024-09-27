using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    [Table("resortpkg")]
    public class ResortPKG
    {
        [Key]
        [Column("packageid")]
        public string PackageId { get; set; }
        [Column("descr")]
        public string Descr { get; set; }
        [Column("packageamt")]
        public decimal PackageAmt { get; set; }
        [Column("hourtype")]
        public int HourType { get; set; }
        [Column("numroom")]
        public int NumRoom { get; set; }
        [Column("loc")]
        public string Location { get; set; }
    }
}
