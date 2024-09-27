namespace EdsanBooking.Models
{
    public class PaymentRequestModel
    {
        public string Id { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ExcessAmount { get; set; }

    }
}
