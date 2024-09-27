// Models/DashboardViewModel.cs
using System.Collections.Generic;

namespace EdsanBooking.Models
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OccupiedHouses { get; set; }
        public int AvailableHouses { get; set; }
        public List<Booking> RecentBookings { get; set; }
        public List<Revenue> RevenueData { get; set; }
    }
}
