using EdsanBooking.Models;
using EdsanBooking.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInApiController : ControllerBase
    {
        private readonly CheckInRepository _checkInRepository;
        public CheckInApiController(CheckInRepository checkinRepository)
        {
            _checkInRepository = checkinRepository;
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCheckInCount(string searchTerm = null)
        {
            var count = await _checkInRepository.GetTotalCheckInCountAsync(searchTerm);
            return Ok(count);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckInViewModel>>> GetAllCheckin(int skip = 0, int take = 12, string searchTerm = null, string loc = null)
        {
            var checkIn = await _checkInRepository.GetCheckInAsync(skip, take, searchTerm, loc);
            return Ok(checkIn);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckInById(string id)
        {
            var checkIn = await _checkInRepository.GetCheckInByIdAsync(id);
            if (checkIn == null)
            {
                return NotFound();
            }

            return Ok(checkIn);
        }
        [HttpPost("SaveOrUpdateCheckInAsync")]
        public async Task<ActionResult<CheckIn>> SaveOrUpdateCheckInAsync([FromBody] CreateCheckInRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCheckIn = await _checkInRepository.SaveOrUpdateCheckInAsync(
                    request.CheckIn, request.FeatureName,
                    request.TypeName, request.HourType, request.pkgName, request.isNew);

                return Ok(createdCheckIn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("GenerateNextCheckInId")]
        public async Task<IActionResult> GenerateNextReservationId()
        {
            var nextCheckInId = await _checkInRepository.GenerateNextCheckInIdAsync();
            return Ok(new { CheckInId = nextCheckInId });
        }
        [HttpGet("GetGuestsByCheckInId/{checkInId}")]
        public async Task<ActionResult<List<GuestDetailViewModel>>> GetGuestsByCheckInIdAsync(string checkInId)
        {
            var guests = await _checkInRepository.GetGuestsByCheckInIdAsync(checkInId);
            return Ok(guests);
        }
        [HttpDelete("DeleteRoomCheckInRecords/{checkInId}")]
        public async Task<IActionResult> DeleteRoomCheckInRecords(string checkInId)
        {
            if (string.IsNullOrEmpty(checkInId))
            {
                return BadRequest("CheckIn ID is required.");
            }

            await _checkInRepository.DeleteRoomCheckInRecordsAsync(checkInId);
            return Ok();
        }
        [HttpPost("UpdateRoomStatusAsync")]
        public async Task<IActionResult> UpdateRoomStatusAsync([FromBody] string checkInId)
        {
            if (string.IsNullOrEmpty(checkInId))
            {
                return BadRequest("Invalid guest data or checkInId ID.");
            }

            await _checkInRepository.UpdateRoomStatusAsync(checkInId);
            return Ok();
        }
        [HttpPost("ConfirmCheckInAsync")]
        public async Task<IActionResult> ConfirmCheckInAsync([FromBody] string id)
        {
            await _checkInRepository.ConfirmCheckInAsync(id);
            return Ok();
        }
        [HttpPost("ConfirmCheckOutAsync")]
        public async Task<IActionResult> ConfirmCheckOutAsync([FromBody] string id)
        {
            await _checkInRepository.ConfirmCheckOutAsync(id);
            return Ok();
        }

    }
}
