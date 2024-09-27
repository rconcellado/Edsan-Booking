namespace EdsanBooking.Models
{
    public class RoomAvailabilityGroupedViewModel
    {
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public string ImagePath1 { get; set; } // Add this property to hold the image path
        public List<RoomAvailabilityViewModel> Rooms { get; set; }
    }

    public class RoomAvailabilityViewModel
    {
        public int HourType { get; set; }
        public decimal RoomRates { get; set; }
        public int AvailableRooms { get; set; }
    }
}
