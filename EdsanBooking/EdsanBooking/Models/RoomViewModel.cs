namespace EdsanBooking.Models
{
    public class RoomViewModel
    {
        public string RoomId { get; set; }
        public string Descr { get; set; }
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public int HourType { get; set; }
        //public decimal RoomRate { get; set; }
        public string StatusName { get; set; }
        public string Classification { get; set; } // Added Classification
        public string Remarks { get; set; }
        public string Location { get; set; }
    }

}
