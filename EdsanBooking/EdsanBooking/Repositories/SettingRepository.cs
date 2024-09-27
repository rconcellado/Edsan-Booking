using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EdsanBooking.Repositories
{
    public class SettingRepository
    {
        private readonly BookingContext _context;
        public SettingRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<int> GetTotalRoomRatesCountAsync(string searchTerm = null)
        {
            var query = _context.RoomRates.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.featureName.Contains(searchTerm) ||
                                         r.typeName.Contains(searchTerm));
            }

            return await query.CountAsync();
        }
        public async Task<List<RoomRates>> GetRoomRates(int skip, int take, string location, string searchTerm = null)
        {
            var query = _context.RoomRates
                        .Where(r => r.Location == location)
                        .AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.featureName.Contains(searchTerm) ||
                                         r.typeName.Contains(searchTerm));
                                   
            }

            return await query.OrderBy(r => r.featureName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<RoomRates> GetRoomRatesByIdAsync(string id)
        {
            return await _context.RoomRates
                .FirstOrDefaultAsync(r => r.ID == id);
        }
        public async Task<RoomRates> UpdateRoomRateAsync(RoomRates roomRate)
        {
            _context.RoomRates.Update(roomRate);
            await _context.SaveChangesAsync();
            return roomRate;
        }


    }
}
