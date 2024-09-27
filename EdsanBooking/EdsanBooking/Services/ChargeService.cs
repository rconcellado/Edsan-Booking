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

namespace EdsanBooking.Services
{
    public class ChargeService : IChargeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public ChargeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task<TransientRes> GetTransientDetailsByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/ChargeApi/GetTransientDetails/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var transientCheckIn = await response.Content.ReadFromJsonAsync<TransientRes>();
            return transientCheckIn;
        }
        public async Task<ResortRes> GetResortDetailsByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/ChargeApi/GetResortDetails/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var resortCheckIn = await response.Content.ReadFromJsonAsync<ResortRes>();
            return resortCheckIn;
        }
        public async Task<decimal> GetTransientTotalAmountAsync(string id, string location)
        {
            var request = new
            {
                Id = id,
                Location = location
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ChargeApi/CalculateTransientTotalAmountAsync", request);

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
        public async Task<decimal> GetResortTotalAmountAsync(string id, string location)
        {
            var request = new
            {
                Id = id,
                Location = location
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/ChargeApi/CalculateResortTotalAmountAsync", request);

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
    }
}
