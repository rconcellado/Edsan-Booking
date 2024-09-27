using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.X86;
using EdsanBooking.Interface;
using EdsanBooking.Repositories;

namespace EdsanBooking.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public CheckInService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task<int> GetTotalCheckInCountAsync(string searchTerm = null, string loc = null)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/CheckInApi/count?searchTerm={searchTerm}&loc={loc}");
            return JsonConvert.DeserializeObject<int>(response);
        }

        public async Task<List<CheckInViewModel>> GetCheckInAsync(int pageNumber, string searchTerm = null, string loc = null)
        {
            var pageSize = 12; // You can store this in appsettings.json for easy configuration
            int skip = (pageNumber - 1) * pageSize;
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/CheckInApi?skip={skip}&take={pageSize}&searchTerm={searchTerm}&loc={loc}");
            return JsonConvert.DeserializeObject<List<CheckInViewModel>>(response);
        }
        public async Task<CheckIn> GetCheckInByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/CheckInApi/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // If the CheckIn ID is not found, return null
                    return null;
                }

                response.EnsureSuccessStatusCode(); // Throw an exception if the response indicates an error

                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CheckIn>(responseData);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new HttpRequestException($"An error occurred while fetching the check-in details: {ex.Message}", ex);
            }
        }

        public async Task<CheckIn> SaveOrUpdateCheckInAsync(CheckIn checkIn, string featureName, string typeName,
                                         int hourType, string pkgName, bool isNew)
        {
            var request = new
            {
                CheckIn = checkIn,
                FeatureName = featureName,
                TypeName = typeName,
                HourType = hourType,
                pkgName = pkgName,
                isNew = isNew
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/CheckInApi/SaveOrUpdateCheckInAsync", request);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API error during SaveOrUpdateCheckInAsync: {response.StatusCode}, Content: {content}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var createdCheckIn = await System.Text.Json.JsonSerializer.DeserializeAsync<CheckIn>(contentStream);

            return createdCheckIn;
        }
        public async Task<List<GuestDetailViewModel>> GetGuestsByCheckInIdAsync(string checkInid)
        {
            var response = await _httpClient.GetAsync($"/api/CheckInApi/GetGuestsByCheckInId/{checkInid}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var guests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GuestDetailViewModel>>(jsonResponse);

            return guests;
        }
        public async Task<string> GenerateNextCheckInIdAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/CheckInApi/GenerateNextCheckInId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return result.checkInId;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to generate next Reservation ID. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
        public async Task DeleteRoomCheckInRecordsAsync(string checkInId)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/CheckInApi/DeleteRoomCheckInRecords/{checkInId}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<bool> UpdateRoomStatusAsync(string checkInId)
        {
            if (string.IsNullOrEmpty(checkInId))
            {
                throw new ArgumentException("CheckInId cannot be null or empty.", nameof(checkInId));
            }

            // Send the CheckInId as the body of the POST request
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/CheckInApi/UpdateRoomStatusAsync", checkInId);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to update room status. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
        public async Task<bool> ConfirmCheckInAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("CheckInId cannot be null or empty.", nameof(id));
            }

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/CheckInApi/ConfirmCheckInAsync", id);

            if (response.IsSuccessStatusCode) 
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to confirm checkIn status. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
        public async Task<bool> ConfirmCheckOutAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("CheckInId cannot be null or empty.", nameof(id));
            }

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/CheckInApi/ConfirmCheckOutAsync", id);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to confirm checkOut status. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }



    }
}

