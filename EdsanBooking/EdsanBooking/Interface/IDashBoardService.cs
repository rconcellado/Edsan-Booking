using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IDashBoardService
    {
        Task<int> GetBookingSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location);
        Task<int> GetCheckOutSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location);
        Task<int> GetReservedSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location);
        Task<int> GetGuestSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location);
        Task<List<RevenueData>> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate, string location);
        Task<int> GetRoomStatusCountAsync(string status, string location);  // Single argument method
        Task<Dictionary<string, int>> GetRoomStatusCountsAsync(string location);
    }
}
