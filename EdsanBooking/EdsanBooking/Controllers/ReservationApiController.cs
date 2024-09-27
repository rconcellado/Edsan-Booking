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
    public class ReservationApiController : ControllerBase
    {
        #region Fields

        private readonly ReservationRepository _reservationRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationApiController"/> class.
        /// </summary>
        /// <param name="reservationRepository">The reservation repository.</param>
        public ReservationApiController(ReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Gets all reservations with optional pagination, search term, and location filtering.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="searchTerm">The search term to filter reservations.</param>
        /// <param name="loc">The location to filter reservations.</param>
        /// <returns>A list of reservations.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationViewModel>>> GetAllReservations(int skip = 0, int take = 12, string searchTerm = null, string loc = null)
        {
            var reservations = await _reservationRepository.GetReservationAsync(skip, take, searchTerm, loc);
            return Ok(reservations);
        }

        /// <summary>
        /// Gets a reservation by its ID.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>The reservation object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(string id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }
        #endregion

        #region Additional Operations

        /// <summary>
        /// Gets the total count of reservations based on the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to filter reservations.</param>
        /// <returns>The total count of reservations.</returns>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetReservationCount(string searchTerm = null)
        {
            var count = await _reservationRepository.GetTotalReservationCountAsync(searchTerm);
            return Ok(count);
        }

        /// <summary>
        /// Generates the next reservation ID.
        /// </summary>
        /// <returns>The next reservation ID as a JSON object.</returns>
        [HttpGet("GenerateNextReservationId")]
        public async Task<IActionResult> GenerateNextReservationId()
        {
            var nextReservationId = await _reservationRepository.GenerateNextReservationIdAsync();
            return Ok(new { ReservationId = nextReservationId });
        }

        /// <summary>
        /// Confirms a transient reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>A status code indicating success or failure.</returns>
        [HttpPost("ConfirmTransientReservation")]
        public async Task<ActionResult> ConfirmTransientReservationAsync([FromBody] string id)
        {
            var result = await _reservationRepository.ConfirmTransientReservationAsync(id);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Error confirming the reservation");
        }

        /// <summary>
        /// Confirms a resort reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>A status code indicating success or failure.</returns>
        [HttpPost("ConfirmResortReservation")]
        public async Task<ActionResult> ConfirmResortReservationAsync([FromBody] string id)
        {
            var result = await _reservationRepository.ConfirmResortReservationAsync(id);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Error confirming the reservation");
        }
        /// <summary>
        /// Creates a reservation with additional details.
        /// </summary>
        /// <param name="request">The reservation creation request object.</param>
        /// <returns>The created reservation object.</returns>
        [HttpPost("SaveOrUpdateReservationAsync")]
        public async Task<ActionResult<Reservation>> SaveOrUpdateReservationAsync([FromBody] CreateReservationRequest request)
        {
            try
            {
                var createdReservation = await _reservationRepository.SaveOrUpdateReservationAsync(
                    request.Reservation, request.FeatureName,
                    request.TypeName, request.HourType, request.pkgName, request.isNew);

                return Ok(createdReservation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpGet("GetGuestsByReservationId/{reservationId}")]
        public async Task<ActionResult<List<GuestDetailViewModel>>> GetGuestsByReservationId(string reservationId)
        {
            var guests = await _reservationRepository.GetGuestsByReservationIdAsync(reservationId);
            return Ok(guests);
        }
        [HttpDelete("DeleteComReservedRecords/{reservationId}")]
        public async Task<IActionResult> DeleteComReservedRecords(string reservationId)
        {
            if (string.IsNullOrEmpty(reservationId))
            {
                return BadRequest("Reservation ID is required.");
            }

            await _reservationRepository.DeleteComReservedRecordsAsync(reservationId);
            return Ok();
        }
        #endregion
    }
}
