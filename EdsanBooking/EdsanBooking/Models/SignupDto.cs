using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    public class SignupDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Phone]
        public string ContactNo { get; set; }
        //public string Address { get; set; }
        //[Required]
        //public string Username { get; set; }
        [Required]
        public string Location { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }
}
