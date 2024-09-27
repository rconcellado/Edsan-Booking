using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EdsanBooking.Repositories;
using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using EdsanBooking.Utilities;

namespace EdsanBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestApiController : ControllerBase
    {
        private readonly GuestRepository _guestRepository;
        private readonly AccountRepository _accountRepository;
        private readonly AppSettings _appSettings;
        public GuestApiController(GuestRepository guestRepository, AccountRepository accountRepository, IOptions<AppSettings> appSettings)
        {
            _guestRepository = guestRepository;
            _accountRepository = accountRepository;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetAllGuest(int skip = 0, int? take = null, string searchTerm = null)
        {
            int pageSize = take ?? _appSettings.PageSize;

            var guest = await _guestRepository.GetGuestAsync(skip, pageSize, searchTerm);
            return Ok(guest);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuestById(string id)
        {
            var guest = await _guestRepository.GetGuestByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }
        [HttpGet("SearchGuests")]
        public async Task<IActionResult> SearchGuests(string searchTerm)
        {
            var guests = await _guestRepository.SearchGuestsAsync(searchTerm);
            return Ok(guests);
        }
        [HttpPost]
        public async Task<ActionResult<Guest>> CreateGuest(Guest guest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _guestRepository.AddGuestAsync(guest);

            return CreatedAtAction(nameof(GetGuestById), new { id = guest.GuestId }, guest);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(string id, Guest guest)
        {
            // Log the incoming request details
            Console.WriteLine($"Received update request for Guest ID: {id}");

            if (id != guest.GuestId)
            {
                Console.WriteLine("Guest ID mismatch.");
                return BadRequest("Guest ID mismatch.");
            }

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
                // Attempt to update the guest
                Console.WriteLine("Attempting to update the guest...");
                await _guestRepository.UpdateGuestAsync(guest);
                Console.WriteLine("Room updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_guestRepository.GuestExist(id))
                {
                    Console.WriteLine($"Guest with ID {id} not found.");
                    return NotFound($"Guest with ID {id} not found.");
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
        public async Task<IActionResult> DeleteGuest(string id)
        {
            var guest = await _guestRepository.GetGuestByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }

            await _guestRepository.DeleteGuestAsync(guest);  // Ensure this method exists in your repository

            return NoContent();
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetGuestCount(string searchTerm = null)
        {
            var count = await _guestRepository.GetTotalGuestCountAsync(searchTerm);
            return Ok(count);
        }
        [HttpGet("GuestExistsByFirstNameLastName")]
        public async Task<IActionResult> GuestExistByFirstNameLastName(string firstName, string lastName)
        {
            var guest = await _guestRepository.CheckGuestExistByFirstNameLastName(firstName, lastName);
            return Ok(guest);
        }
        [HttpGet("GenerateNextGuestId")]
        public async Task<IActionResult> GenerateNextGuestId()
        {
            var nextGuestId = await _guestRepository.GenerateNextGuestIdAsync();
            return Ok(new { GuestId = nextGuestId });  // Return as a JSON object
        }

        [HttpGet("GetGuestIdByFirstNameLastName")]
        public async Task<IActionResult> GetGuestIdByFirstNameLastName(string firstName, string lastName)
        {
            var guestid = await _guestRepository.GetGuestIdByFirstNameLastName(firstName, lastName);
            return Ok(new { GuestId = guestid });  // Return as a JSON object
        }
        [HttpPost("SaveOrUpdateGuest")]
        public async Task<IActionResult> SaveOrUpdateGuest(string id, GuestDetailViewModel guestDetailViewModel)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid guest data or checkInId ID.");
            }

            await _guestRepository.SaveOrUpdateGuestAsync(id, guestDetailViewModel);
            return Ok();
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
        {
            if (ModelState.IsValid)
            {
                //var user = new Users
                //{
                //    UserId = await _accountRepository.GenerateNextUserIdAsync(),
                //    UserName = signupDto.Username,
                //    Password = signupDto.Password, // Hash the password in real implementation
                //    FailedLoginAttempts = 0,
                //    IsLocked = false,
                //    Status = "Active",
                //    Location = signupDto.Location,
                //    UserRole = "Guest", // Assuming Guest role
                //    SecurityQuestion = "N/A",
                //    SecurityAnswer = "N/A",
                //    Location = signupDto.Location,
                //    CreatedBy = signupDto.Username,
                //    CreatedDt = DateTime.Now
                //};

                var guest = new Guest
                {
                    GuestId = await _guestRepository.GenerateNextGuestIdAsync(),
                    GName = signupDto.FirstName,
                    LName = signupDto.LastName,
                    Company = "Not applicable",
                    ContactNo = signupDto.ContactNo,
                    Address = "N/A",
                    GuestType = "Individual" // Assuming default type, adjust as needed
                };

                //await _accountRepository.CreateUserAsync(user);
                await _guestRepository.CreateGuestAsync(guest);

                return Ok(new { Message = "User and Guest created successfully" });
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetGuestIdByUserName/{username}")]
        public async Task<IActionResult> GetGuestIdByUserName(string username)
        {
            try
            {
                var guestId = await _accountRepository.GetGuestIdByUserName(username);
                if (guestId != null)
                {
                    return Ok(guestId);
                }
                else
                {
                    return NotFound("Guest ID not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "An error occurred while retrieving the Guest ID.");
            }
        }


    }
}
