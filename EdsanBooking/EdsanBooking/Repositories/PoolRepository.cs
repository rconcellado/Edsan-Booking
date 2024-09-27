using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdsanBooking.Repositories
{
    public class PoolRepository
    {
        private readonly BookingContext _context;
        public PoolRepository(BookingContext context)
        {
            _context= context;
        }
        public async Task<CheckInDetailsViewModel> GetDetailsAsync(string id)
        {
            try
            {
                var reservation = await _context.Reservation
                    .Where(r => r.ReservationId == id)
                    .FirstOrDefaultAsync();

                var checkIn = await _context.CheckIn
                    .Where(c => c.CheckInId == id)
                    .FirstOrDefaultAsync();

                if (checkIn == null && reservation == null)
                    return null;

                //var chkGuestId = "";

                //if (id.StartsWith("CHK"))
                //{
                //    chkGuestId = checkIn.GuestId;
                //}
                //else
                //{
                //    chkGuestId = reservation.GuestId;
                //}

                var chkGuestId = id.StartsWith("CHK") ? checkIn.GuestId : reservation.GuestId;

                var guestInfo = await _context.Guest
                    .FirstOrDefaultAsync(r => r.GuestId == chkGuestId);

                if (guestInfo == null)
                    return null;

                var guestName = guestInfo.GuestType == "Individual"
                    ? $"{guestInfo.GName} {guestInfo.LName}"
                    : guestInfo.Company;

                var featureName = "";
                var typeName = "";
                var hourType = 0;
                var pkgName = "";

                List<ResortAmenitiesViewModel> resortAmenitiesDetails = new List<ResortAmenitiesViewModel>();

                if (reservation?.ReservationType == "Transient" || checkIn?.CheckInType == "Transient")
                {
                    var transient = await _context.TransientRes
                        .FirstOrDefaultAsync(r => r.CheckInId == id || r.ReservationId == id);

                    if (transient != null)
                    {
                        featureName = transient.FeatureName;
                        typeName = transient.TypeName;
                        hourType = transient.HourType;
                    }
                }
                
                if (reservation?.ReservationType == "Resort" || checkIn?.CheckInType == "Resort")
                {
                    var resort = await _context.ResortRes
                        .FirstOrDefaultAsync(r => r.CheckInId == id || r.ReservationId == id);

                    if(resort != null)
                    {
                        pkgName = resort.PkgName;
                    }

                    resortAmenitiesDetails = await (from rs in _context.Resortamenities
                                                    where rs.PkgName == pkgName
                                                    select new ResortAmenitiesViewModel
                                                    {
                                                        Amenity = rs.Amenity
                                                    }).ToListAsync();
                }

                List<GuestViewModel> guestDetails;

                if (id.StartsWith("CHK"))
                {
                    guestDetails = await (from rc in _context.RoomcheckIn
                                          join g in _context.Guest on rc.GuestId equals g.GuestId
                                          join r in _context.Room on rc.RoomId equals r.RoomId
                                          where rc.CheckInId == id
                                          select new GuestViewModel
                                          {
                                              FirstName = g.GName,
                                              LastName = g.LName,
                                              RoomDescription = r.Descr // Add the room description here
                                          }).ToListAsync();
                }
                else
                {
                    guestDetails = await (from rc in _context.Comreserved
                                          join g in _context.Guest on rc.GuestId equals g.GuestId
                                          where rc.ReservationId == id
                                          select new GuestViewModel
                                          {
                                              FirstName = g.GName,
                                              LastName = g.LName,
                                              Preference = rc.Preference // Add the room description here
                                          }).ToListAsync();
                }

                var paymentDetails = await (from ph in _context.PaymentHistory
                                            where ph.CheckInId == id || ph.ReservationId == id
                                            select new PaymentHistoryViewModel
                                            {
                                                TransactionId = ph.TransactionId,
                                                Amount = ph.Amount,
                                                TransactionDate = ph.TransactionDate,
                                                TransactionType = ph.TransactionType
                                            }).ToListAsync();

                // Calculate the total amount based on the specific transaction types
                var totalOutstanding = paymentDetails
                    .Where(ph => ph.TransactionType == "RMCHG" || ph.TransactionType == "RSCHG" || ph.TransactionType == "EXC")
                    .Sum(ph => ph.Amount)
                    - paymentDetails
                    .Where(ph => ph.TransactionType == "PAY" || ph.TransactionType == "DSC")
                    .Sum(ph => ph.Amount);

                if (id.StartsWith("CHK"))
                {
                    return new CheckInDetailsViewModel
                    {
                        CheckInId = checkIn.CheckInId,
                        Location = checkIn.Location,
                        GuestId = checkIn.GuestId,
                        GuestName = guestName,
                        NumGuest = checkIn.NumGuest,
                        CheckInDt = checkIn.CheckInDt,
                        CheckInTime = checkIn.CheckInTime,
                        CheckOutDt = checkIn.CheckOutDt,
                        CheckOutTime = checkIn.CheckOutTime,
                        Remarks = checkIn?.Remarks,
                        ReservationId = "N/A",
                        CheckInType = checkIn?.CheckInType,
                        ReservationType = "N/A",
                        Status = checkIn?.Status,
                        CreatedBy = checkIn.CreatedBy,
                        CreatedDt = checkIn.CreatedDt,
                        Guests = guestDetails,
                        Payments = paymentDetails,
                        TotalOutstanding = totalOutstanding,
                        featureName = featureName,
                        typeName = typeName,
                        hourType = hourType,
                        resortAmenities = resortAmenitiesDetails
                    };
                }
                else
                {
                    return new CheckInDetailsViewModel
                    {
                        CheckInId = "N/A",
                        Location = reservation.Location,
                        GuestId = reservation.GuestId,
                        GuestName = guestName,
                        NumGuest = reservation.NumGuest,
                        CheckInDt = reservation.CheckInDt,
                        CheckInTime = reservation.CheckInTime,
                        CheckOutDt = reservation.CheckOutDt,
                        CheckOutTime = reservation.CheckOutTime,
                        Remarks = reservation?.Remarks,
                        ReservationId = reservation?.ReservationId,
                        ReservationType = reservation?.ReservationType,
                        CheckInType = "N/A",
                        Status = reservation?.Status,
                        CreatedBy = reservation.CreatedBy,
                        CreatedDt = reservation.CreatedDt,
                        Guests = guestDetails,
                        Payments = paymentDetails,
                        TotalOutstanding = totalOutstanding,
                        featureName = featureName,
                        typeName = typeName,
                        hourType = hourType,
                        resortAmenities = resortAmenitiesDetails
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as necessary
                throw new Exception($"An error occurred while fetching the check-in details: {ex.Message}", ex);
            }
        }



    }
}
