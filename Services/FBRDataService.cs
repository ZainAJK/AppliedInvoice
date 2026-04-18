using AppliedInvoice.Logic.Enums;
using AppliedInvoice.Logic.Models;
using System.Data.SQLite;

namespace AppliedInvoice.Services
{
    public class FBRDataService
    {
        public SQLiteConnection MyConnection { get; set; }
        public FBRDataService(IWebHostEnvironment env)
        {
            var dbPath = Path.Combine(
                env.WebRootPath,
                "DB",
                "FBRData.db"
            );

            MyConnection = new SQLiteConnection($"Data Source={dbPath};");
        }
        public FBRDataService()
        {
            var dbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "DB",
                "FBRData.db"
            );

            MyConnection = new SQLiteConnection($"Data Source={dbPath};");
        }

        #region Buyer
        public BuyerProfileModel? GetBuyerProfile(long _ExtID)
        {
            MyConnection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM BuyerProfile WHERE ExtID = @ExgtID", MyConnection);
            command.Parameters.AddWithValue("@ExtID", _ExtID);
            SQLiteDataReader reader = command.ExecuteReader();
            BuyerProfileModel buyerProfile = null!;
            if (reader.Read())
            {
                buyerProfile = new BuyerProfileModel
                {
                    buyerNTNCNIC = reader["NTN_CNIC"].ToString(),
                    buyerBusinessName = reader["Name"].ToString(),
                    buyerProvince = Enum.TryParse<AppEnums.Provience>(reader["Province"].ToString(), out var province) ? province : default,
                    buyerAddress = reader["Address"].ToString()
                };
            }
            MyConnection.Close();
            return buyerProfile;
        }

        public void SaveBuyerProfile(BuyerProfileModel buyerProfile)
        {
            MyConnection.Open();
            SQLiteCommand command = new SQLiteCommand("INSERT INTO BuyerProfile (ExtID, NTN_CNIC, Name, Province, Address, RegistrationType) VALUES (@ExtID, @NTN_CNIC, @Name, @Province, @Address, @RegistrationType)", MyConnection);
            command.Parameters.AddWithValue("@ExtID", buyerProfile.buyerNTNCNIC);
            command.Parameters.AddWithValue("@NTN_CNIC", buyerProfile.buyerNTNCNIC);
            command.Parameters.AddWithValue("@Name", buyerProfile.buyerBusinessName);
            command.Parameters.AddWithValue("@Province", buyerProfile.buyerProvince.ToString());
            command.Parameters.AddWithValue("@Address", buyerProfile.buyerAddress);
            command.Parameters.AddWithValue("@RegistrationType", buyerProfile.buyerRegisterationType.ToString());
            command.ExecuteNonQuery();
            MyConnection.Close();
        }
        #endregion

        #region Seller
        public SellerProfileModel? GetSellerProfile(long _ExtID)
        {
            MyConnection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM SellerProfile WHERE ExtID = @ExtID", MyConnection);
            command.Parameters.AddWithValue("@ExtID", _ExtID);
            SQLiteDataReader reader = command.ExecuteReader();
            SellerProfileModel sellerProfile = null!;
            if (reader.Read())
            {
                sellerProfile = new SellerProfileModel
                {
                    sellerNTNCNIC = reader["NTN_CNIC"].ToString(),
                    sellerBusinessName = reader["Name"].ToString(),
                    sellerProvince = Enum.TryParse<AppEnums.Provience>(reader["Province"].ToString(), out var province) ? province : default,
                    sellerAddress = reader["Address"].ToString()
                };
            }
            MyConnection.Close();
            return sellerProfile;
        }

        public void SaveSellerProfile(SellerProfileModel SellerProfile)
        {
            MyConnection.Open();
            SQLiteCommand command = new SQLiteCommand("INSERT INTO SellerProfile (ExtID, NTN_CNIC, Name, Province, Address, RegistrationType) VALUES (@ExtID, @NTN_CNIC, @Name, @Province, @Address, @RegistrationType)", MyConnection);
            command.Parameters.AddWithValue("@ExtID", SellerProfile.sellerNTNCNIC);
            command.Parameters.AddWithValue("@NTN_CNIC", SellerProfile.sellerNTNCNIC);
            command.Parameters.AddWithValue("@Name", SellerProfile.sellerBusinessName);
            command.Parameters.AddWithValue("@Province", SellerProfile.sellerProvince.ToString());
            command.Parameters.AddWithValue("@Address", SellerProfile.sellerAddress);
            command.Parameters.AddWithValue("@RegistrationType", SellerProfile.sellerRegisterationType.ToString());
            command.ExecuteNonQuery();
            MyConnection.Close();
        }
        #endregion
    }
}
