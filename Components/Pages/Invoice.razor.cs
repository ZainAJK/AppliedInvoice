using AppliedInvoice.Models;
using AppliedInvoice.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;


namespace AppliedInvoice.Components.Pages
{
  
    public partial class Invoice
    {
        // 🔥 DB Inject
        public List<string> MyLogs { get; set; } = new();
        public List<string> MyErrors { get; set; } = new();


        InvoiceMaster invoice = new InvoiceMaster()
        {
            items = new List<InvoiceDetails>()
        };

        InvoiceDetails currentItem = new InvoiceDetails();

        FbrResponse ApiResponse = new();
        bool IsFbrResponse { get; set; }

        void AddItem()
        {
            //CalculateCurrentItem();
            invoice.items ??= new();
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

        public void EditItem(string _RecID)
        {
            var _Item = invoice.items.Where(e=> e.recID.ToString() == _RecID).FirstOrDefault();
            if(_Item != null)
            {
                currentItem = _Item;
            }

            InvokeAsync(StateHasChanged);
        }

        public void DeleteItem(string _RecID)
        {
            var _Item = invoice.items.Where(e => e.recID.ToString() == _RecID).FirstOrDefault();
            if (_Item != null)
            {
                invoice.items.Remove(_Item);
                if(invoice.items.Count > 0)
                {
                    currentItem = invoice.items[0];
                }
                else
                {
                    currentItem = new();
                }
            }

            InvokeAsync(StateHasChanged);

            
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
        [Inject] public IJSRuntime JS { get; set; }
        public async void Save()
        {
            if (GetFbrResponse())
            {
                long invoiceId = DB.SaveInvoice(invoice);

                if (invoiceId > 0)
                {
                    DB.FBRResponseSave(ApiResponse, invoiceId);

                    // ✅ SUCCESS POPUP
                    await JS.InvokeVoidAsync("alert", "✅ Invoice Successfully Saved!");
                }
                else
                {
                    // ❌ ERROR POPUP
                    await JS.InvokeVoidAsync("alert", "❌ Invoice Save Failed!");
                }

                MyLogs = DB.Logs;
                MyErrors = DB.Errors;

                InvokeAsync(StateHasChanged);
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

        public void Temp()
        {
            invoice = new();

            invoice.invoiceType = "Sale Invoice";
            invoice.invoiceDate = DateTime.Now;
            invoice.sellerNTNCNIC = "123456-7";
            invoice.sellerBusinessName = "Zain Enterprises";
            invoice.sellerProvince = "Sindh";
            invoice.sellerAddress = "Flat # 101, Ellahbad Terrace";

            invoice.buyerNTNCNIC = "765432-1";
            invoice.buyerBusinessName = "Hassan Enterprises";
            invoice.buyerProvince = "Sindh";
            invoice.buyerAddress = "Flat # 202, Ellahbad Terrace";

            invoice.buyerRegisterationType = "Registered";
            invoice.invoiceRefNo = "001-123";
            invoice.scenarioId = "001";

            //---



            currentItem.recID = Guid.NewGuid();
            currentItem.hsCode = "8001.9821";
            currentItem.productDescription = "Rice";
            currentItem.rate = 100;
            currentItem.uoM = "KG";
            currentItem.quantity = 10;
            currentItem.valueSalesExcludingsST = currentItem.rate * currentItem.quantity;
            currentItem.salesTaxApplicable = 0.18M;

            decimal _SalesTax = decimal.Parse((currentItem.valueSalesExcludingsST * currentItem.salesTaxApplicable).ToString());
            currentItem.totalValues = currentItem.valueSalesExcludingsST + (_SalesTax);
            currentItem.fixedNotifiedValueOrRetailPrice = currentItem.totalValues;
            currentItem.salesTaxWithheldAtSource = Math.Round(_SalesTax * 0.20M,2);
            currentItem.extraTax = 0.00M;
            currentItem.furtherTax = 0.00M;
            currentItem.sroScheduleNo = 4321;
            currentItem.fedPayable = 0.00M;
            currentItem.discount = 0.00M;
            currentItem.saleType = "Regular";
            currentItem.sroItemSerialNo = "6541";

            AddItem();

            currentItem.recID = Guid.NewGuid();
            currentItem.hsCode = "8001.1289";
            currentItem.productDescription = "Sugar";
            currentItem.rate = 150;
            currentItem.uoM = "KG";
            currentItem.quantity = 10;
            currentItem.valueSalesExcludingsST = currentItem.rate * currentItem.quantity;
            currentItem.salesTaxApplicable = 0.18M;

             _SalesTax = decimal.Parse((currentItem.valueSalesExcludingsST * currentItem.salesTaxApplicable).ToString());
            currentItem.totalValues = currentItem.valueSalesExcludingsST + (_SalesTax);
            currentItem.fixedNotifiedValueOrRetailPrice = currentItem.totalValues;
            currentItem.salesTaxWithheldAtSource = Math.Round(_SalesTax * 0.20M, 2);
            currentItem.extraTax = 0.00M;
            currentItem.furtherTax = 0.00M;
            currentItem.sroScheduleNo = 1234;
            currentItem.fedPayable = 0.00M;
            currentItem.discount = 0.00M;
            currentItem.saleType = "Regular";
            currentItem.sroItemSerialNo = "9874";

            AddItem();

            currentItem.recID = Guid.NewGuid();
            currentItem.hsCode = "8001.1230";
            currentItem.productDescription = "Cooking Oil";
            currentItem.rate = 520;
            currentItem.uoM = "KG";
            currentItem.quantity = 50;
            currentItem.valueSalesExcludingsST = currentItem.rate * currentItem.quantity;
            currentItem.salesTaxApplicable = 0.18M;

             _SalesTax = decimal.Parse((currentItem.valueSalesExcludingsST * currentItem.salesTaxApplicable).ToString());
            currentItem.totalValues = currentItem.valueSalesExcludingsST + (_SalesTax);
            currentItem.fixedNotifiedValueOrRetailPrice = currentItem.totalValues;
            currentItem.salesTaxWithheldAtSource = Math.Round(_SalesTax * 0.20M, 2);
            currentItem.extraTax = 0.00M;
            currentItem.furtherTax = 0.00M;
            currentItem.sroScheduleNo = 1234;
            currentItem.fedPayable = 0.00M;
            currentItem.discount = 0.00M;
            currentItem.saleType = "Regular";
            currentItem.sroItemSerialNo = "9874";

            AddItem();
        }

    }
}