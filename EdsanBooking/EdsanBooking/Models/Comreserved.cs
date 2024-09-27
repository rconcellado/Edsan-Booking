using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a reserved companion guest.
    /// </summary>
    [Table("comreserved")]
    public class Comreserved
    {
        [Key]
        [Column("comresid")]
        public string ComResId { get; set; }

        [Column("guestid")]
        public string GuestId { get; set; }

        [Column("reservationid")]
        public string ReservationId { get; set; }
        [Column("preference")]
        public string Preference { get; set; }
    }

}
