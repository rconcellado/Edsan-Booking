using EdsanBooking.Configuration;
using EdsanBooking.Models;
using EdsanBooking.Repositories;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingApiController : ControllerBase
    {
        private readonly SettingRepository _settingRepository;
        private readonly AppSettings _appSettings;
        public SettingApiController(SettingRepository settingRepository, IOptions<AppSettings> appSettings)
        {
            _settingRepository = settingRepository;
            _appSettings = appSettings.Value;
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalRoomRatesCount(string searchTerm = null)
        {
            var count = await _settingRepository.GetTotalRoomRatesCountAsync(searchTerm);
            return Ok(count);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetAllRoomRates(int skip = 0, int? take = null, string loc = null, string searchTerm = null)
        {
            int pageSize = take ?? _appSettings.PageSize;

            var guest = await _settingRepository.GetRoomRates(skip, pageSize, loc, searchTerm);
            return Ok(guest);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetRoomRatesById(string id)
        {
            var guest = await _settingRepository.GetRoomRatesByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomRate(string id, RoomRates roomRate)
        {
            if(id != roomRate.ID)
            {
                return BadRequest("Room ID mismatch.");
            }

            if (roomRate == null)
            {
                return BadRequest("RoomRate is null");
            }

            try
            {
                await _settingRepository.UpdateRoomRateAsync(roomRate);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
