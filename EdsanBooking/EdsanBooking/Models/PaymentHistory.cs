using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdsanBooking.Models
{
    [Table("paymenthistory")]
    public class PaymentHistory
    {
        [Key]
        [Column("transactionid")]
        public string TransactionId { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("transactiondate")]
        public DateTime TransactionDate { get; set; }
        [Column("reservationid")]
        public string ReservationId { get; set; }
        [Column("checkinid")]
        public string CheckInId { get; set; }
        [Column("transactiontype")]
        public string TransactionType { get; set; }
    }
}
