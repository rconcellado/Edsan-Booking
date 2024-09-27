namespace EdsanBooking.Models
{
    public class GuestViewModel
    {
        public string GuestId { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string GuestType { get; set; }

        public string RoomDescription { get; set; }  // Add this property
        public string Preference { get; set; }
    }
}
