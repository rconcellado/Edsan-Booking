using EdsanBooking.Data;
using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Configuration;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EdsanBooking.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace EdsanBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly BookingContext _context;
        private readonly IRoomService _roomService;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Constructor for RoomController
        /// </summary>
        /// <param name="roomService">Service interface for room operations</param>
        /// <param name="context">Database context</param>
        public RoomController(IRoomService roomService, BookingContext context, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _roomService = roomService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _baseUrl = configuration["BaseUrl"]; // Get the base URL from configuration
            _appSettings = appSettings.Value;
        }

        #region Resort Action Methods

        /// <summary>
        /// Displays a paginated list of resort rooms with optional search functionality.
        /// </summary>
        /// <param name="pageNumber">Current page number for pagination</param>
        /// <param name="searchTerm">Search term entered by the user</param>
        /// <returns>View with paginated list of rooms</returns>
        public async Task<IActionResult> Roomlist(int pageNumber = 1, string searchTerm = null)
        {
            var loc = HttpContext.Session.GetString("Location");
            ViewBag.Location = loc;

            // Fetch the total count of rooms
            var count = await _roomService.GetTotalRoomCountAsync(searchTerm, loc);

            // Fetch rooms for the current page
            var rooms = await _roomService.GetRoomsAsync(pageNumber, searchTerm, loc);

            var roomViewModels = rooms.Select(r => new RoomViewModel
            {
                RoomId = r.RoomId,
                Descr = r.Descr,
                FeatureName = r.FeatureName ?? "N/A",
                TypeName = r.TypeName ?? "N/A",
                HourType = r.HourType,
                //RoomRate = r.RoomRate,
                StatusName = r.StatusName ?? "N/A",
                Classification = r.Classification,
                Remarks = r.Remarks,
                Location = r.Location
            }).ToList();

            // Create the paginated list
            var paginatedList = new PaginatedListViewModel<RoomViewModel>(
                roomViewModels, count, pageNumber, _appSettings.PageSize
            );

            // Preserve the search term in ViewData to use in pagination links
            ViewData["CurrentFilter"] = searchTerm;

            // Pass the paginated list to the view
            return View(paginatedList);
        }
        // Action method for handling search requests and displaying results
        [HttpGet]
        public async Task<IActionResult> Search(DateTime checkInDate, DateTime checkOutDate, int guests, string location)
        {
            //var location = HttpContext.Session.GetString("Location");
            try
            {
                var rooms = await _roomService.GetRoomAvailabilityAsync(location, checkInDate, checkOutDate);

                if (rooms == null || rooms.Count == 0)
                {
                    ViewBag.Message = "No rooms available for the selected criteria.";
                }

            // Pass the selected values to the ViewBag
            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;
            ViewBag.Guests = guests;
            ViewBag.Location = location;

            return View("Roomlist-Guest", rooms);
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the API call
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Search(string location, DateTime checkInDate, DateTime checkOutDate, int guests)
        {
            try
            {

                var rooms = await _roomService.GetRoomAvailabilityAsync(location, checkInDate, checkOutDate);

                if (rooms == null || rooms.Count == 0)
                {
                    ViewBag.Message = "No rooms available for the selected criteria.";
                }

                // Pass the selected values to the ViewBag
                ViewBag.CheckInDate = checkInDate;
                ViewBag.CheckOutDate = checkOutDate;
                ViewBag.Guests = guests;
                ViewBag.Location = location;

                return View("Roomlist-Guest", rooms);
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the API call
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> RoomdetailsGuest(string featureName, string typeName, DateTime checkInDate, 
                                                          DateTime checkOutDate, int guests, string location, int hourType = 12)
        {
            try
            {
                // Fetch the room details based on the featureName, typeName, and hourType
                var roomDetails = await _roomService.GetRoomDetailsAsync(featureName, typeName, hourType);

                if (roomDetails == null)
                {
                    return View("NotFound"); // You can create a custom "NotFound" view
                }

                // Fetch the images from RoomImages table
                var roomImages = await _context.RoomImages
                    .Where(ri => ri.FeatureName == featureName && ri.TypeName == typeName)
                    .FirstOrDefaultAsync();

                if (roomImages != null)
                {
                    roomDetails.ImagePath1 = roomImages.ImagePath1;
                    roomDetails.ImagePath2 = roomImages.ImagePath2;
                    roomDetails.ImagePath3 = roomImages.ImagePath3;
                    roomDetails.ImagePath4 = roomImages.ImagePath4;
                }

                // Pass the selected values to the ViewBag
                ViewBag.CheckInDate = checkInDate;
                ViewBag.CheckOutDate = checkOutDate;
                ViewBag.Guests = guests;
                ViewBag.Location = location; 

                // Return the Roomdetails-Guest view with the room details model
                return View("Roomdetails-Guest", roomDetails);
            }
            catch (HttpRequestException ex)
            {
                // Handle any errors that occurred during the HTTP request
                ViewBag.ErrorMessage = $"An error occurred while fetching room details: {ex.Message}";
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult UploadImages()
        {
            // Any logic you need before showing the view, e.g., passing data to the view
            // Set common data in ViewBag for the dropdown lists
            ViewBag.RoomFeatures = RoomData.RoomFeatures;
            ViewBag.RoomTypes = RoomData.RoomTypes;
            ViewBag.ResortPackages = RoomData.ResortPackages;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadImages(string FeatureName, string TypeName, IFormFile ImageFile1, IFormFile ImageFile2, IFormFile ImageFile3, IFormFile ImageFile4)
        {
            // Validate that all files are uploaded
            if (ImageFile1 == null || ImageFile2 == null || ImageFile3 == null || ImageFile4 == null)
            {
                ModelState.AddModelError(string.Empty, "All 4 images must be uploaded.");
                return View();
            }

            // Process and save the files, and store the paths in the database
            var imagePath1 = await SaveFile(ImageFile1);
            var imagePath2 = await SaveFile(ImageFile2);
            var imagePath3 = await SaveFile(ImageFile3);
            var imagePath4 = await SaveFile(ImageFile4);

            // Assuming you have a RoomImage model that corresponds to your database table
            var roomImage = new RoomImages
            {
                FeatureName = FeatureName,
                TypeName = TypeName,
                ImagePath1 = imagePath1,
                ImagePath2 = imagePath2,
                ImagePath3 = imagePath3,
                ImagePath4 = imagePath4,
            };

            _context.RoomImages.Add(roomImage);
            await _context.SaveChangesAsync();

            //return RedirectToAction("Success"); // Redirect to a success page or wherever you want
            //return View("EditRoomRate", roomRates);
            return RedirectToAction("Success", new { FeatureName, TypeName });

        }
        [HttpGet]
        public async Task<IActionResult> Success(string featureName, string typeName)
        {
            var roomImages = await _roomService.GetRoomImagesAsync(featureName, typeName);

            if (roomImages == null)
            {
                return NotFound("No images found for the specified room.");
            }

            var viewModel = new RoomImagesViewModel
            {
                FeatureName = featureName,
                TypeName = typeName,
                ImagePaths = new List<string>
            {
                roomImages.ImagePath1,
                roomImages.ImagePath2,
                roomImages.ImagePath3,
                roomImages.ImagePath4
            }
            };

            return View(viewModel);
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
        [HttpGet]
        public async Task<IActionResult> RoomImages(string featureName, string typeName)
        {
            var roomImages = await _roomService.GetRoomImagesAsync(featureName, typeName);

            if (roomImages == null)
            {
                return NotFound("No images found for the specified room.");
            }

            var viewModel = new RoomImagesViewModel
            {
                FeatureName = featureName,
                TypeName = typeName,
                ImagePaths = new List<string>
            {
                roomImages.ImagePath1,
                roomImages.ImagePath2,
                roomImages.ImagePath3,
                roomImages.ImagePath4
            }
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetRoomRate(string featureName, string typeName, int hourType)
        {
            var roomRate = await _roomService.GetRoomDetailsAsync(featureName, typeName, hourType);

            if (roomRate == null)
            {
                return NotFound();
            }

            return Json(roomRate.RoomRate);
        }

        #endregion

        #region Room CRUD Operations

        /// <summary>
        /// Displays the room creation page.
        /// </summary>
        /// <returns>View for creating a new room</returns>
        public async Task<IActionResult> Create()
        {
            // Generate the Room ID based on your desired format
            var nextRoomId = await _roomService.GenerateNextRoomIdAsync(); //IdGenerator.GenerateNextId(_context.Room, "ROM", r => r.RoomId);

            // Retrieve the location from session or another source
            var location = HttpContext.Session.GetString("Location");

            // Populate dropdown lists if needed
            PopulateDropdowns();

            // Pass the Room ID and Location to the view
            var room = new Room
            {
                RoomId = nextRoomId,
                Location = location // Set the location value
            };

            return View(room);
        }

        /// <summary>
        /// Handles the POST request for creating a new room.
        /// </summary>
        /// <param name="room">Room model containing room details</param>
        /// <returns>Redirects to Resort action if successful, otherwise reloads the creation page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                // Call the service to add the room
                var createdRoom = await _roomService.AddRoomAsync(room);

                if (createdRoom != null)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Room has been successfully created.";

                    // If the room is successfully created, redirect to the Resort action
                    return RedirectToAction(nameof(Roomlist));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create room. Please try again.");
                }
            }

            // Populate dropdown lists if needed
            PopulateDropdowns();

            return View(room);
        }

        /// <summary>
        /// Displays the room editing page.
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>View for editing the room details</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Populate dropdown lists if needed
            PopulateDropdowns();

            return View("RoomEdit", room);
        }

        /// <summary>
        /// Handles the POST request for editing an existing room.
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <param name="room">Room model containing updated room details</param>
        /// <returns>Redirects to Resort action if successful, otherwise reloads the edit page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _roomService.UpdateRoomAsync(room);
                if (result)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Room has been successfully updated.";

                    return RedirectToAction(nameof(Roomlist));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update room. Please try again.");
                }
            }

            // Populate dropdown lists if needed
            PopulateDropdowns();

            return View("RoomEdit", room);
        }

        #endregion

        #region Room Details

        /// <summary>
        /// Displays the room details page.
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>View with room details</returns>
        public async Task<IActionResult> RoomDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var client = new HttpClient();
            var response = await client.GetStringAsync($"{_baseUrl}/api/RoomApi/{id}");

            var room = JsonConvert.DeserializeObject<RoomViewModel>(response);

            return View(room);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Populates dropdown lists for room features, types, and statuses.
        /// </summary>
        private void PopulateDropdowns()
        {
            ViewBag.RoomFeatures = new SelectList(new[] { "Aircon", "Ceiling Fan" });
            ViewBag.RoomTypes = new SelectList(new[] { "Single", "Double" });
            ViewBag.RoomStatuses = new SelectList(new[] { "Available", "Occupied", "Maintenance" });
        }

        /// <summary>
        /// Checks if a room exists in the database.
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>Boolean indicating if the room exists</returns>
        private async Task<bool> RoomExists(string id)
        {
            return await _roomService.RoomExistsAsync(id);
        }

        #endregion
    }
}
