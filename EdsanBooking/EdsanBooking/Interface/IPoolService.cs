using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IPoolService
    {
        Task<CheckInDetailsViewModel> GetDetailsAsync(string id);
    }
}
