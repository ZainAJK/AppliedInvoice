using AppliedInvoice.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace AppliedInvoice.Services
{
    public class PdfService
    {

        public void GenerateInvoice(FbrInvoice invoice)
        {

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Invoices");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, $"Invoice_{DateTime.Now.Ticks}.pdf");


            PdfWriter writer = new PdfWriter(file);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);


            document.Add(new Paragraph("SALES TAX INVOICE"));
            document.Add(new Paragraph("Invoice Type: " + invoice.invoiceType));
            document.Add(new Paragraph("Invoice Date: " + invoice.invoiceDate));
            document.Add(new Paragraph("Seller: " + invoice.sellerBusinessName));
            document.Add(new Paragraph("Buyer: " + invoice.buyerBusinessName));

            document.Add(new Paragraph("Items"));

            foreach (var item in invoice.items)
            {
                document.Add(new Paragraph(
                    item.productDescription +
                    " | Qty: " + item.quantity +
                    " | Rate: " + item.rate +
                    " | Total: " + item.totalValues
                ));
            }

            document.Close();
        }

    }
}