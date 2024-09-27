using Microsoft.EntityFrameworkCore;
using EdsanBooking.Models;

namespace EdsanBooking.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Guest> Guest { get; set; }
        public DbSet<SPRoomLoad> SPRoomLoad { get; set; }
        public DbSet<RoomFeature> RoomFeatures { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<RoomStatus> RoomStatus { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Comreserved> Comreserved { get; set; }
        public DbSet<TransientRes> TransientRes { get; set; }
        public DbSet<ResortRes> ResortRes { get; set; }
        public DbSet<CheckIn> CheckIn { get; set; }
        public DbSet<Roomcheckin> RoomcheckIn { get; set; }
        public DbSet<ResortPKG> ResortPKG { get; set; }
        public DbSet<PaymentHistory> PaymentHistory { get; set; }
        public DbSet<RoomRates> RoomRates {  get; set; }
        public DbSet<Resortamenities> Resortamenities { get; set; }
        public DbSet<RoomImages> RoomImages { get; set; }
        public DbSet<UserGuest> UserGuest { get; set; }

    }
}
