using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a guest.
    /// </summary>
    [Table("guest")]
    public class Guest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Guest Id is required")]
        [Column("guestid")]
        public string GuestId { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("gname")]
        //[StringLength(50, ErrorMessage = "Guest firstname cannot be longer than 50 characters")]
        public string GName { get; set; }

        [Column("lname")]
        //[StringLength(50, ErrorMessage = "Guest lastname cannot be longer than 50 characters")]
        public string LName { get; set; }

        [Column("contactno")]
        [StringLength(15, ErrorMessage = "Contact number cannot be longer than 15 characters")]
        //[Required(ErrorMessage = "Contact number is required")]
        public string ContactNo { get; set; }
        [StringLength(150, ErrorMessage = "Address cannot be longer than 150 characters")]
        [Column("address")]
        public string Address { get; set; }

        [Column("guesttype")]
        public string GuestType { get; set; }
    }
}
