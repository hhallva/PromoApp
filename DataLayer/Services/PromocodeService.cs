using DataLayer.Models;
using System.Net;
using System.Net.Http.Json;

namespace DataLayer.Services
{
    public class PromocodeService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7016/api/promocodes";

        public PromocodeService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new(_baseUrl);
        }

        public PromocodeService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new(_baseUrl);
        }

        // GET /api/promocodes
        public async Task<List<Promocode>> GetAllAsync()
        {
            var response = await _client.GetAsync("");
            await HandleResponseAsync(response);

            return await response.Content.ReadFromJsonAsync<List<Promocode>>()
                   ?? new List<Promocode>();
        }

        // GET /api/promocodes/{code}
        public async Task<Promocode?> GetAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promocode code cannot be null or empty.", nameof(code));

            var response = await _client.GetAsync($"{code}");
            await HandleResponseAsync(response);

            return await response.Content.ReadFromJsonAsync<Promocode>();
        }

        // POST /api/promocodes
        public async Task<Promocode> AddPromocodeAsync(Promocode promocode)
        {
            if (promocode == null)
                throw new ArgumentNullException(nameof(promocode));

            var response = await _client.PostAsJsonAsync(_baseUrl, promocode);
            await HandleResponseAsync(response, expectedStatus: HttpStatusCode.Created);

            var createdPromocode = await response.Content.ReadFromJsonAsync<Promocode>();
            if (createdPromocode == null)
                throw new InvalidOperationException("Failed to deserialize created promocode.");

            return createdPromocode;
        }

        // PUT /api/promocodes/{code}/activate
        public async Task ActivatePromocodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promocode code cannot be null or empty.", nameof(code));

            var response = await _client.PutAsync($"{code}/activate", null);
            await HandleResponseAsync(response, expectedStatus: new[] { HttpStatusCode.NoContent, HttpStatusCode.OK });
        }

        // DELETE /api/promocodes/{code}
        public async Task DeletePromocodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promocode code cannot be null or empty.", nameof(code));

            var response = await _client.DeleteAsync($"{code}");
            await HandleResponseAsync(response, expectedStatus: HttpStatusCode.NoContent);
        }


        private static async Task HandleResponseAsync(HttpResponseMessage response, params HttpStatusCode[] expectedStatus)
        {
            if (expectedStatus.Length == 0)
                expectedStatus = new[] { HttpStatusCode.OK };

            if (!expectedStatus.Contains(response.StatusCode))
            {
                string errorMessage = await response.Content.ReadAsStringAsync() ?? $"HTTP {response.StatusCode}";

                throw new HttpRequestException(
                    $"API request failed with status {response.StatusCode}: {errorMessage}",
                    null,
                    response.StatusCode
                );
            }
        }
    }
}
