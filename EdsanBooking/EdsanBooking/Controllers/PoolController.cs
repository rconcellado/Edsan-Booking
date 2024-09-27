using EdsanBooking.Configuration;
using EdsanBooking.Data;
using EdsanBooking.Interface;
using EdsanBooking.Models;
using EdsanBooking.Services;
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
    public class PoolController : Controller
    {
        private readonly IPoolService _poolService;
        private readonly IReservationService _reservationService;
        private readonly ICheckInService _checkInService;
        public PoolController(IPoolService poolService, IReservationService reservationService, ICheckInService checkInService)
        {
            _poolService = poolService;
            _reservationService = reservationService;
            _checkInService = checkInService;
        }
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            try
            {
                var referenceId = "";
                var serviceType = "";

                if(id.StartsWith("RES"))
                {
                    var reservation = await _reservationService.GetReservationByIdAsync(id);
                    referenceId = reservation.ReservationId;
                    serviceType = reservation.ReservationType;
                }
                else
                {
                    var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                    referenceId = checkIn.CheckInId;
                    serviceType = checkIn.CheckInType;
                }

                var Details = await _poolService.GetDetailsAsync(referenceId);

                if(serviceType == "Transient")
                {
                    return View("TransientDetails", Details);
                }
                else
                {
                    return View("ResortDetails", Details);
                }
                
            }
            catch (HttpRequestException ex)
            {
                // Log and handle the exception as necessary
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}
