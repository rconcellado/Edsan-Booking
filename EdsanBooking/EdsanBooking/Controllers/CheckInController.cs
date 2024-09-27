using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EdsanBooking.Controllers
{
    public class CheckInController : Controller
    {
        private readonly BookingContext _context;
        private readonly ICheckInService _checkInService;
        private readonly IGuestService _guestService;
        private readonly IChargeService _chargeService;
        private readonly IPaymentService _paymentService;
        //private readonly IPoolService _sharedService;
        private readonly AppSettings _appSettings;
        public CheckInController(BookingContext context, ICheckInService checkInService, IGuestService guestService,
                                 IChargeService chargeService, IPaymentService paymentService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _checkInService = checkInService;
            _guestService = guestService;
            _chargeService = chargeService;
            _paymentService = paymentService;
            //_sharedService = sharedService;
            _appSettings = appSettings.Value;
        }
        public async Task<IActionResult> CheckInList(int pageNumber = 1, string searchTerm = null)
        {
            var location = HttpContext.Session.GetString("Location");
            ViewBag.Location = location;

            var count = await _checkInService.GetTotalCheckInCountAsync(searchTerm, location);

            var checkInViewModel = await _checkInService.GetCheckInAsync(pageNumber, searchTerm, location);

            var paginatedList = new PaginatedListViewModel<CheckInViewModel>(
                checkInViewModel, count, pageNumber, _appSettings.PageSize
            );

            ViewData["CurrentFilter"] = searchTerm;
            return View(paginatedList);
        }
        public async Task<IActionResult> AddOrEdit(string id = null)
        {
            // Set common data in ViewBag for the dropdown lists
            ViewBag.RoomFeatures = RoomData.RoomFeatures;
            ViewBag.RoomTypes = RoomData.RoomTypes;
            ViewBag.ResortPackages = RoomData.ResortPackages;

            //CheckIn checkIn;
            TransientRes transientCheckIn = null;
            ResortRes resortCheckIn = null;

            bool isNew = string.IsNullOrEmpty(id);  // Determine if this is a new entry

            CheckIn checkIn;

            if (isNew)
            {
                // This is a create operation
                var nextCheckInId = await _checkInService.GenerateNextCheckInIdAsync();
                var currentUser = HttpContext.Session.GetString("Username");

                isNew = true;

                checkIn = new CheckIn
                {
                    CheckInId = nextCheckInId,
                    ReservationId = "N/A",
                    CheckInDt = DateTime.Now.Date,
                    CheckOutDt = DateTime.Now.Date.AddDays(1),
                    CreatedBy = currentUser,
                    CreatedDt = DateTime.Now
                };
            }
            else
            {
                checkIn = await _checkInService.GetCheckInByIdAsync(id);

                if(checkIn == null)
                {
                    return NotFound();
                }

                if (checkIn.CheckInType == "Transient")
                {
                    transientCheckIn = await _chargeService.GetTransientDetailsByIdAsync(id);
                }
                else
                {
                    resortCheckIn = await _chargeService.GetResortDetailsByIdAsync(id);
                }

                if (transientCheckIn == null)
                {
                    transientCheckIn = new TransientRes
                    {
                        FeatureName = "N/A",
                        TypeName = "N/A",
                        HourType = 0
                    };
                }

                if (resortCheckIn == null)
                {
                    resortCheckIn = new ResortRes
                    {
                        PkgName = "N/A"
                    };
                }
            }
            // Populate the view model with common properties
            var viewModel = new CheckInViewModel
            {
                ReservationId = "N/A",
                CheckInId = checkIn.CheckInId,
                GuestId = checkIn.GuestId,
                FirstName = "N/A",
                LastName = "N/A",
                Company = "N/A",
                GuestType = "N/A",
                ReservationType = "N/A",
                CheckInType = checkIn.CheckInType,
                NumGuest = checkIn.NumGuest,
                CheckInDt = checkIn.CheckInDt,
                CheckInTime = checkIn.CheckInTime,
                CheckOutDt = checkIn.CheckOutDt,
                CheckOutTime = checkIn.CheckOutTime,
                Status = checkIn.Status,
                Remarks = checkIn.Remarks,
                Location = checkIn.Location,
                CreatedBy = checkIn.CreatedBy,
                CreatedDt = checkIn.CreatedDt,
                FeatureName = transientCheckIn?.FeatureName ?? "N/A",
                TypeName = transientCheckIn?.TypeName ?? "N/A",
                HourType = transientCheckIn?.HourType ?? 0,
                PkgName = resortCheckIn?.PkgName ?? "N/A",
                bNew = isNew
            };
            return View("AddOrEdit", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(CheckInViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isNew = false;
            CheckIn checkIn = await _checkInService.GetCheckInByIdAsync(model.CheckInId);

            if (checkIn == null)
            {
                // It's a new entry, so generate a new CheckInId and mark as new
                isNew = true;
                checkIn = new CheckIn
                {
                    CheckInId = await _checkInService.GenerateNextCheckInIdAsync(),
                    CreatedBy = HttpContext.Session.GetString("Username"),
                    CreatedDt = DateTime.Now,
                    Location = HttpContext.Session.GetString("Location")
                };
            }

            // Populate the common properties
            checkIn.ReservationId = "N/A";
            checkIn.GuestId = model.GuestId;
            checkIn.CheckInType = model.CheckInType;
            checkIn.NumGuest = model.NumGuest;
            checkIn.CheckInDt = model.CheckInDt;
            checkIn.CheckInTime = model.CheckInTime;
            checkIn.CheckOutDt = model.CheckOutDt;
            checkIn.CheckOutTime = model.CheckOutTime;
            checkIn.Status = model.Status;
            checkIn.Remarks = model.Remarks;

            // Save or update the check-in
            try
            {
                var result = await _checkInService.SaveOrUpdateCheckInAsync(
                    checkIn, model.FeatureName, model.TypeName, model.HourType, model.PkgName, isNew);

                if (result != null)
                {
                    TempData["SuccessMessage"] = isNew ? "CheckIn has been successfully created." : "CheckIn has been successfully updated.";
                    //return RedirectToAction(isNew ? "AddGuests" : "EditGuests", new { id = checkIn.CheckInId, numGuests = checkIn.NumGuest });
                    return RedirectToAction("AddOrEditGuests", new { id = checkIn.CheckInId, numGuests = checkIn.NumGuest });
                }
                else
                {
                    ModelState.AddModelError("", isNew ? "Unable to create check-in. Please try again." : "Unable to update check-in. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while processing the check-in: {ex.Message}");
            }

            return View(model);
        }
        public async Task<IActionResult> AddOrEditGuests(string id, int numGuests)
        {
            if (string.IsNullOrEmpty(id) || numGuests <= 0)
            {
                ModelState.AddModelError("NumGuests", "Number of guests must be at least 1.");
                return View(new AddGuestsViewModel());
            }

            var checkIn = await _checkInService.GetCheckInByIdAsync(id);

            var viewModel = new AddGuestsViewModel
            {
                ReservationId = "N/A",
                CheckInId = id,
                NumGuests = numGuests,
                GuestDetails = new List<GuestDetailViewModel>()
            };

            if (checkIn != null)
            {
                // Edit scenario
                var guests = await _checkInService.GetGuestsByCheckInIdAsync(id);
                viewModel.GuestDetails = guests.ToList();

                for (int i = viewModel.GuestDetails.Count; i < numGuests; i++)
                {
                    viewModel.GuestDetails.Add(new GuestDetailViewModel());
                }

                ViewData["Title"] = "Edit Guests";
                ViewData["Action"] = "EditGuests";
            }
            else
            {
                // Add scenario
                viewModel.GuestDetails = Enumerable.Range(0, numGuests)
                                                   .Select(i => new GuestDetailViewModel())
                                                   .ToList();

                ViewData["Title"] = "Add Guests";
                ViewData["Action"] = "AddGuests";
            }

            return View("AddOrEditGuests", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditGuests(AddGuestsViewModel model, [FromRoute] string id)
        {
            // Determine if the operation is for adding or editing based on whether the check-in ID exists
            //bool isEdit = !string.IsNullOrEmpty(id) && id == model.CheckInId;

            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the view with the current model to display validation errors
                return View(model);
            }

            var referenceId = model.CheckInId ?? model.ReservationId;

            try
            {
                //if (isEdit)
                //{
                    // If editing, delete existing room check-in records
                await _checkInService.DeleteRoomCheckInRecordsAsync(id);
                //}

                // Save or update guest details
                foreach (var guestDetail in model.GuestDetails)
                {
                    await _guestService.SaveOrUpdateGuestAsync(referenceId, guestDetail);
                }

                // Update room status
                var success = await _checkInService.UpdateRoomStatusAsync(referenceId);
                if (!success)
                {
                    return StatusCode(500, "Failed to update room status.");
                }

                // Add room charges
                await _paymentService.AddRoomChargeAsync(referenceId);

                TempData["SuccessMessage"] = "Guests have been successfully added.";

                // Redirect to the next step, such as entering payment details
                return RedirectToAction("EnterPayment", "Payment", new { id = referenceId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while processing guests: {ex.Message}");
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmCheckin(string id)
        {
            try
            {
                var success = await _checkInService.ConfirmCheckInAsync(id);
                if (!success) 
                {
                    TempData["ErrorMessage"] = "Number of guests does not match the number of checked-in rooms.";
                    return RedirectToAction("Checkinlist", "CheckIn", new { id = id });
                }
                TempData["SuccessMessage"] = "Check-in confirmed successfully.";
                return RedirectToAction("Checkinlist", "CheckIn", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout(string id)
        {
            try
            {
                var success = await _checkInService.ConfirmCheckOutAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "An error occurred while confirming the check-out.";
                    return RedirectToAction("Checkinlist", "CheckIn", new { id = id });
                }
                TempData["SuccessMessage"] = "Check-out confirmed successfully.";
                return RedirectToAction("Checkinlist", "CheckIn", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
        public async Task<IActionResult> SearchRooms(string checkInType, string statusName, string featureName, string typeName, int hourType, int pageNumber = 1, int pageSize = 5)
        {
            var query = _context.Room
                .Where(r => r.Classification == checkInType && r.StatusName == statusName)
                .Where(r => r.FeatureName == featureName && r.TypeName == typeName && r.HourType == hourType);

            var count = await query.CountAsync();
            var rooms = await query
                .OrderBy(r => r.RoomId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedList = new PaginatedListViewModel<Room>(rooms, count, pageNumber, pageSize);

            return PartialView("_RoomSearchResults", paginatedList);
        }
        public async Task<IActionResult> SearchGuests(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Guest.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(g => g.GuestId.Contains(searchQuery) || g.GName.Contains(searchQuery) ||
                                         g.LName.Contains(searchQuery) || g.Company.Contains(searchQuery));
            }

            var totalCount = await query.CountAsync();
            var guests = await query
                .OrderBy(g => g.GuestId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedGuests = new PaginatedListViewModel<Guest>(guests, totalCount, pageNumber, pageSize);

            return PartialView("_GuestSearchResults", paginatedGuests);
        }

    }
}
