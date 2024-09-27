using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace EdsanBooking.Models
{
    [Table("roomrate")]
    public class RoomRates
    {
        [Key]
        [Column("id")]
        public string ID { get; set; }
        [Column("featurename")]
        public string featureName { get; set; }
        [Column("typename")]
        public string typeName {  get; set; }
        [Column("hourtype")]
        public int hourType { get; set; }
        [Column("roomrate")]
        public decimal RoomRate {  get; set; }
        [Column("loc")]
        public string Location { get; set; }
        [Column("classification")]
        public string Classification { get; set; }
    }
}
