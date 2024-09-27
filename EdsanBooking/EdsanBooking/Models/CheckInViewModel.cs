namespace EdsanBooking.Models
{
    public class CheckInViewModel
    {
        public string ReservationId { get; set; }
        public string CheckInId { get; set; }
        public string Location { get; set; }
        public string GuestId { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; } // Assuming you'd want to display the guest's name instead of ID
        public string LastName { get; set; } // Assuming you'd want to display the guest's name instead of ID
        public string TypeName { get; set; }
        public string FeatureName { get; set; }
        public int HourType { get; set; }
        public int NumGuest { get; set; }
        public string PkgName { get; set; }
        public DateTime CheckInDt { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public DateTime CheckOutDt { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public decimal PaymentAmt { get; set; }
        public decimal DiscountAmt { get; set; }
        public decimal TotalAmt { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ReservationType { get; set; }
        public string CheckInType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string GuestType { get; set; }
        public bool bNew { get; set; }
    }
}
