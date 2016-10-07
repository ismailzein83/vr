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

        private SourceCarrierProfile SourceCarrierProfileMapper(IDataReader arg)
        {
            SourceCarrierProfile sourceCarrierProfile = new SourceCarrierProfile()
            {
                SourceId = arg["ProfileID"].ToString(),
                Name = arg["Name"] as string,
                AccountManagerContact = arg["AccountManagerContact"].ToString(),
                AccountManagerEmail = arg["AccountManagerEmail"].ToString(),
                Address1 = arg["Address1"] as string,
                Address2 = arg["Address2"] as string,
                Address3 = arg["Address3"] as string,
                BillingContact = arg["BillingContact"].ToString(),
                BillingDisputeEmail = arg["BillingDisputeEmail"].ToString(),
                BillingEmail = arg["BillingEmail"].ToString(),
                CommercialContact = arg["CommercialContact"].ToString(),
                CommercialEmail = arg["CommercialEmail"].ToString(),
                CompanyLogo = GetReaderValue<byte[]>(arg, "CompanyLogo"),
                CompanyLogoName = arg["CompanyLogoName"] as string,
                CompanyName = arg["CompanyName"] as string,
                Country = arg["Country"] as string,
                Fax = arg["Fax"] as string,
                PricingContact = arg["PricingContact"].ToString(),
                PricingEmail = arg["PricingEmail"].ToString(),
                RegistrationNumber = arg["RegistrationNumber"] as string,
                SMSPhoneNumber = arg["SMSPhoneNumber"].ToString(),
                SupportContact = arg["SupportContact"].ToString(),
                SupportEmail = arg["SupportEmail"].ToString(),
                TechnicalContact = arg["TechnicalContact"].ToString(),
                TechnicalEmail = arg["TechnicalEmail"].ToString(),
                Telephone = arg["Telephone"] as string,
                Website = arg["Website"] as string,
                IsDeleted = (arg["IsDeleted"] as string) != "N"
            };
            return sourceCarrierProfile;
        }

        const string query_getSourceCarrierProfiles = @"SELECT [ProfileID] ,[Name] ,[CompanyName]  ,[CompanyLogo]  ,[CompanyLogoName]  , 
                                                               [Address1]  ,[Address2]  ,[Address3]  ,[Country]  ,[Telephone] , 
                                                               [Fax]   ,[BillingContact]  ,[BillingEmail]   ,[PricingContact] ,[PricingEmail],  
                                                               [SupportContact]   ,[SupportEmail] ,[CurrencyID]  ,[RegistrationNumber]   , 
                                                               [AccountManagerEmail]   ,[SMSPhoneNumber] ,[Website]  ,[BillingDisputeEmail]  , 
                                                               [TechnicalContact] ,[TechnicalEmail]   ,[CommercialContact]   ,[CommercialEmail] , 
                                                               [AccountManagerContact], [IsDeleted] FROM [dbo].[CarrierProfile]  
                                                        WITH (NOLOCK)";
    }
}
