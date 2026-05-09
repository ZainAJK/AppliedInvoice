using AppliedInvoice.Models;
using Microsoft.AspNetCore.Components;
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


        FbrInvoice invoice = new FbrInvoice()
        {
            items = new List<FbrInvoiceItems>()
        };

        public FbrInvoiceItems currentItem = new FbrInvoiceItems();

        FbrResponse ApiResponse = new();
        bool IsFbrResponse { get; set; }

        void AddItem()
        {
            //CalculateCurrentItem();
            invoice.items ??= new();
            invoice.items.Add(currentItem);
            currentItem = new FbrInvoiceItems();
        }

        public void CalculateCurrentItem()
        {
            var rate = Convert.ToDecimal(currentItem.rate.Replace("%", ""));

            if (rate < 0)
                rate = 0;

            if (currentItem.quantity < 0)
                currentItem.quantity = 0;

            decimal baseTotal = rate * currentItem.quantity;

            if (currentItem.valueSalesExcludingST == 0)
            {
                currentItem.valueSalesExcludingST = baseTotal;
            }

            if (currentItem.salesTaxApplicable == 0)
            {
                currentItem.salesTaxApplicable =
                    currentItem.valueSalesExcludingST * 0.18M;
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

                //if (invoiceId > 0)
                //{
                //    DB.FBRResponseSave(ApiResponse, invoiceId);

                //    // ✅ SUCCESS POPUP
                //    await JS.InvokeVoidAsync("alert", "✅ Invoice Successfully Saved!");
                //}
                //else
                //{
                //    // ❌ ERROR POPUP
                //    await JS.InvokeVoidAsync("alert", "❌ Invoice Save Failed!");
                //}

                //MyLogs = DB.Logs;
                //MyErrors = DB.Errors;

                await InvokeAsync(StateHasChanged);
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

          

            
        }

        void SafeDefaults()
        {


            if (currentItem.quantity < 0)
                currentItem.quantity = 0;

            if (currentItem.valueSalesExcludingST < 0)
                currentItem.valueSalesExcludingST = 0;

            if (currentItem.salesTaxApplicable < 0)
                currentItem.salesTaxApplicable = 0;

            if (currentItem.extraTax < 0)
                currentItem.extraTax = 0;

            if (currentItem.furtherTax < 0)
                currentItem.furtherTax = 0;

            if (currentItem.fedPayable < 0)
                currentItem.fedPayable = 0;

            if (currentItem.discount < 0)
                currentItem.discount = 0;

        }
        void DeleteItem(int index)
        {
            invoice.items.RemoveAt(index);
        }
        void SaveInvoice()
        {
            Console.WriteLine("Invoice Saved");
        }


    }
}