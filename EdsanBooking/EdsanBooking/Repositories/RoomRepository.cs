using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EdsanBooking.Models;
using EdsanBooking.Data;

namespace EdsanBooking.Repositories
{
    public class RoomRepository
    {
        private readonly BookingContext _context;

        public RoomRepository(BookingContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalRoomCountAsync(string searchTerm = null, string loc = null)
        {
            var query = _context.Room.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.RoomId.Contains(searchTerm) ||
                                         r.Descr.Contains(searchTerm) ||
                                         r.FeatureName.Contains(searchTerm) ||
                                         r.TypeName.Contains(searchTerm));
            }

            // Filter by location if provided
            if (!string.IsNullOrEmpty(loc))
            {
                query = query.Where(r => r.Location == loc);
            }


            return await query.CountAsync();
        }
        /// <summary>
        /// Retrieves a list of rooms based on pagination and search term.
        /// </summary>
        public async Task<List<Room>> GetRoomsAsync(int skip, int take, string searchTerm = null, string loc = null)
        {
            var query = _context.Room
                        .Where(r => !r.RoomId.StartsWith("ROM00000")) // Exclude rooms with RoomId starting with "ROM00000"
                        .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.RoomId.Contains(searchTerm) ||
                                r.Descr.Contains(searchTerm) ||
                                r.FeatureName.Contains(searchTerm) ||
                                r.TypeName.Contains(searchTerm));
            }

            // Filter by location if provided
            if (!string.IsNullOrEmpty(loc))
            {
                query = query.Where(r => r.Location == loc);
            }

            return await query.OrderBy(r => r.RoomId)
                      .Skip(skip)
                      .Take(take)
                      .ToListAsync();
        }
        public async Task<List<RoomAvailabilityGroupedViewModel>> GetRoomAvailabilityAsync(string location, DateTime checkInDate, DateTime checkOutDate)
        {
            var roomsGrouped = await _context.Room
            .Where(r => r.Location == location)
            .GroupBy(r => new { r.FeatureName, r.TypeName })
            .Select(g => new
            {
                g.Key.FeatureName,
                g.Key.TypeName,
                HourTypes = g.Select(r => r.HourType).Distinct().ToList(),
                ImagePath1 = _context.RoomImages
                    .Where(ri => ri.FeatureName == g.Key.FeatureName && ri.TypeName == g.Key.TypeName)
                    .Select(ri => ri.ImagePath1)
                    .FirstOrDefault()
            })
            .ToListAsync();

            var groupedRoomList = new List<RoomAvailabilityGroupedViewModel>();

            foreach (var group in roomsGrouped)
            {
                var roomGroup = new RoomAvailabilityGroupedViewModel
                {
                    FeatureName = group.FeatureName,
                    TypeName = group.TypeName,
                    ImagePath1 = group.ImagePath1,
                    Rooms = new List<RoomAvailabilityViewModel>()
                };

                foreach (var hourType in group.HourTypes.OrderBy(h => h)) // Sort here by HourType
                {
                    var totalRooms = await _context.Room
                        .Where(r => r.FeatureName == group.FeatureName && r.TypeName == group.TypeName && r.HourType == hourType)
                        .CountAsync();

                    var reservedRoomsCount = await _context.TransientRes
                        .Where(t => t.FeatureName == group.FeatureName && t.TypeName == group.TypeName && t.HourType == hourType)
                        .Join(_context.Comreserved, t => t.ReservationId, cr => cr.ReservationId, (t, cr) => new { t.ReservationId, cr.GuestId })
                        .Join(_context.Reservation, tr => tr.ReservationId, r => r.ReservationId, (tr, r) => r)
                        .CountAsync(r => r.CheckInDt < checkOutDate && r.CheckOutDt > checkInDate);

                    var availableRoomCount = totalRooms - reservedRoomsCount;

                    var roomRate = await _context.RoomRates
                        .Where(r => r.featureName == group.FeatureName && r.typeName == group.TypeName && r.hourType == hourType)
                        .Select(r => r.RoomRate)
                        .FirstOrDefaultAsync();

                    if (availableRoomCount > 0)
                    {
                        roomGroup.Rooms.Add(new RoomAvailabilityViewModel
                        {
                            HourType = hourType,
                            AvailableRooms = availableRoomCount,
                            RoomRates = roomRate
                        });
                    }
                }

                if (roomGroup.Rooms.Any())
                {
                    groupedRoomList.Add(roomGroup);
                }
            }

            return groupedRoomList;
        }
        public async Task<RoomDetailsViewModel> GetRoomDetailsAsync(string featureName, string typeName, int hourType)
        {
            // Fetch room rate from the RoomRates table based on featureName, typeName, and hourType
            var roomRate = await _context.RoomRates
                .Where(r => r.featureName == featureName && r.typeName == typeName && r.hourType == hourType)
                .Select(r => r.RoomRate)
                .FirstOrDefaultAsync();

            // Fetch room details from the Rooms table based on featureName, typeName, and hourType
            var room = await _context.Room
                .Where(r => r.FeatureName == featureName && r.TypeName == typeName && r.HourType == hourType)
                .Select(r => new RoomDetailsViewModel
                {
                    FeatureName = r.FeatureName,
                    TypeName = r.TypeName,
                    HourType = r.HourType,
                    RoomRate = roomRate, // Room rate fetched from RoomRates table
                    //AvailableRooms = r.TotalRooms - r.ReservedRooms, // Assuming you calculate available rooms
                    Description = r.Descr // Assuming you have a description for the room
                })
                .FirstOrDefaultAsync();

            // Handle case where room details are not found
            if (room == null)
            {
                return null;
            }

            return room;
        }
        public async Task SaveRoomImagesAsync(RoomImages roomImage)
        {
            _context.RoomImages.Add(roomImage);
            await _context.SaveChangesAsync();
        }
        public async Task<RoomImages> GetRoomImagesAsync(string featureName, string typeName)
        {
            return await _context.RoomImages
                .FirstOrDefaultAsync(ri => ri.FeatureName == featureName && ri.TypeName == typeName);
        }
        public async Task<Room> GetRoomByIdAsync(string id)
        {
            return await _context.Room
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<Room> AddRoomAsync(Room room)
        {
            _context.Room.Add(room);
            await _context.SaveChangesAsync();
            return room; // This will now include the generated RoomId
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Room.Update(room);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteRoomAsync(Room room)
        {
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
        }
        public bool RoomExists(string id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
        public async Task<string> GenerateNextRoomIdAsync()
        {
            var lastRoom = await _context.Room
                .OrderByDescending(r => r.RoomId)
                .FirstOrDefaultAsync();

            if(lastRoom == null)
            {
                return "ROM00001";
            }

            int lastIdNumber = int.Parse(lastRoom.RoomId.Substring(3));
            return $"ROM{(lastIdNumber + 1).ToString("D5")}";
        }
    }
}
