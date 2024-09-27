using System.ComponentModel.DataAnnotations;

namespace EdsanBooking.Models
{
    public class PaymentViewModel
    {
        public string PayType { get; set; }
        public string ReservationId { get; set; }
        public string CheckInId { get; set; }
        public decimal ChargeAmount { get; set; }
        [Required]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }

        [Required]
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Total Amount")]
        //public decimal TotalAmount { get; set; }
        public string Location { get; set; }
        public decimal RemainingBalance { get; set; } // Add this property
        public decimal CurrentPayment { get; set; }
        public decimal ExcessPayment { get; set; }

        // Dynamic properties to get the appropriate ID
        public string DisplayId => CheckInId != "N/A" ? CheckInId : ReservationId;
        public string DisplayLabel => CheckInId != "N/A" ? "Check-In ID" : "Reservation ID";

    }

}
