using System;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule
{
    public class PercentageBalanceAlertThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId { get { return new Guid("30B37A0A-63D8-4323-899B-3A2782FC5A05"); } }

        public Decimal Percentage { get; set; }
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();
        static CurrencyExchangeRateManager s_currencyExRateManager = new CurrencyExchangeRateManager();
        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            Guid balanceAccountTypeId = s_financialAccountManager.GetBalanceAccountTypeIdByAlertRuleTypeId(context.AlertRuleTypeId);
            Guid accountBEDefinitionId;
            long accountId;
            FinancialAccountData financialAccountData;
            Decimal creditLimit;
            int creditLimitCurrencyId;
            if (!s_financialAccountManager.TryGetBalanceAccountCreditLimit(balanceAccountTypeId, context.EntityBalanceInfo.EntityId, out accountBEDefinitionId, out accountId, out financialAccountData, out creditLimit, out creditLimitCurrencyId))
                {
                    throw new Exception(String.Format("Financial Account '{0}' has no credit limit, Balance Account Type Id '{1}', Retail AccountId '{2}'", context.EntityBalanceInfo.EntityId, balanceAccountTypeId, accountId));
                }
            creditLimit = s_currencyExRateManager.ConvertValueToCurrency(creditLimit, creditLimitCurrencyId, context.EntityBalanceInfo.CurrencyId, DateTime.Now);

            return -(creditLimit * (100 - this.Percentage) / 100);
        }
    }
}
