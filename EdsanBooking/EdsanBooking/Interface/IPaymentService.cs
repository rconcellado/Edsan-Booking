using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(string id, decimal chargeAmount, decimal paymentAmount,
                             decimal discountAmount, decimal excessAmount);
        Task<decimal> GetTotalPaymentsMadeAsync(string id);
        Task AddRoomChargeAsync(string id);
    }
}
