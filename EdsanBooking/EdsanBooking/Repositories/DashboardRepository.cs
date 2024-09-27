using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EdsanBooking.Repositories
{
    public class DashboardRepository
    {
        private readonly BookingContext _context; 
        public DashboardRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<int> GetBookingsSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            try
            {
                // Convert the input dates to UTC
                fromDate = fromDate.ToUniversalTime();
                toDate = toDate.ToUniversalTime();

                return await _context.CheckIn
                    .Where(c => c.Location == location
                                && c.Status != "CheckOut"
                                && c.Status != "Cancelled"
                                && c.Status != "Pending"
                                && c.CheckInDt.Date >= fromDate.Date
                                && c.CheckInDt.Date <= toDate.Date)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the booking sum: {ex.Message}", ex);
            }
        }
        public async Task<int> GetCheckOutSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            try
            {
                // Convert the input dates to UTC
                fromDate = fromDate.ToUniversalTime();
                toDate = toDate.ToUniversalTime();

                return await _context.CheckIn
                    .Where(c => c.Location == location
                                && c.Status == "CheckOut"
                                //&& c.Status != "Cancelled"
                                && c.CheckInDt.Date >= fromDate.Date
                                && c.CheckInDt.Date <= toDate.Date)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the booking sum: {ex.Message}", ex);
            }
        }
        public async Task<int> GetReservedSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            try
            {
                // Convert the input dates to UTC
                fromDate = fromDate.ToUniversalTime();
                toDate = toDate.ToUniversalTime();

                return await _context.Reservation
                    .Where(c => c.Location == location
                                //&& c.Status != "CheckOut"
                                && c.Status != "Cancelled"
                                && c.CheckInDt.Date >= fromDate.Date
                                && c.CheckInDt.Date <= toDate.Date)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the booking sum: {ex.Message}", ex);
            }
        }
        public async Task<int> GetGuestSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            try
            {
                // Convert the input dates to UTC
                fromDate = fromDate.ToUniversalTime();
                toDate = toDate.ToUniversalTime();

                // Fetch all relevant check-ins based on the filters
                var checkInIds = await _context.CheckIn
                    .Where(c => c.Location == location
                                && c.Status != "CheckOut"
                                && c.Status != "Cancelled"
                                && c.CheckInDt.Date >= fromDate.Date
                                && c.CheckInDt.Date <= toDate.Date)
                    .Select(c => c.CheckInId)
                    .ToListAsync();

                // Sum the guests from RoomcheckIn table based on the filtered check-in IDs
                var totalGuests = await _context.RoomcheckIn
                    .Where(r => checkInIds.Contains(r.CheckInId))
                    .CountAsync(r => r.GuestId != null); // Count only if GuestId is not null

                return totalGuests;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the guest sum: {ex.Message}", ex);
            }
        }
        public async Task<List<PaymentHistory>> GetPaymentsByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            fromDate = fromDate.ToUniversalTime();
            toDate = toDate.ToUniversalTime();

            var checkInIds = await _context.CheckIn
                .Where(c => c.CheckInDt >= fromDate && c.CheckInDt <= toDate && c.Location == location)
                .Select(c => c.CheckInId)
                .ToListAsync();

            return await _context.PaymentHistory
                .Where(p => checkInIds.Contains(p.CheckInId) && p.TransactionType == "PAY")
                .ToListAsync();
        }

        public async Task<int> GetRoomStatusAsync(string status, string location)
        {
            return await _context.Room
                .Where(r => r.StatusName == status && r.Location == location)
                .CountAsync();
        }
        public async Task<Dictionary<string, int>> GetRoomStatusCountsAsync(string location)
        {
            return await _context.Room
                .Where(r => r.Location == location)
                .GroupBy(r => r.StatusName)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.StatusName, g => g.Count);
        }
        




    }
}

