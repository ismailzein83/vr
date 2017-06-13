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
            VRAccountBalanceManager vRAccountBalanceManager = new VRAccountBalanceManager();
            var accountBEDefinitionId = vRAccountBalanceManager.GetAccountBEDefinitionIdByAlertRuleTypeId(context.AlertRuleTypeId);

            Decimal creditLimit;
             var financialAccountData = s_financialAccountManager.GetFinancialAccountData(accountBEDefinitionId, context.EntityBalanceInfo.EntityId);
            if (financialAccountData != null)
            {
                if (!financialAccountData.CreditLimit.HasValue)
                    throw new NullReferenceException(String.Format("financialAccountData.CreditLimit '{0}'", context.EntityBalanceInfo.EntityId));
                if (!financialAccountData.CreditLimitCurrencyId.HasValue)
                    throw new NullReferenceException(String.Format("financialAccountData.CreditLimitCurrencyId '{0}'", context.EntityBalanceInfo.EntityId));
                creditLimit = s_currencyExRateManager.ConvertValueToCurrency(financialAccountData.CreditLimit.Value, financialAccountData.CreditLimitCurrencyId.Value, context.EntityBalanceInfo.CurrencyId, DateTime.Now);
            }
            else
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                AccountPart accountPart;

                if (!accountBEManager.TryGetAccountPart(accountBEDefinitionId, Convert.ToInt64(context.EntityBalanceInfo.EntityId), AccountPartFinancial._ConfigId, false, out accountPart))
                    throw new NullReferenceException(String.Format("accountPart. Account '{0}'", context.EntityBalanceInfo.EntityId));
                var accountPartFinancial = accountPart.Settings.CastWithValidate<AccountPartFinancial>("accountPart.Settings", context.EntityBalanceInfo.EntityId);

                ProductManager productManager = new ProductManager();
                var product = productManager.GetProduct(accountPartFinancial.ProductId);
                product.ThrowIfNull("product", accountPartFinancial.ProductId);
                product.Settings.ThrowIfNull("product.Settings", accountPartFinancial.ProductId);

                var postPaidSettings = product.Settings.ExtendedSettings.CastWithValidate<PostPaidSettings>("product.Settings.ExtendedSettings", accountPartFinancial.ProductId);

                creditLimit = s_currencyExRateManager.ConvertValueToCurrency(postPaidSettings.CreditLimit, product.Settings.PricingCurrencyId, context.EntityBalanceInfo.CurrencyId, DateTime.Now);
            }



            return -(creditLimit * (100 - this.Percentage) / 100);
        }
    }
}
