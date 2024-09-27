using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using EdsanBooking.Repositories;
using System.Text.Json;
using EdsanBooking.Interface;

namespace EdsanBooking.Services
{
    /// <summary>
    /// Service class for handling reservation-related operations.
    /// </summary>
    public class ReservationService : IReservationService
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configuration">The configuration settings.</param>
        public ReservationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the total number of reservations based on search term and location.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="loc">The location.</param>
        /// <returns>The total reservation count.</returns>
        public async Task<int> GetTotalReservationCountAsync(string searchTerm = null, string loc = null)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/ReservationApi/count?searchTerm={searchTerm}&loc={loc}");
            return JsonConvert.DeserializeObject<int>(response);
        }

        /// <summary>
        /// Gets a paginated list of reservations.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="loc">The location.</param>
        /// <returns>A list of reservation view models.</returns>
        public async Task<List<ReservationViewModel>> GetReservationsAsync(int pageNumber, string searchTerm = null, string loc = null)
        {
            var pageSize = 12; // You can store this in appsettings.json for easy configuration
            int skip = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/ReservationApi?skip={skip}&take={pageSize}&searchTerm={searchTerm}&loc={loc}");
            return JsonConvert.DeserializeObject<List<ReservationViewModel>>(response);
        }

        /// <summary>
        /// Gets a reservation by its ID.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>The reservation object.</returns>
        public async Task<Reservation> GetReservationByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/ReservationApi/{id}");

                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + responseData);  // Log the raw JSON response
                return JsonConvert.DeserializeObject<Reservation>(responseData);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new HttpRequestException($"An error occurred while fetching the reservation details: {ex.Message}", ex);
            }

        }

        /// <summary>
        /// Checks if a reservation exists by its ID.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>A boolean indicating whether the reservation exists.</returns>
        public async Task<bool> ReservationExistsAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/ReservationApi/{id}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Generates the next reservation ID.
        /// </summary>
        /// <returns>The next reservation ID as a string.</returns>
        public async Task<string> GenerateNextReservationIdAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/ReservationApi/GenerateNextReservationId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return result.reservationId;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to generate next Reservation ID. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }

        /// <summary>
        /// Confirms a transient reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        public async Task<bool> ConfirmTransientReservationAsync(string id)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ReservationApi/ConfirmTransientReservation", id);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Confirms a resort reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>A boolean indicating success or failure.</returns>
        public async Task<bool> ConfirmResortReservationAsync(string id)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ReservationApi/ConfirmResortReservation", id);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets the total amount for a transient reservation.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <param name="location">The location.</param>
        /// <returns>The total amount as a decimal.</returns>
        public async Task<decimal> GetTransientReservationTotalAmountAsync(string id, string location)
        {
            var request = new
            {
                Id = id,
                Location = location
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ReservationApi/CalculateTransientReservationTotalAmountAsync", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return (decimal)result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to get total amount. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }

        /// <summary>
        /// Creates a new reservation with additional details.
        /// </summary>
        /// <param name="reservation">The reservation object.</param>
        /// <param name="featureName">The feature name.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="hourType">The hour type.</param>
        /// <param name="ResortPackageId">The resort package ID.</param>
        /// <returns>The created reservation.</returns>
        public async Task<Reservation> SaveOrUpdateReservationAsync(Reservation reservation, string featureName, string typeName,
                                         int hourType, string pkgName, bool isNew)
        {
            var request = new
            {
                Reservation = reservation,
                FeatureName = featureName,
                TypeName = typeName,
                HourType = hourType,
                pkgName = pkgName,
                isNew = isNew
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ReservationApi/SaveOrUpdateReservationAsync", request);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API error during AddReservationAsync: {response.StatusCode}, Content: {content}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var createdReservation = await System.Text.Json.JsonSerializer.DeserializeAsync<Reservation>(contentStream);

            return createdReservation;
        }
        public async Task<List<GuestDetailViewModel>> GetGuestsByReservationIdAsync(string reservationId)
        {
            var response = await _httpClient.GetAsync($"/api/ReservationApi/GetGuestsByReservationId/{reservationId}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var guests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GuestDetailViewModel>>(jsonResponse);

            return guests;
        }
        public async Task DeleteComReservedRecordsAsync(string reservationId)
        {
            var response = await _httpClient.DeleteAsync($"/api/ReservationApi/DeleteComReservedRecords/{reservationId}");
            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}
