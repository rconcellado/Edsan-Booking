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
    public class PoolApiController : ControllerBase
    {
        private readonly PoolRepository _poolRepository;
        public PoolApiController(PoolRepository poolRepository)
        {
            _poolRepository = poolRepository;
        }
        [HttpGet("GetDetails/{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            try
            {
                var checkInDetails = await _poolRepository.GetDetailsAsync(id);

                if (checkInDetails == null)
                {
                    return NotFound();
                }

                return Ok(checkInDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
