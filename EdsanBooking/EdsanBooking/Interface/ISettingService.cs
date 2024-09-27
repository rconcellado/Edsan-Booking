using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface ISettingService
    {
        Task<int> GetTotalRoomRatesCountAsync(string searchTerm = null);
        Task<List<RoomRates>> GetAllRoomRatesAsync(int pageNumber, int take, string loc, string searchTerm = null);
        Task<RoomRates> GetRoomRatesByIdAsync(string id);
        Task UpdateRoomRateAsync(RoomRates roomRates);
    }
}
