using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions.AccountBalanceFieldSource
{
    public class FinancialAccountFieldSourceSetting : AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("71C32B30-204D-4CB5-801A-DC1B52FC5208"); }
        }

        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> accountBalanceFieldDefinitions = new List<AccountBalanceFieldDefinition> {
                new AccountBalanceFieldDefinition{
                    Name = "CreditLimit",
                    Title = "Credit Limit",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },

                }, 
            };
            return accountBalanceFieldDefinitions;
        }
        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            return FinancialAccountBalanceManager.GetAccountBEDefinitionIdByAccountTypeId(context.AccountTypeId);
        }
        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            if(context.FieldName != null)
            {
                context.PreparedData.ThrowIfNull(" context.PreparedData");
                Guid accountBEDefinitionId = Guid.Parse(context.PreparedData.ToString());
                switch(context.FieldName)
                {
                    case "CreditLimit":
                        FinancialAccountManager financialAccountManager = new FinancialAccountManager();

                        var financialAccountData = financialAccountManager.GetFinancialAccountData(accountBEDefinitionId, context.AccountBalance.AccountId);
                        financialAccountData.ThrowIfNull("financialAccountData");
                        return financialAccountData.CreditLimit;
                }
            }
            return null;
        }

       
    }
}
