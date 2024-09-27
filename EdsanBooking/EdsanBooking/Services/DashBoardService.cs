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
using System.Net.Http.Json;

namespace EdsanBooking.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public DashBoardService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task<int> GetBookingSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            // Format dates as ISO 8601
            var fromDateString = fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var requestUri = $"{_baseUrl}/api/DashboardApi/sum-bookings?fromDate={Uri.EscapeDataString(fromDateString)}&toDate={Uri.EscapeDataString(toDateString)}&location={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
        public async Task<int> GetCheckOutSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            // Format dates as ISO 8601
            var fromDateString = fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var requestUri = $"{_baseUrl}/api/DashboardApi/sum-checkout?fromDate={Uri.EscapeDataString(fromDateString)}&toDate={Uri.EscapeDataString(toDateString)}&location={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }

        public async Task<int> GetReservedSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            // Format dates as ISO 8601
            var fromDateString = fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var requestUri = $"{_baseUrl}/api/DashboardApi/sum-reserved?fromDate={Uri.EscapeDataString(fromDateString)}&toDate={Uri.EscapeDataString(toDateString)}&location={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                // Use ReadFromJsonAsync instead of ReadAsAsync
                return await response.Content.ReadFromJsonAsync<int>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
        public async Task<int> GetGuestSumByDateRangeAsync(DateTime fromDate, DateTime toDate, string location)
        {
            // Format dates as ISO 8601
            var fromDateString = fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var requestUri = $"{_baseUrl}/api/DashboardApi/sum-guest?fromDate={Uri.EscapeDataString(fromDateString)}&toDate={Uri.EscapeDataString(toDateString)}&location={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                // Use ReadFromJsonAsync instead of ReadAsAsync
                return await response.Content.ReadFromJsonAsync<int>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
        public async Task<List<RevenueData>> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate, string location)
        {
            // Format dates as ISO 8601
            var fromDateString = fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var toDateString = toDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Construct the query string with URL encoding
            var requestUri = $"{_baseUrl}/api/DashboardApi/revenue?fromDate={Uri.EscapeDataString(fromDateString)}&toDate={Uri.EscapeDataString(toDateString)}&location={Uri.EscapeDataString(location)}";


            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<RevenueData>>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
        public async Task<int> GetRoomStatusCountAsync(string status, string location)
        {
            // Combine the status and location into a single API request string
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/DashboardApi/room-status-count?status={status}&location={location}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }
        public async Task<Dictionary<string, int>> GetRoomStatusCountsAsync(string location)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/DashboardApi/room-status-counts?location={location}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve data: {response.ReasonPhrase}. Response content: {responseContent}");
            }
        }


    }
}
