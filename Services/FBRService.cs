using AppliedInvoice.Models;
using System.Text;
using System.Text.Json;
using AppliedInvoice.Logic.Models;

namespace AppliedInvoice.Services
{
    public class FbrService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FbrService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<FbrResponse> SubmitInvoiceAsync(FBRRequestModel invoice)
        {
            var url = _config["FBR:BaseUrl"]; // e.g sandbox/prod

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Headers (VERY IMPORTANT)
            request.Headers.Add("Authorization", $"Bearer {_config["FBR:Token"]}");
            request.Headers.Add("Client-Id", _config["FBR:ClientId"]);
            request.Headers.Add("Client-Secret", _config["FBR:ClientSecret"]);

            request.Content = new StringContent(
                JsonSerializer.Serialize(invoice),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"FBR Error: {json}");
            }

            return JsonSerializer.Deserialize<FbrResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
