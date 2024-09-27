using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a reservation.
    /// </summary>
    [Table("reservation")]
    public class Reservation
    {
        [Key]
        [Column("reservationid")]
        public string ReservationId { get; set; }
        [Column("loc")]
        public string Location { get; set; }

        [Column("guestid")]
        public string GuestId { get; set; }

        [Column("numguest")]
        public int NumGuest { get; set; }

        [Column("checkindt")]
        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDt { get; set; }

        [Column("checkintime")]
        public TimeSpan CheckInTime { get; set; }

        [Column("checkoutdt")]
        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDt { get; set; }

        [Column("checkouttime")]
        public TimeSpan CheckOutTime { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("reservationtype")]
        public string ReservationType { get; set; }

        [Column("createdby")]
        public string CreatedBy { get; set; }

        [Column("createddt")]
        public DateTime CreatedDt { get; set; }
    }
}
