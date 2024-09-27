using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Repositories;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        public DashboardApiController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        [HttpGet("sum-bookings")]
        public async Task<ActionResult<int>> GetBookingsSumByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string location)
        {
            try
            {
                var totalBookings = await _dashboardRepository.GetBookingsSumByDateRangeAsync(fromDate, toDate, location);
                return Ok(totalBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("sum-checkout")]
        public async Task<ActionResult<int>> GetCheckOutSumByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string location)
        {
            try
            {
                var totalBookings = await _dashboardRepository.GetCheckOutSumByDateRangeAsync(fromDate, toDate, location);
                return Ok(totalBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("sum-reserved")]
        public async Task<ActionResult<int>> GetReservedSumByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string location)
        {
            try
            {
                var totalBookings = await _dashboardRepository.GetReservedSumByDateRangeAsync(fromDate, toDate, location);
                return Ok(totalBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("sum-guest")]
        public async Task<ActionResult<int>> GetGuestSumByDateRangeAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string location)
        {
            try
            {
                var totalGuests = await _dashboardRepository.GetGuestSumByDateRangeAsync(fromDate, toDate, location);
                return Ok(totalGuests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string location)
        {
            try
            {
                var payments = await _dashboardRepository.GetPaymentsByDateRangeAsync(fromDate, toDate, location);

                var revenueByDate = payments
                    .GroupBy(p => p.TransactionDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalRevenue = g.Sum(p => p.Amount)
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return Ok(revenueByDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("room-status-count")]
        public async Task<ActionResult<int>> GetRoomStatusCount([FromQuery] string status, [FromQuery] string location)
        {
            try
            {
                var count = await _dashboardRepository.GetRoomStatusAsync(status, location);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("room-status-counts")]
        public async Task<ActionResult<Dictionary<string, int>>> GetRoomStatusCounts([FromQuery] string location)
        {
            try
            {
                var counts = await _dashboardRepository.GetRoomStatusCountsAsync(location);
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
