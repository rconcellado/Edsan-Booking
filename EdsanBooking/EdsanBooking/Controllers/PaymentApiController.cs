using EdsanBooking.Models;
using EdsanBooking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {
        private readonly PaymentRepository _paymentRepository;
        public PaymentApiController(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentRequestModel request)
        {
            try
            {
                await _paymentRepository.AddPaymentAsync(request.Id, request.ChargeAmount,
                                                         request.PaymentAmount, request.DiscountAmount, request.ExcessAmount);
                return Ok(new { success = true, message = "Payment added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("GetTotalPaymentsMadeAsync")]
        public async Task<ActionResult<decimal>> GetTotalPaymentsMadeAsync([FromBody] string id)
        {
            var payment = await _paymentRepository.GetTotalPaymentsMadeAsync(id);

            // Always return a value, even if it's 0
            return Ok(payment);
        }
        [HttpPost("AddRoomChargeAsync")]
        public async Task<IActionResult> AddRoomChargeAsync([FromBody] string id)
        {
            try
            {
                await _paymentRepository.AddRoomChargeAsync(id);
                return Ok(new { success = true, message = "Charges added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }
}
