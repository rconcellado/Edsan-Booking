using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    public class ReservationViewModel
    {
        public string ReservationId { get; set; }
        public string Location { get; set; }

        public string GuestId { get; set; }

        public string Company { get; set; }

        public string FirstName { get; set; } // Assuming you'd want to display the guest's name instead of ID

        public string LastName { get; set; } // Assuming you'd want to display the guest's name instead of ID

        public string TypeName { get; set; }

        public string FeatureName { get; set; }

        public int HourType { get; set; }

        public int NumGuest { get; set; }

        public string PkgDescr { get; set; }

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

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string GuestType { get; set; }

        //public string featureName { get; set; }
        //public string typeName { get; set; }    
        //public int hourType { get; set; }
        //public string PkgName { get; set; }
    }
}
