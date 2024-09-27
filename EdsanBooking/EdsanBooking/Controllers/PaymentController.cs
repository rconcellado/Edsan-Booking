using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdsanBooking.Controllers
{
    //[Route("Payment")]
    public class PaymentController : Controller
    {
        private readonly ICheckInService _checkInService;
        private readonly IReservationService _reservationService;
        private readonly IChargeService _chargeService;
        private readonly IPaymentService _paymentService;
        public PaymentController(ICheckInService checkInService, IReservationService reservationService,
                                 IChargeService chargeService, IPaymentService paymentService)
        {
            _chargeService = chargeService;
            _paymentService = paymentService;
            _checkInService = checkInService;
            _reservationService = reservationService;
        }
        //[Route("Payment/{id}")]
        public async Task<IActionResult> EnterPayment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var location = HttpContext.Session.GetString("Location");
            ViewBag.Location = location;

            try
            {
                var referenceId = "";
                var serviceType = "";
                decimal totalCharge = 0;

                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                if (checkIn != null) {
                    referenceId = checkIn.CheckInId;
                    serviceType = checkIn.CheckInType;
                }
                else
                {
                    var reservation = await _reservationService.GetReservationByIdAsync(id);
                    referenceId = reservation.ReservationId;
                    serviceType = reservation.ReservationType;
                }

                if(serviceType == "Transient")
                {
                    totalCharge = await _chargeService.GetTransientTotalAmountAsync(referenceId, location);
                }
                else
                {
                    totalCharge = await _chargeService.GetResortTotalAmountAsync(referenceId, location);    
                }

                var totalPayment = await _paymentService.GetTotalPaymentsMadeAsync(id);

                var model = new PaymentViewModel
                {
                    PayType = "CheckIn",
                    Location = location,
                    ReservationId = id.StartsWith("CHK") ? "N/A" : id,
                    CheckInId = id.StartsWith("CHK") ? id : "N/A",
                    ChargeAmount = totalCharge,
                    PaymentAmount = totalPayment,
                    RemainingBalance = totalCharge - totalPayment
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterPayment(PaymentViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return the view to show validation errors
                return View(model);
            }

            string id = model.CheckInId == "N/A" ? model.ReservationId : model.CheckInId;

            if (string.IsNullOrEmpty(id))
            {
                // Handle the error if neither ID is provided
                ModelState.AddModelError("", "No valid ID provided.");
                return View(model);
            }

            await _paymentService.AddPaymentAsync(id, model.ChargeAmount, model.CurrentPayment,
                                                  model.DiscountAmount, model.ExcessPayment);

            TempData["SuccessMessage"] = "Payment processed successfully.";
            if(id.StartsWith("CHK")) {
                return RedirectToAction("Checkinlist", "CheckIn", new { id = id });
            }
            else
            {
                return RedirectToAction("Reservationlist", "Reservation", new { id = id });
            }
            
        }

    }
}
