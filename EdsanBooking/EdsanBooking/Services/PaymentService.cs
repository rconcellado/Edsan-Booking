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
using System.Globalization;

namespace EdsanBooking.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public PaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task AddPaymentAsync(string id, decimal chargeAmount,
                                          decimal paymentAmount, decimal discountAmount, decimal excessAmount)
        {
            var paymentRequest = new PaymentRequestModel
            {
                Id = id,
                ChargeAmount = chargeAmount,
                PaymentAmount = paymentAmount,
                DiscountAmount = discountAmount,
                ExcessAmount = excessAmount
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/PaymentApi/AddPayment", paymentRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {response.StatusCode}, Content: {errorContent}");
            }

            response.EnsureSuccessStatusCode();
        }
        public async Task<decimal> GetTotalPaymentsMadeAsync(string id)
        {
            //var paymentRequest = new PaymentRequestModel
            //{
            //    ReservationId = reservationId,
            //    CheckInId = checkInId
            //};

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/PaymentApi/GetTotalPaymentsMadeAsync", id);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<decimal>(content);
                return result;
            }
            else
            {
                // Instead of throwing an error, return 0 when something goes wrong
                return 0;
            }
        }

        public async Task AddRoomChargeAsync(string id)
        {
            //var paymentRequest = new PaymentRequestModel
            //{
            //    Id = Id,
            //    ChargeAmount = 0,
            //    PaymentAmount = 0,
            //    DiscountAmount = 0,
            //    ExcessAmount = 0
            //};

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/PaymentApi/AddRoomChargeAsync", id);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {response.StatusCode}, Content: {errorContent}");
            }

            response.EnsureSuccessStatusCode();
        }


    }
}
