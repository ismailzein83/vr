using System;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;


namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
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

                var financialAccountManager = new WHSFinancialAccountManager();
                var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();

                Guid accountDefinitionId = new Guid();
                if (carrierAccount.AccountType == CarrierAccountType.Customer)
                {
                    businessPartner.IsCustomer = 1;
                    if (financialAccountManager.TryGetCustAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out var financialAccountData))
                        accountDefinitionId = financialAccountData.FinancialAccount.FinancialAccountDefinitionId;
                }
                else if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                {
                    businessPartner.IsSupplier = 1;
                    if (financialAccountManager.TryGetSuppAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out var financialAccountData))
                        accountDefinitionId = financialAccountData.FinancialAccount.FinancialAccountDefinitionId;
                }
                else
                    businessPartner.IsExchange = 1;

                AccountDefinitionType? accountDefinitionType = financialAccountDefinitionManager.GetAccountDefinitionType(accountDefinitionId);
                if (accountDefinitionType != null)
                {
                    switch (accountDefinitionType)
                    {
                        case AccountDefinitionType.CustomerPostpaid:
                            businessPartner.CustomerFinancialAccountType = 0;
                            break;
                        case AccountDefinitionType.CustomerPrepaid:
                            businessPartner.CustomerFinancialAccountType = 1;
                            break;
                        case AccountDefinitionType.SupplierPostpaid:
                            businessPartner.SupplierFinancialAccountType = 0;
                            break;
                        case AccountDefinitionType.SupplierPrepaid:
                            businessPartner.SupplierFinancialAccountType = 1;
                            break;
                        case AccountDefinitionType.Netting:
                            businessPartner.SupplierFinancialAccountType = 2;
                            break;
                    }
                }
                businessPartnerInfos.Add(businessPartner);
            }

            return GetRawMemoryRecords(businessPartnerInfos);
        }

        private List<RawMemoryRecord> GetRawMemoryRecords(List<BusinessPartnerInfo> businessPartnerInfos)
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
                rawMemoryRecord.FieldValues.Add("CustomerFinancialAccountType", businessPartnerInfo.CustomerFinancialAccountType);
                rawMemoryRecord.FieldValues.Add("SupplierFinancialAccountType", businessPartnerInfo.SupplierFinancialAccountType);
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
        public int? CustomerFinancialAccountType { get; set; }
        public int? SupplierFinancialAccountType { get; set; }
    }
}
