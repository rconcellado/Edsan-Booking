using Microsoft.AspNetCore.Mvc;
using EdsanBooking.Models;
using EdsanBooking.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Options;

namespace EdsanBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomApiController : ControllerBase
    {
        private readonly RoomRepository _roomRepository;
        private readonly AppSettings _appSettings;

        public RoomApiController(RoomRepository roomRepository, IOptions<AppSettings> appSettings)
        {
            _roomRepository = roomRepository;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRooms(int skip = 0, int? take = null, string searchTerm = null, string loc = null)
        {
            // Use _appSettings.PageSize if take is not provided
            int pageSize = take ?? _appSettings.PageSize;

            var rooms = await _roomRepository.GetRoomsAsync(skip, pageSize, searchTerm, loc);
            return Ok(rooms);
        }
        [HttpGet("GetRoomAvailability")]
        public async Task<IActionResult> GetRoomAvailability([FromQuery] string location, [FromQuery] DateTime checkInDate, [FromQuery] DateTime checkOutDate)
        {
            try
            {
                var roomAvailability = await _roomRepository.GetRoomAvailabilityAsync(location, checkInDate, checkOutDate);
                if (roomAvailability.Any())
                {
                    return Ok(roomAvailability);
                }
                return NotFound(new { message = "No rooms available for the selected criteria." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("GetRoomDetails")]
        public async Task<IActionResult> GetRoomDetails(string featureName, string typeName, int hourType = 12)
        {
            var roomDetails = await _roomRepository.GetRoomDetailsAsync(featureName, typeName, hourType);

            if (roomDetails == null)
            {
                return NotFound(new { message = "Room details not found." });
            }

            return Ok(roomDetails);
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadRoomImages([FromForm] string featureName, [FromForm] string typeName, [FromForm] IFormFile imageFile1, [FromForm] IFormFile imageFile2, [FromForm] IFormFile imageFile3, [FromForm] IFormFile imageFile4)
        {
            if (imageFile1 == null || imageFile2 == null || imageFile3 == null || imageFile4 == null)
            {
                return BadRequest("All 4 images must be uploaded.");
            }

            var imagePath1 = await SaveFile(imageFile1);
            var imagePath2 = await SaveFile(imageFile2);
            var imagePath3 = await SaveFile(imageFile3);
            var imagePath4 = await SaveFile(imageFile4);

            var roomImage = new RoomImages
            {
                FeatureName = featureName,
                TypeName = typeName,
                ImagePath1 = imagePath1,
                ImagePath2 = imagePath2,
                ImagePath3 = imagePath3,
                ImagePath4 = imagePath4,
            };

            await _roomRepository.SaveRoomImagesAsync(roomImage);

            return Ok("Images uploaded successfully.");
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var filePath = Path.Combine("wwwroot/images/rooms", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/rooms/{file.FileName}";
        }
        [HttpGet("images")]
        public async Task<IActionResult> GetRoomImages([FromQuery] string featureName, [FromQuery] string typeName)
        {
            var roomImages = await _roomRepository.GetRoomImagesAsync(featureName, typeName);

            if (roomImages == null)
            {
                return NotFound("No images found for the specified room.");
            }

            return Ok(roomImages);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomById(string id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _roomRepository.AddRoomAsync(room);

            return CreatedAtAction(nameof(GetRoomById), new { id = room.RoomId }, room); // Return the room object
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(string id, Room room)
        {
            // Log the incoming request details
            Console.WriteLine($"Received update request for Room ID: {id}");

            if (id != room.RoomId)
            {
                Console.WriteLine("Room ID mismatch.");
                return BadRequest("Room ID mismatch.");
            }

            // Validate the model state
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            try
            {
                // Attempt to update the room
                Console.WriteLine("Attempting to update the room...");
                await _roomRepository.UpdateRoomAsync(room);
                Console.WriteLine("Room updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_roomRepository.RoomExists(id))  // Ensure this method checks if the room exists
                {
                    Console.WriteLine($"Room with ID {id} not found.");
                    return NotFound($"Room with ID {id} not found.");
                }
                else
                {
                    Console.WriteLine("Concurrency exception occurred.");
                    throw; // Re-throw the exception to be handled by global exception handlers or middleware
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            // If everything goes well, return a NoContent result
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            await _roomRepository.DeleteRoomAsync(room);  // Ensure this method exists in your repository

            return NoContent();
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetRoomCount(string searchTerm = null, string loc = null)
        {
            var count = await _roomRepository.GetTotalRoomCountAsync(searchTerm, loc);
            return Ok(count);
        }
        [HttpGet("GenerateNextRoomId")]
        public async Task<IActionResult> GenerateNextRoomId()
        {
            var nextRoomId = await _roomRepository.GenerateNextRoomIdAsync();
            return Ok(new { RoomId = nextRoomId });  // Return as a JSON object
        }
    }
}
