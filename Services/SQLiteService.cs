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

        public void SaveInvoice(FbrInvoice invoice)
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

                Row_Detail["valueSalesExcludingsST"] = item.valueSalesExcludingST;
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

            try
            {
                if (MyConnection.State != ConnectionState.Open)
                    MyConnection.Open();

                // ===== INSERT MASTER =====
                string masterQuery = @"
                     INSERT INTO InvoiceMaster
                (invoiceType, invoiceDate, invoiceRefNo, scenarioId,
                 sellerNTNCNIC, sellerBusinessName, sellerProvince, sellerAddress,
                 buyerNTNCNIC, buyerBusinessName, buyerProvince, buyerRegisterationType, buyerAddress)
                VALUES
                (@invoiceType, @invoiceDate, @invoiceRefNo, @scenarioId,
                 @sellerNTNCNIC, @sellerBusinessName, @sellerProvince, @sellerAddress,
                 @buyerNTNCNIC, @buyerBusinessName, @buyerProvince, @buyerRegisterationType, @buyerAddress);
    
                SELECT last_insert_rowid();";

                long masterId;

                using (SQLiteCommand cmd = new SQLiteCommand(masterQuery, MyConnection))
                {
                    cmd.Parameters.AddWithValue("@invoiceType", invoice.invoiceType);
                    cmd.Parameters.AddWithValue("@invoiceDate", invoice.invoiceDate);
                    cmd.Parameters.AddWithValue("@invoiceRefNo", invoice.invoiceRefNo);
                    cmd.Parameters.AddWithValue("@scenarioId", invoice.scenarioId);

                    cmd.Parameters.AddWithValue("@sellerNTNCNIC", invoice.sellerNTNCNIC);
                    cmd.Parameters.AddWithValue("@sellerBusinessName", invoice.sellerBusinessName);
                    cmd.Parameters.AddWithValue("@sellerProvince", invoice.sellerProvince);
                    cmd.Parameters.AddWithValue("@sellerAddress", invoice.sellerAddress);

                    cmd.Parameters.AddWithValue("@buyerNTNCNIC", invoice.buyerNTNCNIC);
                    cmd.Parameters.AddWithValue("@buyerBusinessName", invoice.buyerBusinessName);
                    cmd.Parameters.AddWithValue("@buyerProvince", invoice.buyerProvince);
                    cmd.Parameters.AddWithValue("@buyerRegisterationType", invoice.buyerRegisterationType);
                    cmd.Parameters.AddWithValue("@buyerAddress", invoice.buyerAddress);

                    masterId = (long)cmd.ExecuteScalar();
                }

                // ===== INSERT DETAILS =====
                foreach (var item in invoice.items)
                {
                    string detailQuery = @"
                    INSERT INTO InvoiceDetails
                    (InvoiceMasterId, hsCode, productDescription, rate, uoM, quantity,
                     valueSalesExcludingsST, fixedNotifiedValueOrRetailPrice,
                     salesTaxApplicable, salesTaxWithheldAtSource,
                     extraTax, furtherTax, sroScheduleNo, fedPayable,
                     discount, saleType, sroItemSerialNo, totalValues)
                    VALUES
                    (@InvoiceMasterId, @hsCode, @productDescription, @rate, @uoM, @quantity,
                     @valueSalesExcludingsST, @fixedNotifiedValueOrRetailPrice,
                     @salesTaxApplicable, @salesTaxWithheldAtSource,
                     @extraTax, @furtherTax, @sroScheduleNo, @fedPayable,
                     @discount, @saleType, @sroItemSerialNo, @totalValues);";

                    using (SQLiteCommand cmd = new SQLiteCommand(detailQuery, MyConnection))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceMasterId", masterId);

                        cmd.Parameters.AddWithValue("@hsCode", item.hsCode);
                        cmd.Parameters.AddWithValue("@productDescription", item.productDescription);
                        cmd.Parameters.AddWithValue("@rate", item.rate);
                        cmd.Parameters.AddWithValue("@uoM", item.uoM);
                        cmd.Parameters.AddWithValue("@quantity", item.quantity);

                        cmd.Parameters.AddWithValue("@valueSalesExcludingST", item.valueSalesExcludingST);
                        cmd.Parameters.AddWithValue("@fixedNotifiedValueOrRetailPrice", item.fixedNotifiedValueOrRetailPrice);
                        cmd.Parameters.AddWithValue("@salesTaxApplicable", item.salesTaxApplicable);
                        cmd.Parameters.AddWithValue("@salesTaxWithheldAtSource", item.salesTaxWithheldAtSource);

                        cmd.Parameters.AddWithValue("@extraTax", item.extraTax);
                        cmd.Parameters.AddWithValue("@furtherTax", item.furtherTax);
                        cmd.Parameters.AddWithValue("@sroScheduleNo", item.sroScheduleNo);
                        cmd.Parameters.AddWithValue("@fedPayable", item.fedPayable);

                        cmd.Parameters.AddWithValue("@discount", item.discount);
                        cmd.Parameters.AddWithValue("@saleType", item.saleType);
                        cmd.Parameters.AddWithValue("@sroItemSerialNo", item.sroItemSerialNo);
                        cmd.Parameters.AddWithValue("@totalValues", item.totalValues);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Save Error: " + ex.Message);
            }
            finally
            {
                if (MyConnection.State == ConnectionState.Open)
                    MyConnection.Close();
            }
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
