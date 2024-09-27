using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    public class AddGuestsViewModel
    {
        //[Required]
        public string ReservationId { get; set; }

        public string CheckInId { get; set; }

        public int NumGuests { get; set; }

        public List<GuestDetailViewModel> GuestDetails { get; set; } = new List<GuestDetailViewModel>();

        public string DisplayId => !string.IsNullOrEmpty(CheckInId) ? CheckInId : ReservationId;

        public string DisplayLabel => !string.IsNullOrEmpty(CheckInId) ? "Check-In ID" : "Reservation ID";
    }

    public class GuestDetailViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        public string Preference { get; set; }  // Add validation if needed

        public string RoomId { get; set; }
    }
}
