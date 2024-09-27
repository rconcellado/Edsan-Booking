using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdsanBooking.Repositories
{
    public class CheckInRepository
    {
        private readonly BookingContext _context;
        private readonly IGuestService _guestService;
        private readonly ChargeRepository _chargeRepository;
        public CheckInRepository(BookingContext context, IGuestService guestService, ChargeRepository chargeRepository)
        {
            _context = context;
            _guestService = guestService;
            _chargeRepository = chargeRepository;
        }
        public async Task<int> GetTotalCheckInCountAsync(string searchTerm = null, string loc = null)
        {
            var query = _context.CheckIn.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.CheckInId.Contains(searchTerm) ||
                                        r.GuestId.Contains(searchTerm) ||
                                        r.Status.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(loc))
            {
                query = query.Where(r => r.Location == loc);
            }

            return await query.CountAsync();
        }
        public async Task<List<CheckInViewModel>> GetCheckInAsync(int skip, int take, string searchTerm = null, string loc = null)
        {
            var query = from res in _context.CheckIn
                        join guest in _context.Guest
                        on res.GuestId equals guest.GuestId
                        where (loc == null || res.Location == loc) &&
                              res.Status != "CheckOut" &&
                              res.Status != "Cancelled"
                        select new CheckInViewModel
                        {
                            CheckInId = res.CheckInId,
                            GuestId = res.GuestId,
                            FirstName = guest.GName,
                            LastName = guest.LName,
                            Company = guest.Company,
                            NumGuest = res.NumGuest,
                            CheckInDt = res.CheckInDt,
                            CheckInTime = res.CheckInTime,
                            CheckOutDt = res.CheckOutDt,
                            CheckOutTime = res.CheckOutTime,
                            Status = res.Status,
                            Remarks = res.Remarks,
                            CheckInType = res.CheckInType,
                            CreatedBy = res.CreatedBy,
                            CreatedDt = res.CreatedDt,
                            Location = res.Location,
                            GuestType = guest.GuestType
                        };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.CheckInId.Contains(searchTerm) ||
                                         r.GuestId.Contains(searchTerm) ||
                                         r.Status.Contains(searchTerm) ||
                                         (r.FirstName + " " + r.LastName).Contains(searchTerm) ||
                                         r.Company.Contains(searchTerm));
            }

            return await query.OrderBy(r => r.CheckInId)
                              .Skip(skip)
                              .Take(take)
                              .ToListAsync();
        }
        public async Task<CheckIn> GetCheckInByIdAsync(string id)
        {
            return await _context.CheckIn.FindAsync(id);
        }
        public async Task<CheckIn> SaveOrUpdateCheckInAsync(CheckIn checkIn,
                                                    string featureName, string typeName, int hourType, string pkgName,
                                                    bool isNew)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (isNew)
                    {
                        checkIn.CreatedDt = DateTime.UtcNow;
                        _context.CheckIn.Add(checkIn);
                    }
                    else
                    {
                        // Ensure the check-in entity is tracked by the context and marked as modified
                        _context.Attach(checkIn);
                        _context.Entry(checkIn).State = EntityState.Modified;
                    }

                    checkIn.CheckInDt = checkIn.CheckInDt.ToUniversalTime();
                    checkIn.CheckOutDt = checkIn.CheckOutDt.ToUniversalTime();
                    checkIn.CreatedDt = checkIn.CreatedDt.ToUniversalTime();

                    await _context.SaveChangesAsync();

                    await _chargeRepository.SaveOrUpdateSpace(checkIn.CheckInId, checkIn.CheckInType,
                                                              featureName, typeName, hourType, pkgName);

                    await transaction.CommitAsync();
                    return checkIn;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"An error occurred while saving or updating the check-in: {ex.Message}");
                }
            }
        }
        public bool CheckInExists(string id)
        {
            return _context.CheckIn.Any(e => e.CheckInId == id);
        }
        public async Task<string> GenerateNextCheckInIdAsync()
        {
            var lastCheckIn = await _context.CheckIn
                    .OrderByDescending(r => r.CheckInId)
                    .FirstOrDefaultAsync();

            if (lastCheckIn == null)
            {
                return "CHK0000001";
            }

            int lastIdNumber = int.Parse(lastCheckIn.CheckInId.Substring(3));
            return $"CHK{(lastIdNumber + 1).ToString("D7")}";
        }
        public async Task DeleteRoomCheckInRecordsAsync(string checkInId)
        {
            // Retrieve the list of RoomcheckIn records associated with the CheckInId
            var roomCheckIns = await _context.RoomcheckIn
                .Where(r => r.CheckInId == checkInId)
                .ToListAsync();

            // Proceed only if there are records to delete
            if (roomCheckIns.Count > 0)
            {
                // Delete the RoomcheckIn records
                _context.RemoveRange(roomCheckIns);
                await _context.SaveChangesAsync();

                // Get the location of the check-in
                var location = await _context.CheckIn
                    .Where(r => r.CheckInId == checkInId)
                    .Select(r => r.Location)
                    .FirstOrDefaultAsync();

                // Proceed only if the location is found
                if (!string.IsNullOrEmpty(location))
                {
                    // Get the list of RoomIds that were checked in
                    var roomIds = roomCheckIns.Select(rc => rc.RoomId).ToList();

                    // Update the StatusName of the corresponding rooms to "Available"
                    var roomsToUpdate = await _context.Room
                        .Where(r => roomIds.Contains(r.RoomId) && r.Location == location)
                        .ToListAsync();

                    foreach (var room in roomsToUpdate)
                    {
                        room.StatusName = "Available";
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task UpdateRoomStatusAsync(string checkinId)
        {
            // Call the method to get the guest details by CheckInId
            var guestDetailsList = await GetGuestsByCheckInIdAsync(checkinId);

            // Initialize a HashSet to store distinct room IDs (HashSet is used to avoid duplicates more efficiently)
            var roomIds = new HashSet<string>();

            // Loop through the guest details and add each RoomId to the HashSet
            foreach (var guestDetail in guestDetailsList)
            {
                roomIds.Add(guestDetail.RoomId);
            }

            // Now you can process the roomIds HashSet as needed
            foreach (var roomId in roomIds)
            {
                var room = await _context.Room.FirstOrDefaultAsync(r => r.RoomId == roomId);

                if (room != null)
                {
                    room.StatusName = "Occupied";
                }
            }

            // Save all changes in one go for better performance
            await _context.SaveChangesAsync();
        }
        public async Task ConfirmCheckInAsync(string id)
        {
            try
            {
                var roomCount = await _context.RoomcheckIn
                    .Where(r => r.CheckInId == id)
                    .CountAsync();

                var checkIn = await _context.CheckIn.FindAsync(id);
                if (checkIn != null)
                {
                    if(checkIn.CheckInType == "Transient")
                    {
                        if (roomCount == checkIn.NumGuest)
                        {
                            checkIn.Status = "CheckIn";

                            checkIn.CheckInDt = checkIn.CheckInDt.ToUniversalTime();
                            checkIn.CheckOutDt = checkIn.CheckOutDt.ToUniversalTime();
                            checkIn.CreatedDt = checkIn.CreatedDt.ToUniversalTime();

                            _context.Update(checkIn);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception("The number of checked-in rooms does not match the number of guests.");
                        }
                    }
                }
                else
                {
                    throw new Exception("CheckIn record not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if you have a logging mechanism)
                // throw or handle the exception as needed
                throw new Exception($"An error occurred while confirming the check-in: {ex.Message}", ex);
            }
        }
        public async Task ConfirmCheckOutAsync(string id)
        {
            try
            {
                // Sum amounts by transaction types
                var paymentSums = await _context.PaymentHistory
                    .Where(r => r.CheckInId == id &&
                                (r.TransactionType == "PAY" ||
                                 r.TransactionType == "DSC" ||
                                 r.TransactionType == "RMCHG" ||
                                 r.TransactionType == "EXC"))
                    .GroupBy(r => r.TransactionType)
                    .Select(g => new
                    {
                        TransactionType = g.Key,
                        TotalAmount = g.Sum(r => r.Amount)
                    })
                    .ToListAsync();

                // Retrieve totals
                var totalPayAmount = paymentSums
                    .FirstOrDefault(p => p.TransactionType == "PAY")?.TotalAmount ?? 0;

                var totalDiscountAmount = paymentSums
                    .FirstOrDefault(p => p.TransactionType == "DSC")?.TotalAmount ?? 0;

                var totalRoomChargeAmount = paymentSums
                    .FirstOrDefault(p => p.TransactionType == "RMCHG")?.TotalAmount ?? 0;

                var totalExcessAmount = paymentSums
                    .FirstOrDefault(p => p.TransactionType == "EXC")?.TotalAmount ?? 0;

                // Check if room charges plus excess are covered by payments and discounts
                if (totalRoomChargeAmount + totalExcessAmount <= (totalPayAmount + totalDiscountAmount))
                {
                    var checkIn = await _context.CheckIn.FindAsync(id);
                    if (checkIn != null)
                    {
                        checkIn.Status = "CheckOut";

                        // Convert dates to UTC
                        checkIn.CheckInDt = checkIn.CheckInDt.ToUniversalTime();
                        checkIn.CheckOutDt = checkIn.CheckOutDt.ToUniversalTime();
                        checkIn.CreatedDt = checkIn.CreatedDt.ToUniversalTime();

                        // Update the check-in status
                        _context.Update(checkIn);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (assuming you have a logger, otherwise handle accordingly)
                // e.g., _logger.LogError(ex, "An error occurred while confirming the check-out.");

                // Throw a new exception or handle it as required
                throw new Exception($"An error occurred while confirming the check-out: {ex.Message}", ex);
            }
        }

        public async Task<List<GuestDetailViewModel>> GetGuestsByCheckInIdAsync(string checkInId)
        {
            // Retrieve all room check-ins associated with the given check-in ID
            var roomCheckIns = await _context.RoomcheckIn
                .Where(rc => rc.CheckInId == checkInId)
                .ToListAsync();

            // Join RoomcheckIn with Guest to get the details
            var guestDetails = await (from rc in _context.RoomcheckIn
                                      join g in _context.Guest on rc.GuestId equals g.GuestId
                                      where rc.CheckInId == checkInId
                                      select new GuestDetailViewModel
                                      {
                                          FirstName = g.GName,
                                          LastName = g.LName,
                                          RoomId = rc.RoomId
                                      }).ToListAsync();

            return guestDetails;
        }
        


    }
}
