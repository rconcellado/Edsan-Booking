using System.Collections.Generic;
using System.Threading.Tasks;
using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EdsanBooking.Repositories
{
    public class GuestRepository
    {
        private readonly BookingContext _context;
        public GuestRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<int> GetTotalGuestCountAsync(string searchTerm = null)
        {
            var query = _context.Guest.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.GuestId.Contains(searchTerm) || 
                                         r.GName.Contains(searchTerm) ||
                                         r.LName.Contains(searchTerm) ||
                                         r.Company.Contains(searchTerm));
            }
            
            return await query.CountAsync();
        }
        public async Task<List<Guest>> GetGuestAsync(int skip, int take, string searchTerm = null)
        {
            var query = _context.Guest
                        .Where(r => !r.GuestId.StartsWith("GUE00000"))
                        .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.GuestId.Contains(searchTerm) ||
                                         r.GName.Contains(searchTerm) ||
                                         r.LName.Contains(searchTerm) ||
                                         r.Company.Contains(searchTerm));
            }

            return await query.OrderBy(r => r.GuestId)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
        }
        public async Task<Guest> GetGuestByIdAsync(string id)
        {
            return await _context.Guest
                .FirstOrDefaultAsync(r => r.GuestId == id);
        }
        public async Task<Guest> AddGuestAsync(Guest guest)
        {
            _context.Guest.Add(guest);
            await _context.SaveChangesAsync();
            return guest;
        }
        public async Task SaveOrUpdateGuestAsync(string id, GuestDetailViewModel guestDetail)
        {
            try
            {
                // Attempt to find the guest by first and last name, or create a new one if not found
                var guest = await _context.Guest
                    .FirstOrDefaultAsync(g => g.GName == guestDetail.FirstName && g.LName == guestDetail.LastName);

                var newId = await GenerateNextGuestIdAsync();

                if (guest == null) {
                    var newGuest = new Guest
                    {
                        GuestId = newId,
                        GName = guestDetail.FirstName,
                        LName = guestDetail.LastName,
                        ContactNo = "N/A",
                        Address = "N/A",
                        Company = "Not applicable",
                        GuestType = "Individual"
                    };
                    _context.Guest.Add(newGuest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    newId = guest.GuestId;
                }
                   

                // Determine if the operation is for a reservation or a check-in and save accordingly
                if (id.StartsWith("RES"))
                {
                    var newcomReserved = new Comreserved
                    {
                        ComResId = IdGenerator.GenerateNextId(_context.Comreserved, "CRD", r => r.ComResId),
                        GuestId = newId,
                        ReservationId = id,
                        Preference = guestDetail.Preference
                    };
                    _context.Comreserved.Add(newcomReserved);
                }
                else
                {
                    var newRoomCheckIn = new Roomcheckin
                    {
                        RoomCheckInId = IdGenerator.GenerateNextId(_context.RoomcheckIn, "RCN", r => r.RoomCheckInId),
                        GuestId = newId,
                        CheckInId = id,
                        RoomId = guestDetail.RoomId
                    };
                    _context.RoomcheckIn.Add(newRoomCheckIn);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"An error occurred while saving the guest: {ex.Message}", ex);
            }
        }
        public async Task UpdateGuestAsync(Guest guest)
        {
            _context.Guest.Update(guest);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteGuestAsync(Guest guest)
        {
            _context.Guest.Remove(guest);
            await _context.SaveChangesAsync();
        }
        public bool GuestExist(string id)
        {
            return _context.Guest.Any(e => e.GuestId == id);
        }
        public async Task<string> GetGuestIdByFirstNameLastName(string firstName, string lastName)
        {
            var guest = await _context.Guest.FirstOrDefaultAsync(e => e.GName == firstName && e.LName == lastName);

            if (guest != null)
            {
                return guest.GuestId;
            }

            // Return null or an appropriate value if no guest is found
            return null;
        }

        public async Task<bool> CheckGuestExistByFirstNameLastName(string firstName, string lastName) {
            return await _context.Guest.AnyAsync(e => e.GName == firstName && e.LName == lastName);
        }
        public async Task<IEnumerable<GuestViewModel>> SearchGuestsAsync(string searchTerm)
        {
            return await _context.Guest
                .Where(g => g.GName.Contains(searchTerm) || g.LName.Contains(searchTerm) || 
                            g.GuestId.Contains(searchTerm) || g.Company.Contains(searchTerm))
                .Select(g => new GuestViewModel
                {
                    GuestId = g.GuestId,
                    FirstName = g.GName,
                    LastName = g.LName,
                    Company = g.Company
                })
                .ToListAsync();
        }
        public async Task<string> GenerateNextGuestIdAsync()
        {
            var lastGuest = await _context.Guest
                            .OrderByDescending(g => g.GuestId)
                            .FirstOrDefaultAsync();

            if(lastGuest == null)
            {
                return "GUE00001"; // Starting point if there are no records
            }

            int lastIdNumber = int.Parse(lastGuest.GuestId.Substring(3));
            return $"GUE{(lastIdNumber + 1).ToString("D5")}";
        }
        public async Task CreateGuestAsync(Guest guest)
        {
            _context.Guest.Add(guest);
            await _context.SaveChangesAsync();
        }
    }
}
