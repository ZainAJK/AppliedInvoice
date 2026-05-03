using AppliedInvoice.Models;
using AppliedInvoice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppliedInvoice.Controllers
{
    [ApiController]
    [Route("api")]
    public class FbrController : ControllerBase
    {
        private readonly FbrService _service;
 
        public FbrController(FbrService service)
        {
            _service = service;
        }

        [HttpPost("submit-invoice")]
        public async Task<IActionResult> SubmitInvoice([FromBody] InvoiceMaster model)
        {
            if (model == null || model.items.Count == 0)
            {
                return BadRequest("Invalid invoice data");
            }

            try
            {
                var result = await _service.SubmitInvoiceAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }

        }
    }
}
