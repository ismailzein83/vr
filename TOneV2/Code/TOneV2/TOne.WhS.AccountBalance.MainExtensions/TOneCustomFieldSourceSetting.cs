using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
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
            return definitionFields;
        }

        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
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
                    toneCustomFieldsDataByAccountId.Add(item.AccountId, new TOneCustomFieldsData
                    {
                        CustomerCreditLimit =customerCreditLimit,
                        CustomerTolerance= customerCreditLimit.HasValue ? (customerCreditLimit.Value + item.CurrentBalance) : item.CurrentBalance,
                        SupplierCreditLimit=supplierCreditLimit,
                        SupplierTolerance = supplierCreditLimit.HasValue ? (supplierCreditLimit.Value - item.CurrentBalance) : item.CurrentBalance,
                        Consumed = consumed
                    });
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
        public class TOneCustomFieldsData
        {
            public decimal? CustomerCreditLimit { get; set; }
            public decimal  CustomerTolerance { get; set; }
            public decimal Consumed { get; set; }
            public decimal? SupplierCreditLimit { get; set; }
            public decimal  SupplierTolerance { get; set; }
        }
    }

}
