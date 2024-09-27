using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IChargeService
    {
        Task<TransientRes> GetTransientDetailsByIdAsync(string id);
        Task<ResortRes> GetResortDetailsByIdAsync(string id);
        Task<decimal> GetTransientTotalAmountAsync(string id, string location);
        Task<decimal> GetResortTotalAmountAsync(string id, string location);
    }
}
