namespace AppliedInvoice.Components.Pages
{
    public partial class FbrDataLoad
    {
        private HttpClient Http = new HttpClient();
        public string ResponseText { get; set; }
        public string InvoiceJson { get; set; }
        // RATE
        public DateTime RateDate { get; set; } = DateTime.Today;
        public int TransTypeId { get; set; } = 18;
        public int OriginationSupplier { get; set; } = 1;
        // HS UOM
        public string HsCode { get; set; } = "5904.9000";
        public int AnnexureId { get; set; } = 3;
        // SRO
        public DateTime SroDate { get; set; } = DateTime.Today;
        public int SroId { get; set; } = 389;
        


        public void GetRegType()
        {
            ResponseText = GetRegisterType().Result;
        }
        
        
        public async Task<string> GetRegisterType()
        {
            try
            {
                var _response = await ApiService.GetRegistrationTypeAsync("1761600");
                return $"{_response.REGISTRATION_NO} : {_response.REGISTRATION_TYPE}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
