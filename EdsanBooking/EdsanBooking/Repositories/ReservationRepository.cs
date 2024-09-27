using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdsanBooking.Repositories
{
    /// <summary>
    /// Repository for handling reservation-related database operations.
    /// </summary>
    public class ReservationRepository
    {
        #region Fields

        private readonly BookingContext _context;
        private readonly IGuestService _guestService;
        private readonly ChargeRepository _chargeRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="guestService">The guest service.</param>
        public ReservationRepository(BookingContext context, IGuestService guestService, ChargeRepository chargeRepository)
        {
            _context = context;
            _guestService = guestService;
            _chargeRepository = chargeRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the total count of reservations based on search term and location.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="loc">The location.</param>
        /// <returns>The total count of reservations.</returns>
        public async Task<int> GetTotalReservationCountAsync(string searchTerm = null, string loc = null)
        {
            var query = _context.Reservation.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.ReservationId.Contains(searchTerm) ||
                                        r.GuestId.Contains(searchTerm) ||
                                        r.Status.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(loc))
            {
                query = query.Where(r => r.Location == loc);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Gets a paginated list of reservations based on search term and location.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="loc">The location.</param>
        /// <returns>A list of reservation view models.</returns>
        public async Task<List<ReservationViewModel>> GetReservationAsync(int skip, int take, string searchTerm = null, string loc = null)
        {
            var query = from res in _context.Reservation
                        join guest in _context.Guest
                        on res.GuestId equals guest.GuestId
                        where res.Location == loc
                        select new ReservationViewModel
                        {
                            ReservationId = res.ReservationId,
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
                            ReservationType = res.ReservationType,
                            CreatedBy = res.CreatedBy,
                            CreatedDt = res.CreatedDt,
                            Location = res.Location,
                            GuestType = guest.GuestType
                        };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.ReservationId.Contains(searchTerm) ||
                                         r.GuestId.Contains(searchTerm) ||
                                         r.Status.Contains(searchTerm) ||
                                         (r.FirstName + " " + r.LastName).Contains(searchTerm) ||
                                         r.Company.Contains(searchTerm));
            }

            return await query.OrderBy(r => r.ReservationId)
                              .Skip(skip)
                              .Take(take)
                              .ToListAsync();
        }

        /// <summary>
        /// Gets a reservation by its ID.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>The reservation object.</returns>
        public async Task<Reservation> GetReservationByIdAsync(string id)
        {
            //return await _context.Reservation.FindAsync(id);
            return await _context.Reservation.FirstOrDefaultAsync(r => r.ReservationId == id);
        
        }
        /// <summary>
        /// Checks if a reservation exists by its ID.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>True if the reservation exists; otherwise, false.</returns>
        public bool ReservationExists(string id)
        {
            return _context.Reservation.Any(e => e.ReservationId == id);
        }

        /// <summary>
        /// Generates the next reservation ID.
        /// </summary>
        /// <returns>The next reservation ID.</returns>
        public async Task<string> GenerateNextReservationIdAsync()
        {
            var lastReservation = await _context.Reservation
                            .OrderByDescending(g => g.ReservationId)
                            .FirstOrDefaultAsync();

            if (lastReservation == null)
            {
                return "RES0000001"; // Starting point if there are no records
            }

            int lastIdNumber = int.Parse(lastReservation.ReservationId.Substring(3));
            return $"RES{(lastIdNumber + 1).ToString("D7")}";
        }

        /// <summary>
        /// Confirms a transient reservation by its ID.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the confirmation is successful; otherwise, false.</returns>
        public async Task<bool> ConfirmTransientReservationAsync(string reservationId)
        {
            try
            {
                var reservation = await _context.Reservation
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

                if (reservation == null)
                {
                    throw new Exception("Reservation not found.");
                }

                var guestCount = await _context.Comreserved
                    .Where(g => g.ReservationId == reservationId)
                    .CountAsync();

                if (guestCount != reservation.NumGuest)
                {
                    throw new Exception("The number of guests in the reservation does not match the number of guests being listed.");
                }

                await _context.Database.ExecuteSqlRawAsync("CALL confirm_transientreservation({0})", reservationId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming reservation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Confirms a resort reservation by its ID.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the confirmation is successful; otherwise, false.</returns>
        public async Task<bool> ConfirmResortReservationAsync(string reservationId)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL confirm_resortreservation({0})", reservationId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming reservation: {ex.Message}");
                return false;
            }
        }
        public async Task<Reservation> SaveOrUpdateReservationAsync(Reservation reservation,
                                                            string featureName, string typeName, int hourType, string pkgName,
                                                            bool isNew)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (isNew)
                    {
                        reservation.CreatedDt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Ensure the reservation entity is tracked by the context and marked as modified
                        _context.Attach(reservation);
                        _context.Entry(reservation).State = EntityState.Modified;
                    }

                    reservation.CheckInDt = reservation.CheckInDt.ToUniversalTime();
                    reservation.CheckOutDt = reservation.CheckOutDt.ToUniversalTime();
                    reservation.CreatedDt = reservation.CreatedDt.ToUniversalTime();

                    if (isNew)
                    {
                        _context.Reservation.Add(reservation);
                    }

                    await _context.SaveChangesAsync();

                    await _chargeRepository.SaveOrUpdateSpace(reservation.ReservationId, reservation.ReservationType,
                                                              featureName, typeName, hourType, pkgName);

                    await transaction.CommitAsync();
                    return reservation;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"An error occurred while saving or updating the reservation: {ex.Message}");
                }
            }
        }
        public async Task<List<GuestDetailViewModel>> GetGuestsByReservationIdAsync(string reservationId)
        {
            var guests = await (from cr in _context.Comreserved
                                join g in _context.Guest on cr.GuestId equals g.GuestId
                                where cr.ReservationId == reservationId
                                select new GuestDetailViewModel
                                {
                                    FirstName = g.GName,
                                    LastName = g.LName,
                                    Preference = cr.Preference
                                }).ToListAsync();

            return guests;
        }
        public async Task DeleteComReservedRecordsAsync(string reservationId)
        {
            var comReservedRecords = await _context.Comreserved
                .Where(c => c.ReservationId == reservationId)
                .ToListAsync();

            if (comReservedRecords.Any())
            {
                _context.Comreserved.RemoveRange(comReservedRecords);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
