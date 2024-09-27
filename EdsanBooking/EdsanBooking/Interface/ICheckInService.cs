using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface ICheckInService
    {
        Task<int> GetTotalCheckInCountAsync(string searchTerm = null, string loc = null);
        Task<List<CheckInViewModel>> GetCheckInAsync(int pageNumber, string searchTerm = null, string loc = null);
        //Task<CheckInDetailsViewModel> GetCheckInDetailsAsync(string checkInId);
        Task<CheckIn> GetCheckInByIdAsync(string id);
        Task<List<GuestDetailViewModel>> GetGuestsByCheckInIdAsync(string checkInId);
        Task DeleteRoomCheckInRecordsAsync(string checkInId);
        Task<CheckIn> SaveOrUpdateCheckInAsync(CheckIn checkIn, string featureName, string typeName,
                                                 int hourType, string pkgName, bool isNew);
        Task<bool> UpdateRoomStatusAsync(string checkInId);
        Task<string> GenerateNextCheckInIdAsync();
        Task<bool> ConfirmCheckInAsync(string id);
        Task<bool> ConfirmCheckOutAsync(string id);
    }
}
