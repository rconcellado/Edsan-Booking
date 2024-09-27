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
using EdsanBooking.Services;

namespace EdsanBooking.Controllers
{
    public class SettingController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;
        public SettingController(ISettingService settingService, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _settingService = settingService;
            _baseUrl = configuration["BaseUrl"];
            _appSettings = appSettings.Value;
        }
        public async Task<IActionResult> RoomRatesList(int pageNumber = 1, string searchTerm = null)
        {
            var location = HttpContext.Session.GetString("Location");
            ViewBag.Location = location;

            var count = await _settingService.GetTotalRoomRatesCountAsync(searchTerm);

            var roomrates = await _settingService.GetAllRoomRatesAsync(pageNumber, count, location, searchTerm);

            var roomrateviewModel = roomrates.Select(r => new RoomRateViewModel
            {
                Id = r.ID,
                FeatureName = r.featureName,
                TypeName = r.typeName,
                HourType = r.hourType,
                RoomRate = r.RoomRate,
                Classification = r.Classification
            }).ToList();

            var paginatedList = new PaginatedListViewModel<RoomRateViewModel>(
                roomrateviewModel, count, pageNumber, _appSettings.PageSize
            );

            ViewData["CurrentFilter"] = searchTerm;

            return View(paginatedList);
        }
        // GET: Setting/EditRoomRate/{id}
        public async Task<IActionResult> EditRoomRate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var roomRate = await _settingService.GetRoomRatesByIdAsync(id);
            if (roomRate == null)
            {
                return NotFound();
            }


            //var viewModel = new RoomRateViewModel
            //{
            //    Id = roomRate.ID,
            //    FeatureName = roomRate.featureName,
            //    TypeName = roomRate.typeName,
            //    HourType = roomRate.hourType,
            //    RoomRate = roomRate.RoomRate,
            //    Classification = roomRate.Classification
            //};

            return View("EditRoomRate", roomRate);
        }
        // POST: Setting/SaveRoomRate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoomRate(string id, RoomRates roomRates)
        {
            if(id != roomRates.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _settingService.UpdateRoomRateAsync(roomRates);
                return RedirectToAction("RoomRatesList");
            }

            return View("EditRoomRate", roomRates);
        }


    }
}
