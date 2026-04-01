using AppliedInvoice.Models;
using AppliedInvoice.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppliedInvoice.Components.Pages
{
    public partial class Invoice
    {
        // 🔥 DB Inject
        

        InvoiceMaster invoice = new InvoiceMaster()
        {
            items = new List<InvoiceDetails>()
        };

        InvoiceDetails currentItem = new InvoiceDetails();

        FbrResponse ApiResponse = new();
        bool IsFbrResponse { get; set; }

        void AddItem()
        {
            CalculateCurrentItem();
            invoice.items.Add(currentItem);
            currentItem = new InvoiceDetails();
        }

        public void CalculateCurrentItem()
        {
            if (currentItem.rate < 0)
                currentItem.rate = 0;

            if (currentItem.quantity < 0)
                currentItem.quantity = 0;

            decimal baseTotal = currentItem.rate * currentItem.quantity;

            if (currentItem.valueSalesExcludingsST == 0)
            {
                currentItem.valueSalesExcludingsST = baseTotal;
            }

            if (currentItem.salesTaxApplicable == 0)
            {
                currentItem.salesTaxApplicable =
                    currentItem.valueSalesExcludingsST * 0.18M;
            }

            if (currentItem.extraTax < 0)
                currentItem.extraTax = 0;

            if (currentItem.furtherTax < 0)
                currentItem.furtherTax = 0;

            if (currentItem.fedPayable < 0)
                currentItem.fedPayable = 0;

            if (currentItem.discount < 0)
                currentItem.discount = 0;

            currentItem.totalValues =
                baseTotal
                + currentItem.salesTaxApplicable
                + currentItem.extraTax
                + currentItem.furtherTax
                + currentItem.fedPayable
                - currentItem.discount;
        }

        public void DeleteItem(int index)
        {
            invoice.items.RemoveAt(index);
        }

        public decimal GrandTotal()
        {
            if (invoice.items == null || invoice.items.Count == 0)
                return 0;

            return invoice.items.Sum(item =>
                item.totalValues
                + item.salesTaxApplicable
                + item.extraTax
                + item.furtherTax
                + item.fedPayable
                - item.discount
            );
        }

        public void Save()
        {
            if (GetFbrResponse())
            {
                DB.SaveInvoice(invoice);
            }
        }

        public bool GetFbrResponse()
        {
            ApiResponse = new()
            {
                statusCode = "00",
                status = "Success",
                message = "FBR API Response found successfully",
                invoiceNumber = "12345678901234567890",
                qrCode = "ajsdfhkashfasjdkajfkjadsfkjalkjgjlakgfja",
                validationResponse = null!,
                errorDetails = null!
            };

            return true; 

            string BaseAddress = "https://gw.fbr.gov.pk/di_data/v1/di/postinvoicedata_sb";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", "your-token");

                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(invoice),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage httpResponse = client.PostAsync(BaseAddress, content).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;

                    ApiResponse = JsonConvert.DeserializeObject<FbrResponse>(jsonResponse);

                    if (ApiResponse.statusCode == "00")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}