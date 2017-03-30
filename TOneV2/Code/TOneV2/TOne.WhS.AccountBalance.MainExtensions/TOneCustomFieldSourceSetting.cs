using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace TOne.WhS.AccountBalance.MainExtensions
{
    public class TOneCustomFieldSourceSetting : AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("BD8DB941-019D-40C2-9E9A-DEBA0F567878"); }
        }
        CarrierAccountManager carrierAccountManager;
        FinancialAccountManager financialAccountManager;
        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> definitionFields = new List<AccountBalanceFieldDefinition>();
            AccountBalanceSettings accountBalanceSettings = context.AccountTypeSettings.ExtendedSettings as AccountBalanceSettings;
            if (accountBalanceSettings.IsApplicableToCustomer)
            {
                GetCustomerFields(definitionFields);
            }
            if (accountBalanceSettings.IsApplicableToSupplier)
            {
                GetSupplierFields(definitionFields);
            }
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "Consumed",
                Title = "Consumed",
                FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
            });
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "CarrierType",
                Title = "Carrier Type",
                FieldType = new FieldTextType(),
            });
            return definitionFields;
        }

        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            AccountBalanceSettings accountBalanceSettings = context.AccountTypeSettings.ExtendedSettings as AccountBalanceSettings;
             financialAccountManager = new FinancialAccountManager();
             carrierAccountManager = new CarrierAccountManager();
            Dictionary<string, TOneCustomFieldsData> toneCustomFieldsDataByAccountId = new Dictionary<string, TOneCustomFieldsData>();
            foreach(var item in context.AccountBalances)
            {
                int accountId = Convert.ToInt32(item.AccountId);
               

                TOneCustomFieldsData toneCustomFieldsData;
                if (!toneCustomFieldsDataByAccountId.TryGetValue(item.AccountId, out toneCustomFieldsData))
                {
                    var customerCreditLimit =  GetCustomerCreditLimit(accountId);
                    var supplierCreditLimit =  GetSupplierCreditLimit(accountId);
                    decimal consumed = 0;
                    if (customerCreditLimit.HasValue && supplierCreditLimit.HasValue)
                    {
                        var sumOfCreditLimit = customerCreditLimit.Value + supplierCreditLimit.Value;
                        if (sumOfCreditLimit > 0 )
                         consumed = item.CurrentBalance * 100 / sumOfCreditLimit;
                    }
                    else if (customerCreditLimit.HasValue)
                    {
                       if (customerCreditLimit.Value > 0)
                         consumed = item.CurrentBalance * 100 / customerCreditLimit.Value;

                    }
                    else if (supplierCreditLimit.HasValue)
                    {
                        if (customerCreditLimit.Value > 0)
                            consumed = item.CurrentBalance * 100 / supplierCreditLimit.Value;

                    }
                    var financialAccount = financialAccountManager.GetFinancialAccount(accountId);
                    toneCustomFieldsData = new TOneCustomFieldsData
                    {
                        CustomerCreditLimit = customerCreditLimit,
                        CustomerTolerance = customerCreditLimit.HasValue ? (customerCreditLimit.Value + item.CurrentBalance) : item.CurrentBalance,
                        SupplierCreditLimit = supplierCreditLimit,
                        SupplierTolerance = supplierCreditLimit.HasValue ? (supplierCreditLimit.Value - item.CurrentBalance) : item.CurrentBalance,
                        Consumed = consumed,
                        CarrierType = financialAccount.CarrierAccountId.HasValue? "Account":"Profile"
                    };
                    
                     if (accountBalanceSettings.IsApplicableToCustomer)
                     {
                         if (IsRoutingStatusEnabled(financialAccount, true))
                             toneCustomFieldsData.CustomerRoutingStatus = Vanrise.Common.Utilities.GetEnumDescription(RoutingStatus.Enabled);
                         else
                             toneCustomFieldsData.CustomerRoutingStatus = Vanrise.Common.Utilities.GetEnumDescription(RoutingStatus.Blocked);
                        
                     }
                     if (accountBalanceSettings.IsApplicableToSupplier)
                     {
                         if (IsRoutingStatusEnabled(financialAccount, false))
                             toneCustomFieldsData.SupplierRoutingStatus = Vanrise.Common.Utilities.GetEnumDescription(RoutingStatus.Enabled);
                         else
                             toneCustomFieldsData.SupplierRoutingStatus = Vanrise.Common.Utilities.GetEnumDescription(RoutingStatus.Blocked);
                     }
                     toneCustomFieldsDataByAccountId.Add(item.AccountId,toneCustomFieldsData);
                }
            }
            return toneCustomFieldsDataByAccountId;
        }
        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            if(context.FieldName == null)
                throw new NullReferenceException("context.FieldName");
            Dictionary<string, TOneCustomFieldsData> toneCustomFieldsDataByAccountId = context.PreparedData as Dictionary<string, TOneCustomFieldsData>;

            TOneCustomFieldsData toneCustomFieldsData;
            if (toneCustomFieldsDataByAccountId.TryGetValue(context.AccountBalance.AccountId, out toneCustomFieldsData))
            {
                switch (context.FieldName)
                {
                    case "CutomerCreditLimit": return toneCustomFieldsData.CustomerCreditLimit;
                    case "CutomerTolerance": return toneCustomFieldsData.CustomerTolerance;
                    case "SupplierCreditLimit": return toneCustomFieldsData.SupplierCreditLimit;
                    case "SupplierTolerance": return toneCustomFieldsData.SupplierTolerance;
                    case "Consumed": return toneCustomFieldsData.Consumed;
                    case "CustomerRoutingStatus": return toneCustomFieldsData.CustomerRoutingStatus;
                    case "SupplierRoutingStatus": return toneCustomFieldsData.SupplierRoutingStatus;
                    case "CarrierType": return toneCustomFieldsData.CarrierType;

                }
            }
            return null;
        }
        private void GetCustomerFields(List<AccountBalanceFieldDefinition> definitionFields)
        {
            if (definitionFields == null)
                definitionFields = new List<AccountBalanceFieldDefinition>();

            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "CutomerCreditLimit",
                Title = "Cutomer Credit Limit",
                FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
            });
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "CutomerTolerance",
                Title = "Cutomer Tolerance",
                FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
            });
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "CustomerRoutingStatus",
                Title = "Customer Routing Status",
                FieldType = new FieldTextType (),
            });
        }
        private void GetSupplierFields(List<AccountBalanceFieldDefinition> definitionFields)
        {
            if (definitionFields == null)
                definitionFields = new List<AccountBalanceFieldDefinition>();
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "SupplierCreditLimit",
                Title = "Supplier Credit Limit",
                FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
            });
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "SupplierTolerance",
                Title = "Supplier Tolerance",
                FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
            });
            definitionFields.Add(new AccountBalanceFieldDefinition
            {
                Name = "SupplierRoutingStatus",
                Title = "Supplier Routing Status",
                FieldType = new FieldTextType(),
            });
        }
        private decimal? GetCustomerCreditLimit(int accountId)
        {
             FinancialAccountManager financialAccountManager = new FinancialAccountManager();
             CarrierFinancialAccountData carrierFinancialAccountData =  financialAccountManager.GetCustCarrierFinancialByFinAccId(accountId);
            return carrierFinancialAccountData.CreditLimit;
        }
        private decimal? GetSupplierCreditLimit(int accountId)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            CarrierFinancialAccountData carrierFinancialAccountData = financialAccountManager.GetSuppCarrierFinancialByFinAccId(accountId);
            return carrierFinancialAccountData.CreditLimit;
        }

        private bool IsRoutingStatusEnabled(FinancialAccount financialAccount, bool isCustomerApplicable)
        {

            if (financialAccount.CarrierAccountId.HasValue)
            {
                var carrierAccount = carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                if (isCustomerApplicable)
                {
                    return IsCustomerRoutingStatusEnabled(carrierAccount);
                }
                else if (!isCustomerApplicable)
                {
                    return IsSupplierRoutingStatusEnabled(carrierAccount);
                }
            }
            else
            {
                var carrierAccounts = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, isCustomerApplicable, !isCustomerApplicable);
                bool routingStatus = false;
                foreach (var account in carrierAccounts)
                {
                    if (isCustomerApplicable)
                    {
                        routingStatus = IsCustomerRoutingStatusEnabled(account);
                        if (routingStatus)
                            break;
                    }
                    else if (!isCustomerApplicable)
                    {
                        routingStatus = IsSupplierRoutingStatusEnabled(account);
                        if (routingStatus)
                            break;
                    }
                }
                return routingStatus;
            }
            return false;
        }
        private bool IsCustomerRoutingStatusEnabled(CarrierAccount carrierAccount)
        {
            if (carrierAccount.CustomerSettings.RoutingStatus == RoutingStatus.Enabled)
                return true;
            return false;

        }
        private bool IsSupplierRoutingStatusEnabled(CarrierAccount carrierAccount)
        {
            if (carrierAccount.SupplierSettings.RoutingStatus == RoutingStatus.Enabled)
                return true;
            return false;
        }
        public class TOneCustomFieldsData
        {
            public decimal? CustomerCreditLimit { get; set; }
            public decimal  CustomerTolerance { get; set; }
            public decimal Consumed { get; set; }
            public decimal? SupplierCreditLimit { get; set; }
            public decimal  SupplierTolerance { get; set; }
            public string CustomerRoutingStatus { get; set; }
            public string SupplierRoutingStatus{ get; set; }
            public string CarrierType { get; set; }

        }
    }

}
