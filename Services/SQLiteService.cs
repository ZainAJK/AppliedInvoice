using System.Data.SQLite;
using System.Data;
using AppliedInvoice.Models;

namespace AppliedInvoice.Services
{
    public class SQLiteService
    {
        public SQLiteConnection MyConnection { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Logs { get; set; } = new();

        public SQLiteService(IWebHostEnvironment env)
        {
            var dbPath = Path.Combine(env.WebRootPath, "DB", "Invoice.db");
            MyConnection = new SQLiteConnection($"Data Source={dbPath};");
        }

        // ✅ SAVE INVOICE
        public long SaveInvoice(InvoiceMaster invoice)
        {
            long masterId = 0;

            try
            {
                if (MyConnection.State != ConnectionState.Open)
                    MyConnection.Open();

                using (var transaction = MyConnection.BeginTransaction())
                {
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

                    using (SQLiteCommand cmd = new SQLiteCommand(masterQuery, MyConnection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@invoiceType", invoice.invoiceType ?? "");
                        cmd.Parameters.AddWithValue("@invoiceDate", invoice.invoiceDate);
                        cmd.Parameters.AddWithValue("@invoiceRefNo", invoice.invoiceRefNo ?? "");
                        cmd.Parameters.AddWithValue("@scenarioId", invoice.scenarioId ?? "");

                        cmd.Parameters.AddWithValue("@sellerNTNCNIC", invoice.sellerNTNCNIC ?? "");
                        cmd.Parameters.AddWithValue("@sellerBusinessName", invoice.sellerBusinessName ?? "");
                        cmd.Parameters.AddWithValue("@sellerProvince", invoice.sellerProvince ?? "");
                        cmd.Parameters.AddWithValue("@sellerAddress", invoice.sellerAddress ?? "");

                        cmd.Parameters.AddWithValue("@buyerNTNCNIC", invoice.buyerNTNCNIC ?? "");
                        cmd.Parameters.AddWithValue("@buyerBusinessName", invoice.buyerBusinessName ?? "");
                        cmd.Parameters.AddWithValue("@buyerProvince", invoice.buyerProvince ?? "");
                        cmd.Parameters.AddWithValue("@buyerRegisterationType", invoice.buyerRegisterationType ?? "");
                        cmd.Parameters.AddWithValue("@buyerAddress", invoice.buyerAddress ?? "");

                        masterId = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // DETAILS
                    string detailQuery = @"INSERT INTO InvoiceDetails
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

                    foreach (var item in invoice.items)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(detailQuery, MyConnection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@InvoiceMasterId", masterId);
                            cmd.Parameters.AddWithValue("@hsCode", item.hsCode ?? "");
                            cmd.Parameters.AddWithValue("@productDescription", item.productDescription ?? "");
                            cmd.Parameters.AddWithValue("@rate", item.rate); // ✅ decimal
                            cmd.Parameters.AddWithValue("@uoM", item.uoM ?? "");
                            cmd.Parameters.AddWithValue("@quantity", item.quantity);

                            cmd.Parameters.AddWithValue("@valueSalesExcludingsST", item.valueSalesExcludingsST);
                            cmd.Parameters.AddWithValue("@fixedNotifiedValueOrRetailPrice", item.fixedNotifiedValueOrRetailPrice);
                            cmd.Parameters.AddWithValue("@salesTaxApplicable", item.salesTaxApplicable);
                            cmd.Parameters.AddWithValue("@salesTaxWithheldAtSource", item.salesTaxWithheldAtSource);

                            cmd.Parameters.AddWithValue("@extraTax", item.extraTax);
                            cmd.Parameters.AddWithValue("@furtherTax", item.furtherTax);
                            cmd.Parameters.AddWithValue("@sroScheduleNo", item.sroScheduleNo);
                            cmd.Parameters.AddWithValue("@fedPayable", item.fedPayable);

                            cmd.Parameters.AddWithValue("@discount", item.discount);
                            cmd.Parameters.AddWithValue("@saleType", item.saleType ?? "");
                            cmd.Parameters.AddWithValue("@sroItemSerialNo", item.sroItemSerialNo ?? "");
                            cmd.Parameters.AddWithValue("@totalValues", item.totalValues);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex.Message);
            }
            finally
            {
                if (MyConnection.State == ConnectionState.Open)
                    MyConnection.Close();
            }

            return masterId;
        }

        // ✅ SAVE FBR RESPONSE
        public long FBRResponseSave(FbrResponse apiResponse, long invoiceMasterId)
        {
            long id = 0;

            try
            {
                if (MyConnection.State != ConnectionState.Open)
                    MyConnection.Open();

                string query = @"
                INSERT INTO FbrResponse
                (InvoiceMasterId, statusCode, status, message, invoiceNumber, qrCode, errorDetails)
                VALUES
                (@InvoiceMasterId, @statusCode, @status, @message, @invoiceNumber, @qrCode, @errorDetails);

                SELECT last_insert_rowid();";

                using (SQLiteCommand cmd = new SQLiteCommand(query, MyConnection))
                {
                    cmd.Parameters.AddWithValue("@InvoiceMasterId", invoiceMasterId);
                    cmd.Parameters.AddWithValue("@statusCode", apiResponse.statusCode ?? "");
                    cmd.Parameters.AddWithValue("@status", apiResponse.status ?? "");
                    cmd.Parameters.AddWithValue("@message", apiResponse.message ?? "");
                    cmd.Parameters.AddWithValue("@invoiceNumber", apiResponse.invoiceNumber ?? "");
                    cmd.Parameters.AddWithValue("@qrCode", apiResponse.qrCode ?? "");
                    cmd.Parameters.AddWithValue("@errorDetails", apiResponse.errorDetails ?? "");

                    id = Convert.ToInt64(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex.Message);
            }
            finally
            {
                if (MyConnection.State == ConnectionState.Open)
                    MyConnection.Close();
            }

            return id;
        }
    }
}