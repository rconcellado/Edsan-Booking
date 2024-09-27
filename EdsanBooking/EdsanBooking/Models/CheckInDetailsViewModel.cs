namespace EdsanBooking.Models
{
    public class CheckInDetailsViewModel
    {
        public string CheckInId { get; set; }
        public string Location { get; set; }
        public string GuestId { get; set; }
        public string GuestName { get; set; }
        public string GuestProfilePictureUrl { get; set; }
        public string GuestDescription { get; set; }
        public int NumGuest { get; set; }
        public DateTime CheckInDt { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public DateTime CheckOutDt { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public string Remarks { get; set; }
        public string ReservationId { get; set; }
        public string CheckInType { get; set; }
        public string ReservationType { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string featureName { get; set; }
        public string typeName { get; set; }
        public string pkgName { get; set; }
        public int hourType { get; set; }

        // List of guest details
        public List<GuestViewModel> Guests { get; set; } = new List<GuestViewModel>();

        // New properties for payment and charges
        public decimal? TotalOutstanding { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<PaymentHistoryViewModel> Payments { get; set; }
        public List<ResortAmenitiesViewModel> resortAmenities { get; set; } = new List<ResortAmenitiesViewModel>();
    }

    public class PaymentHistoryViewModel
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
    }

    public class ResortAmenitiesViewModel
    {
        public string Amenity { get; set; }
    }

}