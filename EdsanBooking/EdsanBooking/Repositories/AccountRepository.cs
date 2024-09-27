using System.Collections.Generic;
using System.Threading.Tasks;
using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EdsanBooking.Repositories
{
    public class AccountRepository
    {
        private readonly BookingContext _context;
        public AccountRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateNextUserIdAsync()
        {
            var lastUser = await _context.Users
                            .OrderByDescending(g => g.UserId)
                            .FirstOrDefaultAsync();

            if (lastUser == null)
            {
                return "USR00001"; // Starting point if there are no records
            }

            int lastIdNumber = int.Parse(lastUser.UserId.Substring(3));
            return $"USR{(lastIdNumber + 1).ToString("D5")}";
        }
        public async Task CreateUserAsync(Users user)
        {
            user.CreatedDt = user.CreatedDt.ToUniversalTime();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<string> GetGuestIdByUserName(string username)
        {
            try
            {
                var users = await _context.Users.Where(r => r.UserName == username).FirstOrDefaultAsync();

                if (users != null)
                {
                    var userGuest = await _context.UserGuest.FirstOrDefaultAsync(r => r.UserId == users.UserId);
                    return userGuest?.GuestId;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional, depending on your logging setup)
                // Example: _logger.LogError(ex, "Error occurred while fetching GuestId by username.");

                // Return a specific value or rethrow the exception, depending on your error handling strategy
                return null; // or throw;
            }
        }
    }
}
