using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Services;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TransientHouseManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashBoardService _dashBoardService;
        public DashboardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }
        public async Task<IActionResult> Summary(DateTime? fromDate, DateTime? toDate, string location)
        {
            fromDate ??= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1); // First day of the current month
            toDate ??= DateTime.UtcNow; // Current date

            if (string.IsNullOrEmpty(location))
            {
                location = HttpContext.Session.GetString("Location");
                ViewBag.Location = location;
            }

            var totalBookings = await _dashBoardService.GetBookingSumByDateRangeAsync(fromDate.Value, toDate.Value, location);

            var totalCheckOut = await _dashBoardService.GetCheckOutSumByDateRangeAsync(fromDate.Value, toDate.Value, location);

            var totalReserved = await _dashBoardService.GetReservedSumByDateRangeAsync(fromDate.Value, toDate.Value, location);

            var totalGuests = await _dashBoardService.GetGuestSumByDateRangeAsync(fromDate.Value, toDate.Value, location);

            var revenueData = await _dashBoardService.GetRevenueStatisticsAsync(fromDate.Value, toDate.Value, location);

            //var totalRevenue = revenueData.Sum(r => r.TotalRevenue);


            var availableRooms = await _dashBoardService.GetRoomStatusCountAsync("Available", location);
            var occupiedRooms = await _dashBoardService.GetRoomStatusCountAsync("Occupied", location);
            var maintenanceRooms = await _dashBoardService.GetRoomStatusCountAsync("Maintenance", location);

            var roomStatusCounts = await _dashBoardService.GetRoomStatusCountsAsync(location);

            var viewModel = new SummaryViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Location = location,
                TotalBookings = totalBookings,
                TotalCheckOut = totalCheckOut,
                TotalReservations = totalReserved,
                TotalGuests = totalGuests,
                RevenueStatistics = revenueData,
                AvailableRooms = availableRooms,
                OccupiedRooms = occupiedRooms,
                MaintenanceRooms = maintenanceRooms,
                RoomStatusCounts = roomStatusCounts,
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> FilterSummary(DateTime fromDate, DateTime toDate, string location)
        {
            // Assuming you have a service to get the summary data
            var totalBookings = await _dashBoardService.GetBookingSumByDateRangeAsync(fromDate, toDate, location);

            var totalCheckOut = await _dashBoardService.GetCheckOutSumByDateRangeAsync(fromDate, toDate, location);

            var totalReserved = await _dashBoardService.GetReservedSumByDateRangeAsync(fromDate, toDate, location);

            var totalGuests = await _dashBoardService.GetGuestSumByDateRangeAsync(fromDate, toDate, location);

            var revenueData = await _dashBoardService.GetRevenueStatisticsAsync(fromDate, toDate, location);

            var model = new SummaryViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Location = location,
                TotalBookings = totalBookings,
                TotalCheckOut = totalCheckOut,
                TotalReservations = totalReserved,
                TotalGuests = totalGuests,
                RevenueStatistics = revenueData
            };

            return View("Summary", model);
        }

       
    }
}
