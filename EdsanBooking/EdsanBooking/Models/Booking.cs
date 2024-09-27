// Models/Booking.cs
namespace EdsanBooking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string HouseName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
    }

    // Models/House.cs
    public class House
    {
        public int HouseId { get; set; }
        public string HouseName { get; set; }
        public string Location { get; set; }
        public bool IsOccupied { get; set; }
        public decimal RatePerNight { get; set; }
    }

    // Models/Revenue.cs
    public class Revenue
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}

