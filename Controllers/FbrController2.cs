using AppliedInvoice.Models;
using AppliedInvoice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppliedInvoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FbrController2 : ControllerBase
    {

        private readonly FbrService fbrService;

        public FbrController2(FbrService service)
        {
            fbrService = service;
        }

        // POST Invoice
        [HttpPost("postinvoice")]
        public async Task<IActionResult> PostInvoice(
            [FromBody] FbrInvoice invoice)
        {
            var result = await fbrService.PostInvoiceAsync(invoice);

            return Ok(result);
        }

        // Validate Invoice
        [HttpPost("validateinvoice")]
        public async Task<IActionResult> ValidateInvoice(
            [FromBody] FbrInvoice invoice)
        {
            var result = await fbrService.ValidateInvoiceAsync(invoice);

            return Ok(result);
        }

        // Province
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var url = "https://gw.fbr.gov.pk/pdi/v1/provinces";
            return Ok(await fbrService.GetAsync(url));
        }

        // Document Types
        [HttpGet("doctype")]
        public async Task<IActionResult> DocType()
        {
            var url = "https://gw.fbr.gov.pk/pdi/v1/doctypecode";

            return Ok(await fbrService.GetAsync(url));
        }

        // Item Codes
        [HttpGet("itemcodes")]
        public async Task<IActionResult> ItemCodes()
        {
            var url = "https://gw.fbr.gov.pk/pdi/v1/itemdesccode";

            return Ok(await fbrService.GetAsync(url));
        }

        // UOM
        [HttpGet("uom")]
        public async Task<IActionResult> Uom()
        {
            var url = "https://gw.fbr.gov.pk/pdi/v1/uom";

            return Ok(await fbrService.GetAsync(url));
        }

        // Transaction Type
        [HttpGet("transtype")]
        public async Task<IActionResult> TransactionType()
        {
            var url = "https://gw.fbr.gov.pk/pdi/v1/transtypecode";

            return Ok(await fbrService.GetAsync(url));
        }

        // Sale Type Rate
        [HttpGet("rate")]
        public async Task<IActionResult> Rate(
            string date,
            int transTypeId,
            int originationSupplier)
        {
            var url =
                $"https://gw.fbr.gov.pk/pdi/v2/SaleTypeToRate?date={date}&transTypeId={transTypeId}&originationSupplier={originationSupplier}";

            return Ok(await fbrService.GetAsync(url));
        }

        // HS UOM
        [HttpGet("hsuom")]
        public async Task<IActionResult> HsUom(
            string hsCode,
            int annexureId)
        {
            var url =
                $"https://gw.fbr.gov.pk/pdi/v2/HS_UOM?hs_code={hsCode}&annexure_id={annexureId}";

            return Ok(await fbrService.GetAsync(url));
        }

        // SRO Item
        [HttpGet("sroitem")]
        public async Task<IActionResult> SroItem(
            string date,
            int sroId)
        {
            var url =
                $"https://gw.fbr.gov.pk/pdi/v2/SROItem?date={date}&sro_id={sroId}";

            return Ok(await fbrService.GetAsync(url));
        }

        [HttpGet("RegisterType")]
        public async Task<IActionResult> RegisterType(string NTN_CNIC)
        { 
            var _result = fbrService.GetRegistrationTypeAsync(NTN_CNIC);
            return Ok(_result);
        }
            
            
            
            
    }
}