using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{

    [Table("transientres")]
    public class TransientRes
    {
        [Key]
        [Column("trresid")]
        public string TrResId { get; set; }
        [Column("featurename")]
        public string FeatureName { get; set; }
        [Column("typename")]
        public string TypeName { get; set; }
        [Column("hourtype")]
        public int HourType { get; set; }
        [Column("reservationid")]
        public string ReservationId { get; set; }
        [Column("checkinid")]
        public string CheckInId {  get; set; }


    }
}
