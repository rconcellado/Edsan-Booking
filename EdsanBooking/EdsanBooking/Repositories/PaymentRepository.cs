using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Services;
using EdsanBooking.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdsanBooking.Repositories
{
    public class PaymentRepository
    {
        private readonly BookingContext _context;
        public PaymentRepository(BookingContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateNextTransactionIdAsync()
        {
            // Retrieve the last transaction ID in descending order
            var lastTransaction = await _context.PaymentHistory
                .OrderByDescending(r => r.TransactionId)
                .Select(r => r.TransactionId)
                .FirstOrDefaultAsync();

            // If no transaction exists, start with "0000000001"
            if (lastTransaction == null)
            {
                return "0000000001";
            }

            // Parse the last numeric part of the transaction ID
            int lastIdNumber = int.Parse(lastTransaction);

            // Increment the number by 1 and return as a 10-digit string
            return $"{(lastIdNumber + 1).ToString("D10")}";
        }
        public async Task AddRoomChargeAsync(string id)
        {
            var reservation = await _context.Reservation.FirstOrDefaultAsync(r => r.ReservationId == id);
            var checkIn = await _context.CheckIn.FirstOrDefaultAsync(r => r.CheckInId == id);

            if (reservation == null && checkIn == null) return;

            var serviceType = id.StartsWith("RES") ? "Reservation" : "CheckIn";

            var reservedId = serviceType == "CheckIn" ? "N/A" : id;
            var checkId = serviceType == "CheckIn" ? id : "N/A";

            if (serviceType == "CheckIn" && checkIn != null)
            {
                if (checkIn.CheckInType == "Transient")
                {
                    var roomCheckIns = await _context.RoomcheckIn.Where(r => r.CheckInId == id).ToListAsync();

                    if (roomCheckIns.Any())
                    {
                        var distinctRoomIds = roomCheckIns.Select(r => r.RoomId).Distinct().ToList();

                        decimal totalRoomCharge = 0m;

                        foreach (var roomId in distinctRoomIds)
                        {
                            var roomDetails = await _context.Room
                                .Where(room => room.RoomId == roomId)
                                .Select(room => new { room.FeatureName, room.TypeName, room.HourType })
                                .FirstOrDefaultAsync();

                            if (roomDetails != null)
                            {
                                var roomRate = await _context.RoomRates
                                    .Where(rate => rate.featureName == roomDetails.FeatureName &&
                                                   rate.typeName == roomDetails.TypeName &&
                                                   rate.hourType == roomDetails.HourType)
                                    .Select(rate => rate.RoomRate)
                                    .FirstOrDefaultAsync();

                                if (roomRate > 0)
                                {
                                    totalRoomCharge += roomRate;
                                }
                            }
                        }

                        var payment = await _context.PaymentHistory
                            .FirstOrDefaultAsync(r => r.CheckInId == id && r.TransactionType == "RMCHG");

                        if (payment != null)
                        {
                            if (payment.Amount != totalRoomCharge)
                            {
                                payment.Amount = totalRoomCharge;
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var charge = new PaymentHistory
                            {
                                TransactionId = await GenerateNextTransactionIdAsync(),
                                Amount = totalRoomCharge,
                                TransactionDate = DateTime.UtcNow,
                                ReservationId = "N/A",
                                CheckInId = id,
                                TransactionType = "RMCHG"
                            };
                            _context.PaymentHistory.Add(charge);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            if (serviceType == "Reservation" && (reservation?.ReservationType == "Resort" || checkIn?.CheckInType == "Resort"))
            {
                var resortRes = await _context.ResortRes
                    .FirstOrDefaultAsync(r => r.ReservationId == id || r.CheckInId == id);

                if (resortRes == null) return;

                var resortPKG = await _context.ResortPKG.FirstOrDefaultAsync(r => r.Descr == resortRes.PkgName);

                if (resortPKG == null) return;

                var resortCharge = resortPKG.PackageAmt;

                var payment = await _context.PaymentHistory
                    .FirstOrDefaultAsync(r => (r.ReservationId == id || r.CheckInId == id) && r.TransactionType == "RSCHG");

                if (payment != null)
                {
                    if (payment.Amount != resortCharge)
                    {
                        payment.Amount = resortCharge;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    var charge = new PaymentHistory
                    {
                        TransactionId = await GenerateNextTransactionIdAsync(),
                        Amount = resortCharge,
                        TransactionDate = DateTime.UtcNow,
                        ReservationId = reservedId,
                        CheckInId = checkId,
                        TransactionType = "RSCHG"
                    };

                    _context.PaymentHistory.Add(charge);
                    await _context.SaveChangesAsync();
                }
            }
        }



        public async Task AddPaymentAsync(string id, decimal chargeAmount, decimal payAmount, decimal discountAmount, 
                                          decimal excessAmount)
        {
            try
            {
                var isCheckIn = id.StartsWith("CHK");
                var checkInId = isCheckIn ? id : "N/A";
                var reservationId = isCheckIn ? "N/A" : id;

                // Helper function to add a payment record
                async Task AddPaymentRecordAsync(decimal amount, string transactionType)
                {
                    if (amount > 0)
                    {
                        var payment = new PaymentHistory
                        {
                            TransactionId = await GenerateNextTransactionIdAsync(),
                            Amount = amount,
                            TransactionDate = DateTime.UtcNow,
                            ReservationId = reservationId,
                            CheckInId = checkInId,
                            TransactionType = transactionType
                        };
                        _context.PaymentHistory.Add(payment);
                        await _context.SaveChangesAsync();
                    }
                }

                // Add payment, discount, and excess payment records
                await AddPaymentRecordAsync(payAmount, "PAY");
                await AddPaymentRecordAsync(discountAmount, "DSC");

                if (isCheckIn)
                {
                    await AddPaymentRecordAsync(excessAmount, "EXC");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("An error occurred while processing payments.", ex);
            }
        }

        public async Task<decimal> GetTotalPaymentsMadeAsync(string id)
        {
            if(id.Substring(0,3) == "CHK")
            {
                var paymentHistory = await _context.PaymentHistory
                .FirstOrDefaultAsync(r => r.CheckInId == id && r.TransactionType == "PAY");

                if (paymentHistory != null)
                {
                    var totalPayments = await _context.PaymentHistory
                    .Where(ph => ph.CheckInId == id && ph.TransactionType == "PAY")
                    .SumAsync(ph => ph.Amount);

                    return totalPayments;
                }
                else
                {
                    return 0;
                }
            }
            else 
            {
                var paymentHistory = await _context.PaymentHistory
                 .FirstOrDefaultAsync(r => r.ReservationId == id && r.TransactionType == "PAY");

                if (paymentHistory != null)
                {
                    var totalPayments = await _context.PaymentHistory
                    .Where(ph => ph.ReservationId == id && ph.TransactionType == "PAY")
                    .SumAsync(ph => ph.Amount);

                    return totalPayments;
                }
                else
                {
                    return 0;
                }
            }
        }



    }
}
