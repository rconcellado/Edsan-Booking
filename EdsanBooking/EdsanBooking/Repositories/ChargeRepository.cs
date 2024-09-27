using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EdsanBooking.Repositories
{
    public class ChargeRepository
    {
        private readonly BookingContext _context;
        public ChargeRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<TransientRes> GetTransientDetailsByIdAsync(string id)
        {
            return await _context.TransientRes
            .FirstOrDefaultAsync(r => id.StartsWith("RES") ? r.ReservationId == id : r.CheckInId == id);

        }
        public async Task<ResortRes> GetResortDetailsByIdAsync(string id)
        {
            return await _context.ResortRes
                .FirstOrDefaultAsync(r => id.StartsWith("RES") ? r.ReservationId == id : r.CheckInId == id);
        }
        public async Task<decimal> CalculateTransientTotalAmountAsync(string id, string location)
        {
            try
            {
                if (id.StartsWith("CHK"))
                {
                    // Retrieve transient check-in details and check-in record in a single query
                    var transientCheck = await _context.TransientRes
                    .Where(tr => tr.CheckInId == id)
                    .Select(tr => new
                    {
                        tr.FeatureName,
                        tr.TypeName,
                        tr.HourType,
                        CheckInType = _context.CheckIn
                            .Where(c => c.CheckInId == id)
                            .Select(c => c.CheckInType)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();


                    if (transientCheck == null)
                    {
                        throw new Exception("Transient check-in details or check-in record not found.");
                    }

                    // Early exit if the check-in type is not "Transient"
                    if (transientCheck.CheckInType != "Transient")
                    {
                        return 0;
                    }

                    // Retrieve the room rate
                    var roomRate = await _context.RoomRates
                        .Where(r => r.featureName == transientCheck.FeatureName &&
                                    r.typeName == transientCheck.TypeName &&
                                    r.hourType == transientCheck.HourType &&
                                    r.Location == location)
                        .Select(r => r.RoomRate)
                        .FirstOrDefaultAsync();

                    if (roomRate == 0)
                    {
                        throw new Exception("Room rate not found for the specified room details.");
                    }

                    // Retrieve distinct room IDs associated with the check-in
                    var roomIds = await _context.RoomcheckIn
                        .Where(rc => rc.CheckInId == id)
                        .Select(rc => rc.RoomId)
                        .Distinct()
                        .ToListAsync();

                    // Calculate total amount based on distinct room IDs
                    return roomIds.Count * roomRate;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating the total amount.", ex);
            }
        }
        public async Task<decimal> CalculateResortTotalAmountAsync(string id, string location)
        {
            try
            {
                var checkIn = await _context.CheckIn
                            .FirstOrDefaultAsync(r => r.CheckInId == id);

                var reservation = await _context.Reservation
                            .FirstOrDefaultAsync(r => r.ReservationId == id);

                if (checkIn == null && reservation == null)
                    return 0;

                var pkgName = "";

                var resortRes = await _context.ResortRes
                    .FirstOrDefaultAsync(r => r.ReservationId == id || r.CheckInId == id);

                if (resortRes != null)
                {
                    pkgName = resortRes.PkgName;
                }

                var resortPkg = await _context.ResortPKG
                    .FirstOrDefaultAsync(r => r.Descr == pkgName && r.Location == location);

                if (resortPkg != null) {
                    return resortPkg.PackageAmt;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating the total amount.", ex);
                return 0;
            }
        }
        public async Task SaveOrUpdateSpace(string id, string Type, string featureName, string typeName, int hourType, string pkgName)
        {
            if (Type == "Transient" && !string.IsNullOrEmpty(featureName) && !string.IsNullOrEmpty(typeName))
            {
                // Determine if the id is a ReservationId or CheckInId and fetch the existing transient record
                var transientRes = await _context.TransientRes
                    .FirstOrDefaultAsync(r => r.ReservationId == id || r.CheckInId == id);

                if (transientRes != null)
                {
                    _context.Remove(transientRes);
                    await _context.SaveChangesAsync();
                }

                // Create new transient record
                var newTransientRes = new TransientRes
                {
                    TrResId = IdGenerator.GenerateNextId(_context.TransientRes, "TRS", r => r.TrResId),
                    FeatureName = featureName,
                    TypeName = typeName,
                    HourType = hourType,
                    ReservationId = id.StartsWith("CHK") ? "N/A" : id,
                    CheckInId = id.StartsWith("CHK") ? id : "N/A"
                };

                _context.TransientRes.Add(newTransientRes);
                await _context.SaveChangesAsync();
                _context.Entry(newTransientRes).State = EntityState.Detached;
            }
            else if (Type == "Resort" && !string.IsNullOrEmpty(pkgName))
            {
                // Determine if the id is a ReservationId or CheckInId and fetch the existing resort record
                var resortRes = await _context.ResortRes
                    .FirstOrDefaultAsync(r => r.ReservationId == id || r.CheckInId == id);

                if (resortRes != null)
                {
                    _context.Remove(resortRes);
                    await _context.SaveChangesAsync();
                }

                // Create new resort record
                var newResortRes = new ResortRes
                {
                    ResortResId = IdGenerator.GenerateNextId(_context.ResortRes, "RST", r => r.ResortResId),
                    PkgName = pkgName,
                    ReservationId = id.StartsWith("CHK") ? "N/A" : id,
                    CheckInId = id.StartsWith("CHK") ? id : "N/A"
                };

                _context.ResortRes.Add(newResortRes);
                await _context.SaveChangesAsync();
                _context.Entry(newResortRes).State = EntityState.Detached;
            }
        }




    }
}
