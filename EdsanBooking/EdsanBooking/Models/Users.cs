using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("userid")]
        public string UserId { get; set; }

        [Column("username")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("loc")]
        public string Location { get; set; }

        [Column("failedloginattempts")]
        public int FailedLoginAttempts { get; set; }

        [Column("islocked")]
        public bool IsLocked { get; set; }

        [Column("status")]
        public string Status { get; set; }
        [Column("secquest")]
        public string SecurityQuestion { get; set; }
        [Column("secanswer")]
        public string SecurityAnswer { get; set; }
        [Column("userrole")]
        public string UserRole { get; set; }

        [Column("createdby")]
        public string CreatedBy { get; set; }

        [Column("createddt")]
        public DateTime CreatedDt { get; set; }
    }
}
