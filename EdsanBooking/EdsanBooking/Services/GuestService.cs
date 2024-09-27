using EdsanBooking.Models;
using EdsanBooking.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using EdsanBooking.Interface;

namespace EdsanBooking.Services
{
    public class GuestService : IGuestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly AppSettings _appSettings;
        public GuestService(HttpClient httpClient, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"]; // Get the base URL from configuration
            _appSettings = appSettings.Value;
        }
        public async Task<int> GetTotalGuestCountAsync(string searchTerm = null)
        {
            var url = $"{_baseUrl}/api/GuestApi/count";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"?searchTerm={searchTerm}";
            }

            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<int>(response);
        }

        public async Task<List<Guest>> GetGuestAsync(int pageNumber, string searchTerm = null)
        {
            int pageSize = _appSettings.PageSize;
            int skip = (pageNumber - 1) * pageSize;

            var url = $"{_baseUrl}/api/GuestApi?skip={skip}&take={pageSize}";
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
                return new List<Guest>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Guest>>(jsonString);
        }
        public async Task<Guest> GetGuestByIdAsync(string id)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/GuestApi/{id}");
            return JsonConvert.DeserializeObject<Guest>(response);
        }
        public async Task<List<Guest>> GetAllGuestAsync()
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/GuestApi");
            return JsonConvert.DeserializeObject<List<Guest>>(response);
        }
        public async Task<Guest> AddGuestAsync(Guest guest)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/GuestApi", guest);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during AddGuestAsync: {response.StatusCode}, Content: {content} ");
                return null;
            }

            var createdGuest = JsonConvert.DeserializeObject<Guest>(await response.Content.ReadAsStringAsync());
            return createdGuest;
        }
        public async Task<bool> DeleteGuestAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/GuestApi/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during DeleteGuestAsync: {response.StatusCode}, Content: {content}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateGuestAsync(Guest guest)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/GuestApi/{guest.GuestId}", guest);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during UpdateGuestAsync: {response.StatusCode}, Content: {content}");
            }
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> GuestExistByFirstNameLastName(string firstName, string lastName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/GuestApi/GuestExistsByFirstNameLastName?firstName={firstName}&lastName={lastName}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GuestExistsAsync: {response.StatusCode}, Content: {content}");
                return false;
            }
            var exists = await response.Content.ReadAsStringAsync();
            return bool.Parse(exists);
        }
        public async Task<bool> GuestExistsAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/GuestApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GuestExistsAsync: {response.StatusCode}, Content: {content}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<string> GenerateNextGuestIdAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/GuestApi/GenerateNextGuestId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return result.guestId;  // Access the GuestId property
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GenerateNextGuestId: {response.StatusCode}, Content: {errorContent}");

                throw new HttpRequestException($"Failed to generate next Guest ID. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }

        public async Task<string> GetGuestIdByFirstNameLastNameAsync(string firstName, string lastName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/GuestApi/GetGuestIdByFirstNameLastName?firstName={firstName}&lastName={lastName}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                return result.guestId;  // Access the GuestId property
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API error during GetGuestIdByFirstNameLastNameAsync: {response.StatusCode}, Content: {errorContent}");

                // Return null or throw an exception, depending on how you want to handle errors
                throw new HttpRequestException($"Failed to get the Guest ID. StatusCode: {response.StatusCode}, Content: {errorContent}");
            }
        }
        public async Task SaveOrUpdateGuestAsync(string id, GuestDetailViewModel guestDetail)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/GuestApi/SaveOrUpdateGuest?id={id}", guestDetail);
            response.EnsureSuccessStatusCode();
        }
        public async Task<string> SignupAsync(SignupDto signupDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/GuestApi/signup", signupDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Signup failed: {error}");
        }
        public async Task<string> GetGuestIdByUserNameAsync(string username)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/GuestApi/GetGuestIdByUserName/{username}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle different status codes accordingly
                throw new Exception("Failed to retrieve the Guest ID.");
            }
        }
    }
}
