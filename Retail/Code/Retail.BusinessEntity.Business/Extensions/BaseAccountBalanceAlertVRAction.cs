using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public abstract class BaseAccountBalanceAlertVRAction : VRAction
    {
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        protected Guid GetAccountBEDefinitionId(VRBalanceAlertEventPayload balanceAlertEventPayload)
        {
            return FinancialAccountBalanceManager.GetAccountBEDefinitionIdByAlertRuleTypeId(balanceAlertEventPayload.AlertRuleTypeId);
        }

        protected long GetAccountId(VRBalanceAlertEventPayload balanceAlertEventPayload, out Guid accountBEDefinitionId)
        {
            accountBEDefinitionId = GetAccountBEDefinitionId(balanceAlertEventPayload);
            var financialAccountData = s_financialAccountManager.GetFinancialAccountData(accountBEDefinitionId, balanceAlertEventPayload.EntityId);
            if (financialAccountData != null)
                return financialAccountData.Account.AccountId;
            else
                return ParseAccountId(balanceAlertEventPayload.EntityId);
        }

        private long ParseAccountId(string balanceAccountId)
        {
            long accountId;
            if (!long.TryParse(balanceAccountId, out accountId))
                throw new Exception(String.Format("balanceAccountId. Cannot parse '{0}' to long", balanceAccountId));
            return accountId;
        }

        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        protected Account GetAccount(VRBalanceAlertEventPayload balanceAlertEventPayload, out Guid accountBEDefinitionId)
        {
            accountBEDefinitionId = GetAccountBEDefinitionId(balanceAlertEventPayload);
            var financialAccountData = s_financialAccountManager.GetFinancialAccountData(accountBEDefinitionId, balanceAlertEventPayload.EntityId);
            if (financialAccountData != null)
            {
                return financialAccountData.Account;
            }
            else
            {
                long accountId = ParseAccountId(balanceAlertEventPayload.EntityId);
                return s_accountBEManager.GetAccount(accountBEDefinitionId, accountId);
            }
        }

        protected Decimal? GetAccountCreditLimit(VRBalanceAlertEventPayload balanceAlertEventPayload, out Guid accountBEDefinitionId)
        {
            accountBEDefinitionId = GetAccountBEDefinitionId(balanceAlertEventPayload);
            var financialAccountData = s_financialAccountManager.GetFinancialAccountData(accountBEDefinitionId, balanceAlertEventPayload.EntityId);
            if (financialAccountData != null)
            {
                return financialAccountData.CreditLimit;
            }
            else
            {
                long accountId = ParseAccountId(balanceAlertEventPayload.EntityId);
                IAccountPayment accountPayment;
                if (s_accountBEManager.HasAccountPayment(accountBEDefinitionId, accountId, true, out accountPayment))
                {
                    var product = new ProductManager().GetProduct(accountPayment.ProductId);
                    product.ThrowIfNull("product", accountPayment.ProductId);
                    if(product.Settings != null)
                    {
                        IPostpaidProductSettings productSettingsAsPostpaid = product.Settings.ExtendedSettings as IPostpaidProductSettings;
                        if (productSettingsAsPostpaid != null)
                            return productSettingsAsPostpaid.CreditLimit;
                    }
                }
                return null;
            }
        }
    }
}
