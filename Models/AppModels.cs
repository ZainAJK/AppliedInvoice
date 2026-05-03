
using System.Text.Json;
using System.Text.Json.Serialization;


namespace AppliedInvoice.Models
{

    public class FbrInvoice
    {
        public string? invoiceType { get; set; }

        public DateTime invoiceDate { get; set; }

        public string? sellerNTNCNIC { get; set; }
        public string? sellerBusinessName { get; set; }
        public string? sellerProvince { get; set; }
        public string? sellerAddress { get; set; }

        public string? buyerNTNCNIC { get; set; }
        public string? buyerBusinessName { get; set; }
        public string? buyerProvince { get; set; }
        public string? buyerAddress { get; set; }

        public string? buyerRegisterationType { get; set; }

        public string? invoiceRefNo { get; set; }

        public string? scenarioId { get; set; }

        public List<FbrInvoiceItems> items { get; set; } = [];
    }

    public class FbrInvoiceItems
    {
        public string? hsCode { get; set; }
        public string? productDescription { get; set; }
        public string rate { get; set; }
        public string? uoM { get; set; }
        public decimal quantity { get; set; }

        public decimal totalValues { get; set; }
        public decimal valueSalesExcludingST { get; set; }
        public decimal fixedNotifiedValueOrRetailPrice { get; set; }

        public decimal salesTaxApplicable { get; set; }
        public decimal salesTaxWithheldAtSource { get; set; }

        public decimal extraTax { get; set; }
        public decimal furtherTax { get; set; }

        public decimal sroScheduleNo { get; set; }

        public decimal fedPayable { get; set; }

        public decimal discount { get; set; }

        public string? saleType { get; set; }

        public string? sroItemSerialNo { get; set; }
    }


    public class FbrResponse
    {
        public string statusCode { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string invoiceNumber { get; set; }
        public string qrCode { get; set; }

        public ValidationResponseRoot validationResponse { get; set; }

        public string errorDetails { get; set; }
    }

    public class ValidationResponseRoot
    {
        public string statusCode { get; set; }
        public string status { get; set; }
        public string error { get; set; }

        public List<InvoiceStatus> invoiceStatuses { get; set; }
    }

    public class InvoiceStatus
    {
        public string itemSNo { get; set; }
        public string statusCode { get; set; }
        public string status { get; set; }
        public int invoiceNo { get; set; }
        public string errorCode { get; set; }
        public string error { get; set; }
    }

    

}