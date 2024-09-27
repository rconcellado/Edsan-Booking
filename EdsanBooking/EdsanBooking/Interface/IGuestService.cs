using EdsanBooking.Models;

namespace EdsanBooking.Interface
{
    public interface IGuestService
    {
        Task<int> GetTotalGuestCountAsync(string searchTerm = null);
        Task<List<Guest>> GetGuestAsync(int pageNumber, string searchTerm = null);
        Task<Guest> GetGuestByIdAsync(string id);
        Task<List<Guest>> GetAllGuestAsync();
        Task<Guest> AddGuestAsync(Guest guest); // Update this method to return guest instead of bool
        Task<bool> DeleteGuestAsync(string id);
        Task<bool> UpdateGuestAsync(Guest guest);
        Task<bool> GuestExistByFirstNameLastName(string firstName, string lastName);
        Task<bool> GuestExistsAsync(string id); // Add this method
        Task<string> GenerateNextGuestIdAsync();
        Task<string> GetGuestIdByFirstNameLastNameAsync(string firstName, string lastName);
        Task SaveOrUpdateGuestAsync(string id, GuestDetailViewModel guestDetail);
        Task<string> SignupAsync(SignupDto signupDto);
        Task<string> GetGuestIdByUserNameAsync(string username);
    }
}
