using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using EdsanBooking.Interface;
using EdsanBooking.Repositories;

namespace EdsanBooking.Services
{
    public class RoomService : IRoomService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;

        public RoomService(HttpClient httpClient, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"]; // Get the base URL from configuration
            _appSettings = appSettings.Value;
        }

        public async Task<int> GetTotalRoomCountAsync(string searchTerm = null, string loc = null)
        {
            var url = $"{_baseUrl}/api/RoomApi/count";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"?searchTerm={searchTerm}&loc={loc}";
            }
            else
            {
                url += $"?loc={loc}";
            }

            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<int>(response);
        }

        public async Task<List<Room>> GetRoomsAsync(int pageNumber, string searchTerm = null, string loc = null)
        {
            int pageSize = _appSettings.PageSize; // Number of records per page
            int skip = (pageNumber - 1) * pageSize; // Calculate how many records to skip

            var url = $"{_baseUrl}/api/RoomApi?skip={skip}&take={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"&searchTerm={searchTerm}&loc={loc}";
            }
            else
            {
                url += $"&loc={loc}";
            }

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Log the error details
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GetRoomsAsync: {response.StatusCode}, Content: {content}");

                // You can either throw an exception or return an empty list
                // throw new Exception("Failed to retrieve rooms from API");
                return new List<Room>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Room>>(jsonString);
        }
        public async Task<List<RoomAvailabilityGroupedViewModel>> GetRoomAvailabilityAsync(string location, DateTime checkInDate, DateTime checkOutDate)
        {
            // Format dates as ISO 8601
            var fromDateString = checkInDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = checkOutDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var url = $"{_baseUrl}/api/RoomApi/GetRoomAvailability?location={Uri.EscapeDataString(location)}&checkInDate={Uri.EscapeDataString(fromDateString)}&checkOutDate={Uri.EscapeDataString(toDateString)}";


            // Call the API controller using HttpClient
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RoomAvailabilityGroupedViewModel>>(content);
            }
            else
            {
                // Handle the error as needed
                throw new HttpRequestException($"Failed to retrieve room availability. StatusCode: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<RoomDetailsViewModel> GetRoomDetailsAsync(string featureName, string typeName, int hourType)
        {
            var url = $"{_baseUrl}/api/RoomApi/GetRoomDetails?featureName={Uri.EscapeDataString(featureName)}&typeName={Uri.EscapeDataString(typeName)}&hourType={hourType}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RoomDetailsViewModel>(content);
            }
            else
            {
                // Handle error response
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve room details. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
        public async Task<string> UploadRoomImagesAsync(string featureName, string typeName, IFormFile imageFile1, IFormFile imageFile2, IFormFile imageFile3, IFormFile imageFile4)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(featureName), "featureName");
            form.Add(new StringContent(typeName), "typeName");

            form.Add(new StreamContent(imageFile1.OpenReadStream()), "imageFile1", imageFile1.FileName);
            form.Add(new StreamContent(imageFile2.OpenReadStream()), "imageFile2", imageFile2.FileName);
            form.Add(new StreamContent(imageFile3.OpenReadStream()), "imageFile3", imageFile3.FileName);
            form.Add(new StreamContent(imageFile4.OpenReadStream()), "imageFile4", imageFile4.FileName);

            var response = await _httpClient.PostAsync("/api/RoomApi/upload", form);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<RoomImages> GetRoomImagesAsync(string featureName, string typeName)
        {
            var response = await _httpClient.GetAsync($"/api/RoomApi/images?featureName={featureName}&typeName={typeName}");
            response.EnsureSuccessStatusCode();

            // Use ReadFromJsonAsync instead of ReadAsAsync
            return await response.Content.ReadFromJsonAsync<RoomImages>();
        }
        public async Task<Room> GetRoomByIdAsync(string id)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/RoomApi/{id}");
            return JsonConvert.DeserializeObject<Room>(response);
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/RoomApi");
            return JsonConvert.DeserializeObject<List<Room>>(response);
        }

        public async Task<Room> AddRoomAsync(Room room)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/RoomApi", room);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during AddRoomAsync: {response.StatusCode}, Content: {content}");
                return null; // Return null if there's an error
            }

            var createdRoom = JsonConvert.DeserializeObject<Room>(await response.Content.ReadAsStringAsync());
            return createdRoom; // Return the created Room object
        }



        public async Task<bool> UpdateRoomAsync(Room room)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/RoomApi/{room.RoomId}", room);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during UpdateRoomAsync: {response.StatusCode}, Content: {content}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRoomAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/RoomApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during DeleteRoomAsync: {response.StatusCode}, Content: {content}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RoomExistsAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/RoomApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during RoomExistsAsync: {response.StatusCode}, Content: {content}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<string> GenerateNextRoomIdAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/RoomApi/GenerateNextRoomId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return result.roomId;  // Access the GuestId property
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GenerateNextRoomId: {response.StatusCode}, Content: {errorContent}");

                throw new HttpRequestException($"Failed to generate next Room ID. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
    }
}
