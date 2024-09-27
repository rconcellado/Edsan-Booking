using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    [Table("resortres")]
    public class ResortRes
    {
        [Key]
        [Column("resortresid")]
        public string ResortResId { get; set; }
        [Column("pkgname")]
        public string PkgName { get; set; }
        [Column("reservationid")]
        public string ReservationId { get; set; }
        [Column("checkinid")]
        public string CheckInId { get; set; }
    }
}
