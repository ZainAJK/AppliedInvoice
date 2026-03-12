namespace AppliedInvoice.Models
{
    public class AppModels
    {
    }

    public class InvoiceMaster
    {
        public string invoiceType { get; set;  }
    }

    public class InvoiceDetails
    {
        public string hsCode { get; set; }
        public decimal quantity { get; set; }
    }
}
