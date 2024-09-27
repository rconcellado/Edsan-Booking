using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a check-in.
    /// </summary>
    [Table("checkin")]
    public class CheckIn
    {
        [Key]
        [Column("checkinid")]
        public string CheckInId { get; set; }
        [Column("loc")]
        public string Location { get; set; }

        [Column("guestid")]
        [Required(ErrorMessage = "Guest ID is required")]
        public string GuestId { get; set; }
        [Column("numguest")]
        [Required(ErrorMessage = "Number of Guests is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Guests must be at least 1")]
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

        [Column("remarks")]
        public string Remarks { get; set; }

        [Column("reservationid")]
        public string ReservationId { get; set; }

        [Column("checkintype")]
        [Required(ErrorMessage = "CheckIn Type is required")]
        [RegularExpression("^(Transient|Resort)$", ErrorMessage = "CheckIn Type must be either 'Transient' or 'Resort'")]
        public string CheckInType { get; set; }
        [Column("status")]
        public string Status { get; set; }

        [Column("createdby")]
        public string CreatedBy { get; set; }

        [Column("createddt")]
        public DateTime CreatedDt { get; set; }
    }
}
