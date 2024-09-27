using EdsanBooking.Models;
using EdsanBooking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeApiController : ControllerBase
    {
        private readonly ChargeRepository _chargeRepository;
        public ChargeApiController(ChargeRepository chargeRepository)
        {
            _chargeRepository = chargeRepository;
        }
        [HttpGet("GetTransientDetails/{id}")]
        public async Task<ActionResult<TransientRes>> GetTransientDetailsById(string id)
        {
            var transientCheckIn = await _chargeRepository.GetTransientDetailsByIdAsync(id);

            if (transientCheckIn == null)
            {
                return NotFound();
            }

            return Ok(transientCheckIn);
        }
        [HttpGet("GetResortDetails/{id}")]
        public async Task<ActionResult<ResortRes>> GetResortDetailsByCheckInId(string id)
        {
            var resortCheckIn = await _chargeRepository.GetResortDetailsByIdAsync(id);

            if (resortCheckIn == null)
            {
                return NotFound();
            }

            return Ok(resortCheckIn);
        }
        [HttpPost("CalculateTransientTotalAmountAsync")]
        public async Task<ActionResult<decimal>> CalculateTransientTotalAmountAsync([FromBody] JsonElement request)
        {
            try
            {
                if (!request.TryGetProperty("id", out var idElement) || !request.TryGetProperty("location", out var locationElement))
                {
                    return BadRequest(new { Error = "Invalid request format." });
                }

                string id = idElement.GetString();
                string location = locationElement.GetString();

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location))
                {
                    return BadRequest(new { Error = "Invalid request data." });
                }

                var totalamount = await _chargeRepository.CalculateTransientTotalAmountAsync(id, location);
                return Ok(totalamount);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpPost("CalculateResortTotalAmountAsync")]
        public async Task<ActionResult<decimal>> CalculateResortTotalAmountAsync([FromBody] JsonElement request)
        {
            try
            {
                if (!request.TryGetProperty("id", out var idElement) || !request.TryGetProperty("location", out var locationElement))
                {
                    return BadRequest(new { Error = "Invalid request format." });
                }

                string id = idElement.GetString();
                string location = locationElement.GetString();

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location))
                {
                    return BadRequest(new { Error = "Invalid request data." });
                }

                var totalamount = await _chargeRepository.CalculateResortTotalAmountAsync(id, location);
                return Ok(totalamount);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


    }
}
