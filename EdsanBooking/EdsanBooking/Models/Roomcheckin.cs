using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    [Table("roomcheckin")]
    public class Roomcheckin
    {
        [Key]
        [Column("roomcheckinid")]
        public string RoomCheckInId { get; set; }
        [Column("checkinid")]
        public string CheckInId { get; set; }
        [Column("guestid")]
        public string GuestId { get; set; }
        [Column("roomid")]
        public string RoomId { get; set; }
    }
}
