namespace EdsanBooking.Models
{
    public class SummaryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Location { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCheckOut { get; set; }
        public int TotalReservations { get; set; }
        public int TotalGuests { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }

        // New properties for room status
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int MaintenanceRooms { get; set; }
        public Dictionary<string, int> RoomStatusCounts { get; set; } // New property for room status counts

        // New property for revenue statistics
        public List<RevenueData> RevenueStatistics { get; set; } = new List<RevenueData>();

        //public int TotalRooms => AvailableRooms + OccupiedRooms + MaintenanceRooms;

        //public double AvailableRoomsPercent => TotalRooms == 0 ? 0 : (double)AvailableRooms / TotalRooms * 100;
        //public double OccupiedRoomsPercent => TotalRooms == 0 ? 0 : (double)OccupiedRooms / TotalRooms * 100;
        //public double MaintenanceRoomsPercent => TotalRooms == 0 ? 0 : (double)MaintenanceRooms / TotalRooms * 100;
    }
    public class RevenueData
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
