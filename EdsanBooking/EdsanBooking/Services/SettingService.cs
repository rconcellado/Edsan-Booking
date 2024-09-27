using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using EdsanBooking.Interface;

namespace EdsanBooking.Services
{
    public class SettingService : ISettingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;
        public SettingService(HttpClient httpClient, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"]; // Get the base URL from configuration
            _appSettings = appSettings.Value;
        }
        public async Task<int> GetTotalRoomRatesCountAsync(string searchTerm = null)
        {
            var url = $"{_baseUrl}/api/SettingApi/count";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"?searchTerm={searchTerm}";
            }

            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<int>(response);
        }
        public async Task<List<RoomRates>> GetAllRoomRatesAsync(int pageNumber, int take, string loc, string searchTerm = null)
        {
            int pageSize = _appSettings.PageSize;
            int skip = (pageNumber - 1) * pageSize;

            var url = $"{_baseUrl}/api/SettingApi?skip={skip}&take={pageSize}&loc={loc}";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"&searchTerm={searchTerm}";
            }

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Log the error details
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GetGuestAsync: {response.StatusCode}, Content: {content} ");

                // You can either throw an exception or return an empty list
                // throw new Exception("Failed to retrieve rooms from API");
                return new List<RoomRates>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RoomRates>>(jsonString);
        }
        public async Task<RoomRates> GetRoomRatesByIdAsync(string id)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/SettingApi/{id}");
            return JsonConvert.DeserializeObject<RoomRates>(response);
        }
        public async Task UpdateRoomRateAsync(RoomRates roomRates)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/SettingApi/{roomRates.ID}", roomRates);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to update room rate: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }


    }
}
