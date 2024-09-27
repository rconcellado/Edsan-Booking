using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Services;
using EdsanBooking.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EdsanBooking.Controllers
{
    /// <summary>
    /// Controller for managing reservations.
    /// </summary>
    public class ReservationController : Controller
    {
        #region Fields

        private readonly BookingContext _context;
        private readonly IReservationService _reservationService;
        private readonly IGuestService _guestService;
        private readonly IChargeService _chargeService;
        private readonly IPaymentService _paymentService;
        private readonly AppSettings _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationController"/> class.
        /// </summary>
        /// <param name="reservationService">The reservation service.</param>
        /// <param name="guestService">The guest service.</param>
        /// <param name="context">The database context.</param>
        /// <param name="appSettings">The application settings.</param>
        public ReservationController(IReservationService reservationService, IGuestService guestService, IChargeService chargeService,
                                     IPaymentService paymentService, BookingContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _reservationService = reservationService;
            _guestService = guestService;
            _chargeService = chargeService;
            _paymentService = paymentService;
            _appSettings = appSettings.Value;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Displays a list of reservations with pagination and search functionality.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>The reservation list view.</returns>
        public async Task<IActionResult> ReservationList(int pageNumber = 1, string searchTerm = null)
        {
            var location = HttpContext.Session.GetString("Location");
            ViewBag.Location = location;

            var count = await _reservationService.GetTotalReservationCountAsync(searchTerm, location);
            var reservationViewModels = await _reservationService.GetReservationsAsync(pageNumber, searchTerm, location);
            var paginatedList = new PaginatedListViewModel<ReservationViewModel>(
                reservationViewModels, count, pageNumber, _appSettings.PageSize
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

            TransientRes transientRes = null;
            ResortRes resortRes = null;

            bool isNew = string.IsNullOrEmpty(id);  // Determine if this is a new entry

            Reservation reservation;

            if (isNew)
            {
                // Create a new reservation
                var nextReservationId = await _reservationService.GenerateNextReservationIdAsync();
                var currentUser = HttpContext.Session.GetString("Username");

                reservation = new Reservation
                {
                    ReservationId = nextReservationId,
                    CheckInDt = DateTime.Now.Date,
                    CheckOutDt = DateTime.Now.Date.AddDays(1),
                    CreatedBy = currentUser,
                    CreatedDt = DateTime.UtcNow
                };

                // Initialize transientRes and resortRes to default values if it's a new entry
                transientRes = new TransientRes
                {
                    FeatureName = "N/A",
                    TypeName = "N/A",
                    HourType = 0
                };

                resortRes = new ResortRes
                {
                    PkgName = "N/A"
                };
            }
            else
            {
                // Fetch the existing reservation for editing
                reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                {
                    return NotFound();  // Handle the case where the reservation doesn't exist
                }

                // Load the relevant details based on the reservation type
                if (reservation.ReservationType == "Transient")
                {
                    transientRes = await _chargeService.GetTransientDetailsByIdAsync(id) ?? new TransientRes
                    {
                        FeatureName = "N/A",
                        TypeName = "N/A",
                        HourType = 0
                    };
                }
                else
                {
                    resortRes = await _chargeService.GetResortDetailsByIdAsync(id) ?? new ResortRes
                    {
                        PkgName = "N/A"
                    };
                }
            }

            var viewModel = new CheckInViewModel
            {
                ReservationId = reservation.ReservationId,
                CheckInId = "N/A",
                GuestId = reservation.GuestId,
                FirstName = "N/A",
                LastName = "N/A",
                Company = "N/A",
                ReservationType = reservation.ReservationType,
                CheckInType = "N/A",
                NumGuest = reservation.NumGuest,
                CheckInDt = reservation.CheckInDt,
                CheckInTime = reservation.CheckInTime,
                CheckOutDt = reservation.CheckOutDt,
                CheckOutTime = reservation.CheckOutTime,
                Status = reservation.Status,
                Remarks = reservation.Remarks,
                Location = reservation.Location,
                CreatedBy = reservation.CreatedBy,
                CreatedDt = reservation.CreatedDt,
                FeatureName = transientRes?.FeatureName ?? "N/A",
                TypeName = transientRes?.TypeName ?? "N/A",
                HourType = transientRes?.HourType ?? 0,
                PkgName = resortRes?.PkgName ?? "N/A",
                GuestType = "N/A",
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
            Reservation reservation = await _reservationService.GetReservationByIdAsync(model.ReservationId);

            if(reservation == null)
            {
                isNew = true;
                reservation = new Reservation
                {
                    ReservationId = await _reservationService.GenerateNextReservationIdAsync(),
                    CreatedBy = HttpContext.Session.GetString("Username"),
                    CreatedDt = DateTime.Now,
                    Location = HttpContext.Session.GetString("Location")
                };
            }

            // Populate the common properties
            reservation.GuestId = model.GuestId;
            reservation.ReservationType = model.ReservationType;
            reservation.NumGuest = model.NumGuest;
            reservation.CheckInDt = model.CheckInDt;
            reservation.CheckInTime = model.CheckInTime;
            reservation.CheckOutDt = model.CheckOutDt;
            reservation.CheckOutTime = model.CheckOutTime;
            reservation.Status = model.Status;
            reservation.Remarks = model.Remarks;

            try
            {
                var result = await _reservationService.SaveOrUpdateReservationAsync(
                    reservation, model.FeatureName, model.TypeName, model.HourType, model.PkgName, isNew);

                if(result != null)
                {
                    TempData["SuccessMessage"] = "Reservation has been successfully created.";
                    return RedirectToAction("AddOrEditGuests", new { id = reservation.ReservationId, numGuests = reservation.NumGuest });
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create reservation. Please try again.");
                }
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", $"An error occurred while creating the reservation: {ex.Message}");
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

            var reservation = await _reservationService.GetReservationByIdAsync(id);

            var viewModel = new AddGuestsViewModel
            {
                ReservationId = id,
                CheckInId = "N/A",
                NumGuests = numGuests,
                GuestDetails = new List<GuestDetailViewModel>()
            };

            if(reservation != null)
            {
                var guests = await _reservationService.GetGuestsByReservationIdAsync(id);
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

            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the view with the current model to display validation errors
                return View(model);
            }

            var referenceId = model.ReservationId ?? model.CheckInId;

            try
            {
                // Delete records from comreserved table
                await _reservationService.DeleteComReservedRecordsAsync(id);

                foreach (var guestDetail in model.GuestDetails)
                {
                    await _guestService.SaveOrUpdateGuestAsync(id, guestDetail);
                }

                // Add room charges
                await _paymentService.AddRoomChargeAsync(referenceId);

                TempData["SuccessMessage"] = "Guests have been successfully added.";
                return RedirectToAction("EnterPayment", "Payment", new { id = referenceId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while processing guests: {ex.Message}");
                return View(model);
            }
        }
        /// <summary>
        /// Handles the confirmation of a transient reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>Redirects to the reservation list view.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmTransientReservation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Reservation ID is required.");
            }

            var result = await _reservationService.ConfirmTransientReservationAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Reservation confirmed successfully.";
                return RedirectToAction(nameof(ReservationList));
            }
            TempData["ErrorMessage"] = "Error confirming the reservation.";
            return RedirectToAction(nameof(ReservationList));
        }

        /// <summary>
        /// Handles the confirmation of a resort reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>Redirects to the reservation list view.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmResortReservation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Reservation ID is required.");
            }

            var result = await _reservationService.ConfirmResortReservationAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Reservation confirmed successfully.";
                return RedirectToAction(nameof(ReservationList));
            }
            TempData["ErrorMessage"] = "Error confirming the reservation.";
            return RedirectToAction(nameof(ReservationList));
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
        [HttpPost]
        public async Task<IActionResult> Register(RoomDetailsViewModel model, string Location,
                                          DateTime checkInDate, DateTime checkOutDate, int guests)
        {

            var firstName = model.FirstName;

            var lastName = model.LastName;

            var contactNo = model.ContactNo;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(contactNo))
            {
                return BadRequest("Guest Name && Contact No is required.");
            }

            var guestId = await _guestService.GetGuestIdByFirstNameLastNameAsync(firstName, lastName);

            if (guestId == null)
            {
                guestId = await _guestService.GenerateNextGuestIdAsync();

                var guest = new Guest
                {
                    GuestId = guestId,
                    GName = firstName,
                    LName = lastName,
                    ContactNo = contactNo,
                    Address = "N/A",
                    GuestType = "Individual",
                    Company = "Not applicable"
                };

                await _guestService.AddGuestAsync(guest);
            }

            //var guestId = await _guestService.GetGuestIdByUserNameAsync(username);

            ModelState.Clear();
            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var reservation = new Reservation
            {
                ReservationId = await _reservationService.GenerateNextReservationIdAsync(),
                CreatedBy = "SYSTEM",
                CreatedDt = DateTime.Now,
                Location = Location,
                GuestId = guestId,
                ReservationType = "Transient",
                NumGuest = guests,
                CheckInDt = checkInDate,
                CheckInTime = new TimeSpan(0, 0, 0), // 12:00 AM
                CheckOutDt = checkOutDate,
                CheckOutTime = new TimeSpan(0, 0, 0), // 12:00 AM
                Status = "Pending",
                Remarks = "N/A"
            };

            try
            {
                var result = _reservationService.SaveOrUpdateReservationAsync(
                    reservation, model.FeatureName, model.TypeName, model.HourType, "", true);

                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Reservation No. {reservation.ReservationId} has been successfully created.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create reservation. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the reservation: {ex.Message}");
            }
            return View(model);
        }

        #endregion
    }
}
