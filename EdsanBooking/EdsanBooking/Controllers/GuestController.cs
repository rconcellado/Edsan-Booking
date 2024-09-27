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
using Microsoft.AspNetCore.Authorization;

namespace EdsanBooking.Controllers
{
    /// <summary>
    /// Controller for managing guests in the application.
    /// </summary>
    public class GuestController : Controller
    {
        #region Private Fields

        private readonly BookingContext _context;
        private readonly IGuestService _guestService;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GuestController"/> class.
        /// </summary>
        /// <param name="guestService">The service for managing guests.</param>
        /// <param name="context">The database context.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="appSettings">The application settings.</param>
        public GuestController(IGuestService guestService, BookingContext context, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _guestService = guestService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _baseUrl = configuration["BaseUrl"];
            _appSettings = appSettings.Value;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Displays a paginated list of guests.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="searchTerm">The search term to filter guests.</param>
        /// <returns>The view with the list of guests.</returns>
        public async Task<IActionResult> GuestList(int pageNumber = 1, string searchTerm = null)
        {
            var count = await _guestService.GetTotalGuestCountAsync(searchTerm);
            var guests = await _guestService.GetGuestAsync(pageNumber, searchTerm);

            var guestViewModels = guests.Select(r => new GuestViewModel
            {
                GuestId = r.GuestId,
                Company = r.Company,
                FirstName = r.GName,
                LastName = r.LName,
                ContactNo = r.ContactNo,
                Address = r.Address,
                GuestType = r.GuestType
            }).ToList();

            var paginatedList = new PaginatedListViewModel<GuestViewModel>(
                guestViewModels, count, pageNumber, _appSettings.PageSize
            );

            ViewData["CurrentFilter"] = searchTerm;

            return View(paginatedList);
        }

        /// <summary>
        /// Displays the form to create a new guest.
        /// </summary>
        /// <returns>The view to create a new guest.</returns>
        public IActionResult Create()
        {
            var nextGuestId = IdGenerator.GenerateNextId(_context.Guest, "GUE", r => r.GuestId);

            PopulateDropdowns();

            var guest = new Guest { GuestId = nextGuestId };

            return View(guest);
        }

        /// <summary>
        /// Handles the submission of the create guest form.
        /// </summary>
        /// <param name="guest">The guest to create.</param>
        /// <returns>The result of the create operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guest guest)
        {
            if (ModelState.IsValid)
            {
                var createdGuest = await _guestService.AddGuestAsync(guest);

                if (createdGuest != null)
                {
                    TempData["SuccessMessage"] = "Guest has been successfully created.";
                    return RedirectToAction(nameof(GuestList));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create guest. Please try again.");
                }
            }
            PopulateDropdowns();

            return View(guest);
        }

        /// <summary>
        /// Displays the form to edit an existing guest.
        /// </summary>
        /// <param name="id">The ID of the guest to edit.</param>
        /// <returns>The view to edit the guest.</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var guest = await _guestService.GetGuestByIdAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            PopulateDropdowns();

            return View("GuestEdit", guest);
        }

        /// <summary>
        /// Handles the submission of the edit guest form.
        /// </summary>
        /// <param name="id">The ID of the guest to edit.</param>
        /// <param name="guest">The updated guest details.</param>
        /// <returns>The result of the edit operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Guest guest)
        {
            if (id != guest.GuestId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var result = await _guestService.UpdateGuestAsync(guest);
                if (result)
                {
                    TempData["SuccessMessage"] = "Guest has been successfully updated.";
                    return RedirectToAction(nameof(GuestList));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update guest. Please try again.");
                }
            }
            PopulateDropdowns();

            return View("GuestEdit", guest);
        }

        /// <summary>
        /// Displays the details of a specific guest.
        /// </summary>
        /// <param name="id">The ID of the guest to view details for.</param>
        /// <returns>The view with the guest details.</returns>
        public async Task<IActionResult> GuestDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var client = new HttpClient();
            var response = await client.GetStringAsync($"{_baseUrl}/api/GuestApi/{id}");

            var guest = JsonConvert.DeserializeObject<GuestViewModel>(response);

            return View(guest);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populates the dropdown lists used in the guest forms.
        /// </summary>
        private void PopulateDropdowns()
        {
            ViewBag.GuestTypes = new SelectList(new[] { "Individual", "Company" });
        }

        /// <summary>
        /// Checks if a guest with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the guest to check.</param>
        /// <returns>True if the guest exists, otherwise false.</returns>
        private async Task<bool> GuestExists(string id)
        {
            return await _guestService.GuestExistsAsync(id);
        }

        #endregion
    }
}
