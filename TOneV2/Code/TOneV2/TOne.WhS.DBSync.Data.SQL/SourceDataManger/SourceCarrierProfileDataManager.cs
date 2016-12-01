using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierProfileDataManager : BaseSQLDataManager
    {
        public SourceCarrierProfileDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierProfile> GetSourceCarrierProfiles()
        {
            return GetItemsText(query_getSourceCarrierProfiles, SourceCarrierProfileMapper, null);
        }

        private SourceCarrierProfile SourceCarrierProfileMapper(IDataReader reader)
        {
            SourceCarrierProfile sourceCarrierProfile = new SourceCarrierProfile()
            {
                SourceId = reader["ProfileID"].ToString(),
                Name = reader["Name"] as string,
                AccountManagerContact = reader["AccountManagerContact"].ToString(),
                AccountManagerEmail = reader["AccountManagerEmail"].ToString(),
                Address1 = reader["Address1"] as string,
                Address2 = reader["Address2"] as string,
                Address3 = reader["Address3"] as string,
                BillingContact = reader["BillingContact"].ToString(),
                BillingDisputeEmail = reader["BillingDisputeEmail"].ToString(),
                BillingEmail = reader["BillingEmail"].ToString(),
                CommercialContact = reader["CommercialContact"].ToString(),
                CommercialEmail = reader["CommercialEmail"].ToString(),
                CompanyLogo = GetReaderValue<byte[]>(reader, "CompanyLogo"),
                CompanyLogoName = reader["CompanyLogoName"] as string,
                CompanyName = reader["CompanyName"] as string,
                Country = reader["Country"] as string,
                Fax = reader["Fax"] as string,
                PricingContact = reader["PricingContact"].ToString(),
                PricingEmail = reader["PricingEmail"].ToString(),
                RegistrationNumber = reader["RegistrationNumber"] as string,
                SMSPhoneNumber = reader["SMSPhoneNumber"].ToString(),
                SupportContact = reader["SupportContact"].ToString(),
                SupportEmail = reader["SupportEmail"].ToString(),
                TechnicalContact = reader["TechnicalContact"].ToString(),
                TechnicalEmail = reader["TechnicalEmail"].ToString(),
                Telephone = reader["Telephone"] as string,
                Website = reader["Website"] as string,
                IsDeleted = (reader["IsDeleted"] as string) != "N",
                CurrencyId = reader["CurrencyID"] as string,
                DuePeriod = GetReaderValue<byte>(reader, "DuePeriod")
            };
            return sourceCarrierProfile;
        }

        const string query_getSourceCarrierProfiles = @"SELECT [ProfileID] ,[Name] ,[CompanyName]  ,[CompanyLogo]  ,[CompanyLogoName]  , 
                                                               [Address1]  ,[Address2]  ,[Address3]  ,[Country]  ,[Telephone] , 
                                                               [Fax]   ,[BillingContact]  ,[BillingEmail]   ,[PricingContact] ,[PricingEmail],  
                                                               [SupportContact]   ,[SupportEmail] ,[CurrencyID]  ,[RegistrationNumber]   , 
                                                               [AccountManagerEmail]   ,[SMSPhoneNumber] ,[Website]  ,[BillingDisputeEmail]  , 
                                                               [TechnicalContact] ,[TechnicalEmail]   ,[CommercialContact]   ,[CommercialEmail] , 
                                                               [AccountManagerContact], [IsDeleted], [CurrencyID], [DuePeriod] FROM [dbo].[CarrierProfile]  
                                                        WITH (NOLOCK)";
    }
}
