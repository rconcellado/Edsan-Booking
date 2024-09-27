using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    [Table("userguest")]
    public class UserGuest
    {
        [Key]
        [Column("userid")]
        public string UserId { get; set; }
        [Column("guestid")]
        public string GuestId { get; set; }
    }
}
