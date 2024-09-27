namespace EdsanBooking.Models
{
    public class RoomRateViewModel
    {
        public string Id { get; set; }
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public int HourType { get; set; }
        public decimal RoomRate { get; set; }
        public string Classification {  get; set; }
    }
}
