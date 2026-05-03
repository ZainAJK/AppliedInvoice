using AppliedInvoice.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppliedInvoice.Services
{
    public class FbrService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public FbrService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient(); // manually created
        }

        public async Task<FbrResponse> SubmitInvoiceAsync(FbrInvoice invoice)
        {
            var url = _config["FBR:BaseUrl"]; // e.g sandbox/prod
            var tokenPost = _config["FBR:TokenPost"]; // your token

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {tokenPost}");
            //request.Headers.Add("Client-Id", _config["FBR:ClientId"]);
            //request.Headers.Add("Client-Secret", _config["FBR:ClientSecret"]);

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

            return JsonSerializer.Deserialize<FbrResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }






        // Copy from FBR web site, not used in code, just for reference
        public void PostInvoiceData(FbrInvoice objinvoice)
        {

            var url = _config["FBR:BaseUrl"]; // e.g sandbox/prod
            var TokenPost = _config["FBR:TokenPost"]; // your token


            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenPost);
            StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(objinvoice), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(url, content).Result;



            if (response.IsSuccessStatusCode)

            {

                Console.WriteLine("Response from API:");

                Console.WriteLine("-------------------------------");

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            }
        }

    }
}


