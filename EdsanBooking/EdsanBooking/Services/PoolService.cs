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
    public class PoolService : IPoolService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public PoolService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task<CheckInDetailsViewModel> GetDetailsAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/PoolApi/GetDetails/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CheckInDetailsViewModel>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve check-in details: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
    }
}
