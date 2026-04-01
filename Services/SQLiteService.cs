using System.Data.SQLite;
using System.Data;
using AppliedInvoice.Models;

namespace AppliedInvoice.Services
{
    public class SQLiteService
    {
        public SQLiteConnection MyConnection { get; set; }

        public SQLiteService(IWebHostEnvironment env)
        {
            var dbPath = Path.Combine(
                env.WebRootPath,
                "DB",
                "Invoice.db"
            );

            MyConnection = new SQLiteConnection($"Data Source={dbPath};");
        }

        public void SaveInvoice(InvoiceMaster invoice)
        {

            DataTable Inv_Master = GetDataTable("InvoiceMaster"); 
            DataTable Inv_Details = GetDataTable("InvoiceDetails");

            DataRow Row_Master = Inv_Master.NewRow();
            DataRow Row_Detail = Inv_Details.NewRow();
            List<DataRow> Row_Details = new();

            Row_Master["invoiceType"] = invoice.invoiceType;
            Row_Master["invoiceDate"] = invoice.invoiceDate;
            Row_Master["invoiceRefNo"] = invoice.invoiceRefNo;
            Row_Master["scenarioId"] = invoice.scenarioId;

            Row_Master["sellerNTNCNIC"] = invoice.sellerNTNCNIC;
            Row_Master["sellerBusinessName"] = invoice.sellerBusinessName;
            Row_Master["sellerProvince"] = invoice.sellerProvince;
            Row_Master["sellerAddress"] = invoice.sellerAddress;

            Row_Master["buyerNTNCNIC"] = invoice.buyerNTNCNIC;
            Row_Master["buyerBusinessName"] = invoice.buyerBusinessName;
            Row_Master["buyerProvince"] = invoice.buyerProvince;
            Row_Master["buyerRegisterationType"] = invoice.buyerRegisterationType;
            Row_Master["buyerAddress"] = invoice.buyerAddress;

            foreach (var item in invoice.items)
            {
                Row_Detail["hsCode"] = item.hsCode;
                Row_Detail["productDescription"] = item.productDescription;
                Row_Detail["rate"] = item.rate;
                Row_Detail["uoM"] = item.uoM;
                Row_Detail["quantity"] = item.quantity;

                Row_Detail["valueSalesExcludingsST"] = item.valueSalesExcludingsST;
                Row_Detail["fixedNotifiedValueOrRetailPrice"] = item.fixedNotifiedValueOrRetailPrice;
                Row_Detail["salesTaxApplicable"] = item.salesTaxApplicable;
                Row_Detail["salesTaxWithheldAtSource"] = item.salesTaxWithheldAtSource;

                Row_Detail["extraTax"] = item.extraTax;
                Row_Detail["furtherTax"] = item.furtherTax;
                Row_Detail["sroScheduleNo"] = item.sroScheduleNo;
                Row_Detail["fedPayable"] = item.fedPayable;

                Row_Detail["discount"] = item.discount;
                Row_Detail["saleType"] = item.saleType;
                Row_Detail["sroItemSerialNo"] = item.sroItemSerialNo;
                Row_Detail["totalValues"] = item.totalValues;

                Row_Details.Add(Row_Detail);
            }


            // Save Master Row
            // Sabe Details item list
        }

        public DataTable GetDataTable(string tableName)
        {
            DataTable dt = new DataTable();

            try
            {
                // Open connection
                if (MyConnection.State != ConnectionState.Open)
                {
                    MyConnection.Open();
                }

                // Create query
                string query = $"SELECT * FROM {tableName}";

                // Create command
                using (SQLiteCommand cmd = new SQLiteCommand(query, MyConnection))
                {
                    // Create adapter
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        // Fill DataTable
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Close connection
                if (MyConnection.State == ConnectionState.Open)
                {
                    MyConnection.Close();
                }
            }

            return dt;
        }
    }
}
