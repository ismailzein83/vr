using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;


namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            return new List<RawMemoryRecord>();

            var businessPartnerInfos = new List<BusinessPartnerInfo>();
            var carrierAccountManager = new CarrierAccountManager();
            var carrierAccounts = carrierAccountManager.GetAllCarriers();

            foreach (var carrierAccount in carrierAccounts)
            {
                BusinessPartnerInfo businessPartner = new BusinessPartnerInfo
                {
                    CarrierAccountId = carrierAccount.CarrierAccountId
                };
                var companySettings = carrierAccountManager.GetCompanySetting(carrierAccount.CarrierAccountId);
                businessPartner.CompanySettingId = companySettings.CompanySettingId;

                WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
                if (carrierAccount.AccountType == CarrierAccountType.Customer)
                {
                    businessPartner.IsCustomer = 1;
                    if (financialAccountManager.TryGetCustAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out var financialAccountData))
                    {
                        businessPartner.CustomerFinancialAccountId = financialAccountData.FinancialAccount.FinancialAccountId;
                    }
                }
                else if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                {
                    businessPartner.IsExchange = 1;
                    if (financialAccountManager.TryGetSuppAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out var financialAccountData))
                    {
                        businessPartner.SupplierFinancialAccountId = financialAccountData.FinancialAccount.FinancialAccountId;
                    }
                }
                else
                {
                    businessPartner.IsExchange = 1;
                }
                businessPartnerInfos.Add(businessPartner);
            }

            return GetRawMemoryRecords(businessPartnerInfos);
        }

        public List<RawMemoryRecord> GetRawMemoryRecords(List<BusinessPartnerInfo> businessPartnerInfos)
        {
            var rawMemoryRecords = new List<RawMemoryRecord>();
            foreach (var businessPartnerInfo in businessPartnerInfos)
            {
                RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
                rawMemoryRecord.FieldValues.Add("CarrierAccountId", businessPartnerInfo.CarrierAccountId);
                rawMemoryRecord.FieldValues.Add("IsCustomer", businessPartnerInfo.IsCustomer);
                rawMemoryRecord.FieldValues.Add("IsSupplier", businessPartnerInfo.IsSupplier);
                rawMemoryRecord.FieldValues.Add("IsExchange", businessPartnerInfo.IsExchange);
                rawMemoryRecord.FieldValues.Add("CompanySettingId", businessPartnerInfo.CompanySettingId);
                rawMemoryRecord.FieldValues.Add("CustomerFinancialAccountId", businessPartnerInfo.CustomerFinancialAccountId);
                rawMemoryRecord.FieldValues.Add("SupplierFinancialAccountId", businessPartnerInfo.SupplierFinancialAccountId);
                rawMemoryRecords.Add(rawMemoryRecord);
            }
            return rawMemoryRecords;
        }
    }
    public class BusinessPartnerInfo
    {
        public int CarrierAccountId { get; set; }
        public int IsCustomer { get; set; }
        public int IsSupplier { get; set; }
        public int IsExchange { get; set; }
        public Guid CompanySettingId { get; set; }
        public int CustomerFinancialAccountId { get; set; }
        public int SupplierFinancialAccountId { get; set; }
    }
}
