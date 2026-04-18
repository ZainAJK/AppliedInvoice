using AppliedInvoice;
using AppliedInvoice.Logic;
using AppliedInvoice.Logic.Models;
using AppliedInvoice.Services;
using Enums = AppliedInvoice.Logic.Enums;



namespace AppliedInvoice.Models
{
    public class InvoiceMigration
    {
        public FBRRequestModel.Voucher? AppliedVoucher { get; set; }
        public SellerProfileModel? SellerProfile { get; set; }
        public BuyerProfileModel? BuyerProfile { get; set; }
        public List<ProductItemModel>? InvoiceItems { get; set; }
        public InvoiceMaster ReqInvoice { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public FBRDataService Source { get; set; }

        public InvoiceMigration(FBRRequestModel.Voucher _Voucher)
        {
            Source = new FBRDataService();
            AppliedVoucher = _Voucher;

            if(_Voucher != null)
            {
                var _Buyer = AppliedVoucher.Master.Seller;
                var _Seller = AppliedVoucher.Master.Buyer;

                if(_Buyer > 0)
                {
                    BuyerProfile = Source.GetBuyerProfile(_Buyer);
                    if (BuyerProfile == null) { Errors.Add($"Buyer Profile with ExtID {_Buyer} not found"); }
                }
                else
                {
                    Errors.Add("Invalid Buyer ExtID in Applied Voucher");

                }

            }

        }

        public InvoiceMaster? MigrateInvoice()
        {

            #region Validatation of invoice data
            Errors = new List<string>();
            if (this.ReqInvoice == null) { Errors.Add("Requested Invoice is null"); }
            if (this.InvoiceItems == null) { Errors.Add("Invoice Items are null"); }
            if (this.SellerProfile == null) { Errors.Add("Seller Profile is null"); }
            if (this.BuyerProfile == null) { Errors.Add("Buyer Profile is null"); }
            if (Errors.Count > 0) { return null; }
            #endregion


            ReqInvoice.invoiceType = AppEnums.InvoiceType.Sale_Invoice.ToString();
            ReqInvoice.invoiceDate = AppliedVoucher.Master.Vou_Date;

            ReqInvoice.sellerNTNCNIC = SellerProfile.sellerNTNCNIC;
            ReqInvoice.sellerBusinessName = SellerProfile.sellerBusinessName;
            ReqInvoice.sellerProvince = SellerProfile.sellerProvince.ToString();
            ReqInvoice.sellerAddress = SellerProfile.sellerAddress;

            ReqInvoice.buyerNTNCNIC = BuyerProfile.buyerNTNCNIC;
            ReqInvoice.buyerBusinessName = BuyerProfile.buyerBusinessName;
            ReqInvoice.buyerProvince = BuyerProfile.buyerProvince.ToString();
            ReqInvoice.buyerAddress = BuyerProfile.buyerAddress;
            ReqInvoice.buyerRegisterationType = BuyerProfile.buyerRegisterationType.ToString();

            ReqInvoice.invoiceRefNo = AppliedVoucher.Master.Vou_No;
            ReqInvoice.scenarioId = ((Enums.AppEnums.ScenarioID)Enums.AppEnums.ScenarioType.Standard_rated_supplies).ToString();  // Sale Invoice
            foreach (var item in AppliedVoucher.Details)
            {
                var _Product = InvoiceItems!.FirstOrDefault(p => p.ExtID == item.Inventory);
                if(_Product != null ) { Errors.Add($"Product with ExtID {item.Inventory} not found in InvoiceItems"); return null; }

                var _totalValue = item.Gross;
                var _valueSalesExcludingsST = _totalValue - item.Discount;    // Zero must be discounted value, but currently not available in AppliedAccountsModel.Detail
                var _retailPrice = item.RetailPrice;                          // Currently not available in AppliedAccountsModel.Detail, need to be calculated based on business logic and scenario
                var _salesTaxApplicable = _valueSalesExcludingsST * item.TaxRate;
                var _salesTaxWithheldAtSource = _valueSalesExcludingsST * item.STHoldRate;  // Currently not available in AppliedAccountsModel.Detail, need to be calculated based on business logic and scenario

                InvoiceDetails details = new InvoiceDetails
                {
                    hsCode = _Product!.hsCode,
                    productDescription = _Product!.productDescription,
                    rate = item.Rate,
                    uoM = item.TitleUnit,
                    quantity = item.Qty,
                    totalValues = _totalValue,
                    valueSalesExcludingsST = _valueSalesExcludingsST,    
                    fixedNotifiedValueOrRetailPrice = _retailPrice,                    
                    salesTaxApplicable = _salesTaxApplicable,
                    salesTaxWithheldAtSource = _salesTaxWithheldAtSource,
                    extraTax = item.ExtraTax,
                    furtherTax = item.FurtherTax,
                    sroScheduleNo = item.SROScheduleNo,
                    fedPayable = item.FEDPayable,
                    discount = item.Discount,
                    saleType = item.SaleType,
                    sroItemSerialNo = item.SROItemSerialNo
                };
                ReqInvoice.items.Add(details);
            }
            return ReqInvoice;

        }
    }
}
