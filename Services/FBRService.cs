using AppliedInvoice.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppliedInvoice.Services
{
    public class FbrService
    {
        public readonly IConfiguration _config;
        public readonly HttpClient _httpClient;

        public FbrService(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _httpClient = factory.CreateClient("ApiClient");
        }


        // Test Invoice Data
        public async Task<FbrResponse> SubmitInvoiceAsync(FbrInvoice invoice)
        {
       
            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress); ;
            
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

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };


            var _result = JsonSerializer.Deserialize<FbrResponse>(json, options)!;
            return _result;
        }


        // Get Invoice Json Text from appsetting.json file
        public async Task<FbrInvoice> GetInvoiceFromAppSettingsAsync()
        {
            var inv = _config.GetSection("FbrValidate_sb").Get<FbrInvoice>();
            if (inv == null)
                throw new Exception("FbrInvoice section not found in appsettings.json");
            // Set runtime values here (IMPORTANT)
            inv.invoiceDate = DateTime.Now;
            // Optional: override invoice ref dynamically
            inv.invoiceRefNo = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            return inv;
        }

        private void SetHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["FBR:Token"]);
        }

        public async Task<string> PostInvoiceAsync(FbrInvoice invoice)
        {
            SetHeaders();

            var json = JsonSerializer.Serialize(invoice);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config["FBR:PostInvoiceUrl"], content);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ValidateInvoiceAsync(FbrInvoice invoice)
        {
            SetHeaders();

            var json = JsonSerializer.Serialize(invoice);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config["FBR:ValidateInvoiceUrl"], content);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string url)
        {
            SetHeaders();
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<RegStateResponse> GetRegistrationTypeAsync(string NTN_CNIC)
        {
            List<string> Messages = new();

            if(string.IsNullOrWhiteSpace(NTN_CNIC))
            {
                return new RegStateResponse
                {
                    statuscode = "Error",
                    REGISTRATION_NO = NTN_CNIC,
                    REGISTRATION_TYPE = "Invalid NTN/CNIC provided."
                };
            }



            try
            {
            var sendBoxUrl = "https://gw.fbr.gov.pk/dist/v1/Get_Reg_Type";



            var requestObject = new
            {
                Registration_No = NTN_CNIC
            };

            var request = new HttpRequestMessage(HttpMethod.Post, sendBoxUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(requestObject),
                    Encoding.UTF8,
                    "application/json")
            };

                Messages.Add($"Request URL: {request.RequestUri}");
                Messages.Add($"Request: {JsonSerializer.Serialize(requestObject)}");

                var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"FBR Error: {json}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var result = JsonSerializer.Deserialize<RegStateResponse>(json, options);

            return result!;

            }
            catch (Exception error)
            {
                return new RegStateResponse
                {
                    statuscode = "Error",
                    REGISTRATION_NO = NTN_CNIC,
                    REGISTRATION_TYPE = "Unknown: " + error.Message + " | " + string.Join(", ", Messages)
                };

            }
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


