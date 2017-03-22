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
            AccountBalanceSettings accountBalanceSettings = context.ExtendedSettings as AccountBalanceSettings;
            if (accountBalanceSettings.IsApplicableToCustomer)
            {
                GetCustomerFields(definitionFields);
            }
            if (accountBalanceSettings.IsApplicableToSupplier)
            {
                GetSupplierFields(definitionFields);
            }
            return definitionFields;
        }

        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
         
            return null;
        }

        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            if(context.FieldName == null)
                throw new NullReferenceException("context.FieldName");
              
            switch(context.FieldName)
            {
                case "CutomerCreditLimit": return GetCustomerCreditLimit(Convert.ToInt32(context.AccountBalance.AccountId));
                case "CutomerTolerance":
                    var customerCreditLimit = GetCustomerCreditLimit(Convert.ToInt32(context.AccountBalance.AccountId));
                    if (customerCreditLimit.HasValue)
                        return customerCreditLimit.Value + context.AccountBalance.CurrentBalance;
                    return context.AccountBalance.CurrentBalance;
                case "SupplierCreditLimit": return GetSupplierCreditLimit(Convert.ToInt32(context.AccountBalance.AccountId));
                case "SupplierTolerance":
                    var supplierCreditLimit = GetSupplierCreditLimit(Convert.ToInt32(context.AccountBalance.AccountId));
                    if (supplierCreditLimit.HasValue)
                        return supplierCreditLimit.Value - context.AccountBalance.CurrentBalance;
                    return context.AccountBalance.CurrentBalance;
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
    }

}
