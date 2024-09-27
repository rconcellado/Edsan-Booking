namespace EdsanBooking.Models
{
    public class RoomSearchViewModel
    {
        public string Location { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
    }
}
