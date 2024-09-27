using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    public class RoomDetailsViewModel
    {
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public int HourType { get; set; }
        public decimal RoomRate { get; set; }
        public int AvailableRooms { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Contact Number is required")]
        public string ContactNo { get; set; }

        // Add these properties to hold image paths
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
    }
}
