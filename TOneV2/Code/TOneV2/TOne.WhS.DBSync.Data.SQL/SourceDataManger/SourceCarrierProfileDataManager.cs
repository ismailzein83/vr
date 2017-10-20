using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using System.Linq;
using System;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierProfileDataManager : BaseSQLDataManager
    {
        public SourceCarrierProfileDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierProfileWithAccounts> GetSourceCarrierProfilesWithFinancialData()
        {
            return GetItemsText(query_getSourceCarrierProfilesWithFinancialData, SourceCarrierProfileWithAccountsMapper, null);
        }

        public List<SourceCarrierProfile> GetSourceCarrierProfiles()
        {
            return GetItemsText(query_getSourceCarrierProfiles, SourceCarrierProfileMapper, null);
        }

        public Dictionary<string, CarrierProfileBlockedStatusEntity> GetProfileIdsWithNoActiveAccounts()
        {
            return GetItemsText(query_getProfileAccountsWithNoActiveCarriers, CarrierProfileBlockedStatusEntityMapper, null).ToDictionary(k => k.ProfileId, k => k);
        }

        CarrierProfileBlockedStatusEntity CarrierProfileBlockedStatusEntityMapper(IDataReader reader)
        {
            return new CarrierProfileBlockedStatusEntity
            {
                ProfileId = reader["ProfileID"].ToString(),
                IsCustomerBlocked = reader["IsCustomerBlocked"].ToString() == "1",
                IsSupplierBlocked = reader["IsSupplierBlocked"].ToString() == "1",
            };

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
                IsDeleted = (reader["IsDeleted"] as string).Equals("Y", System.StringComparison.InvariantCultureIgnoreCase),
                CurrencyId = reader["CurrencyID"] as string,
                DuePeriod = GetReaderValue<byte>(reader, "DuePeriod")
            };
            return sourceCarrierProfile;
        }

        private SourceCarrierProfileWithAccounts SourceCarrierProfileWithAccountsMapper(IDataReader reader)
        {
            string invoiceByProfile = reader["InvoiceByProfile"] as string;
            string isNettingEnabled = reader["IsNettingEnabled"] as string;
            return new SourceCarrierProfileWithAccounts()
            {
                SourceId = reader["ProfileID"].ToString(),
                IsDeleted = (reader["IsDeleted"] as string).Equals("Y", System.StringComparison.InvariantCultureIgnoreCase),
                CustomerActivateDate = GetReaderValue<DateTime?>(reader, "CustomerActivateDate"),
                SupplierActivateDate = GetReaderValue<DateTime?>(reader, "SupplierActivateDate"),
                CustomerDeactivateDate = GetReaderValue<DateTime?>(reader, "CustomerDeactivateDate"),
                SupplierDeactivateDate = GetReaderValue<DateTime?>(reader, "SupplierDeactivateDate"),
                InvoiceByProfile = string.IsNullOrEmpty(invoiceByProfile) || invoiceByProfile.Equals("N") ? false : true,
                IsNettingEnabled = string.IsNullOrEmpty(isNettingEnabled) || isNettingEnabled.Equals("N") ? false : true,
                CustomerCreditLimit = GetReaderValue<int?>(reader, "CustomerCreditLimit"),
                SupplierCreditLimit = GetReaderValue<int?>(reader, "SupplierCreditLimit"),
                CustomerPaymentType = (SourcePaymentType)GetReaderValue<byte>(reader, "CustomerPaymentType"),
                SupplierPaymentType = (SourcePaymentType)GetReaderValue<byte>(reader, "SupplierPaymentType")
            };
        }

        const string query_getSourceCarrierProfiles = @"SELECT [ProfileID] ,[Name] ,[CompanyName]  ,[CompanyLogo]  ,[CompanyLogoName]  , 
                                                               [Address1]  ,[Address2]  ,[Address3]  ,[Country]  ,[Telephone] , 
                                                               [Fax]   ,[BillingContact]  ,[BillingEmail]   ,[PricingContact] ,[PricingEmail],  
                                                               [SupportContact]   ,[SupportEmail] ,[CurrencyID]  ,[RegistrationNumber]   , 
                                                               [AccountManagerEmail]   ,[SMSPhoneNumber] ,[Website]  ,[BillingDisputeEmail]  , 
                                                               [TechnicalContact] ,[TechnicalEmail]   ,[CommercialContact]   ,[CommercialEmail] , 
                                                               [AccountManagerContact], [IsDeleted], [DuePeriod] FROM [dbo].[CarrierProfile]  WITH (NOLOCK)
															   where profileid not in (
                                                                   SELECT distinct cp.[ProfileID]
                                                                   FROM [CarrierProfile] cp with(nolock)
                                                                   join [dbo].carrieraccount ca with(nolock) on ca.profileid = cp.profileid
                                                                   where ca.CarrierAccountID = 'SYS')";

        const string query_getSourceCarrierProfilesWithFinancialData = @"SELECT    [ProfileID]
                                                                                  ,[IsDeleted]
                                                                                  ,[CustomerPaymentType]
                                                                                  ,[SupplierPaymentType]
                                                                                  ,[CustomerCreditLimit]
                                                                                  ,[SupplierCreditLimit]
                                                                                  ,[IsNettingEnabled]
                                                                                  ,[CustomerActivateDate]
                                                                                  ,[CustomerDeactivateDate]
                                                                                  ,[SupplierActivateDate]
                                                                                  ,[SupplierDeactivateDate]
                                                                                  ,[InvoiceByProfile]
                                                                              FROM [dbo].[CarrierProfile] WITH (NOLOCK)
                                                                              where IsDeleted = 'N' and profileid not in (
                                                                                   SELECT distinct cp.[ProfileID]
                                                                                   FROM [CarrierProfile] cp with(nolock)
                                                                                   join [dbo].carrieraccount ca with(nolock) on ca.profileid = cp.profileid
                                                                                   where ca.CarrierAccountID = 'SYS')";


        const string query_getProfileAccountsWithNoActiveCarriers = @";with InActive_Customr as (
                                                            select ca.ProfileID, 1 as IsBlocked
                                                            from   CarrierAccount ca 
                                                            where ca.AccountType = 0 or ca.AccountType = 1
                                                            group by ProfileID
                                                            having   count(*) = SUM(case when ca.ActivationStatus=0 then 1 else 0 END)),
                                                            InActive_Supplier as (
                                                            select ca.ProfileID, 1 as IsBlocked
                                                            from   CarrierAccount ca 
                                                            where ca.AccountType = 2 or ca.AccountType = 1
                                                            group by ProfileID
                                                            having   count(*) = SUM(case when ca.ActivationStatus=0 then 1 else 0 END))
                                                            select isnull(c.ProfileID, s.ProfileID)as ProfileID, isnull(c.IsBlocked,0) as IsCustomerBlocked, isnull(s.IsBlocked,0) as IsSupplierBlocked from InActive_Customr c
                                                            FULL OUTER JOIN  InActive_Supplier s on s.profileid = c.profileid																		   
                                                            order by s.profileid";
    }
}
