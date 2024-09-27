using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IRoomService
    {
        Task<int> GetTotalRoomCountAsync(string searchTerm = null, string loc = null);
        Task<List<Room>> GetRoomsAsync(int pageNumber, string searchTerm = null, string loc = null);
        Task<List<RoomAvailabilityGroupedViewModel>> GetRoomAvailabilityAsync(string location, DateTime checkInDate, DateTime checkOutDate);
        Task<RoomDetailsViewModel> GetRoomDetailsAsync(string featureName, string typeName, int hourType);
        Task<string> UploadRoomImagesAsync(string featureName, string typeName, IFormFile imageFile1, IFormFile imageFile2, IFormFile imageFile3, IFormFile imageFile4);
        Task<RoomImages> GetRoomImagesAsync(string featureName, string typeName);
        Task<Room> GetRoomByIdAsync(string id);
        Task<List<Room>> GetAllRoomsAsync();
        Task<Room> AddRoomAsync(Room room); // Update this method to return Room instead of bool
        Task<bool> DeleteRoomAsync(string id);
        Task<bool> UpdateRoomAsync(Room room);
        Task<bool> RoomExistsAsync(string id); // Add this method
        Task<string> GenerateNextRoomIdAsync();
    }
}
