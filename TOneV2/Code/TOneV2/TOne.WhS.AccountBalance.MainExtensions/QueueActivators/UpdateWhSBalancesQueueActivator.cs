using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateWhSBalancesQueueActivator : BaseUpdateAccountBalancesQueueActivator
    {
        static WHSFinancialAccountManager s_financialAccountManager = new WHSFinancialAccountManager();
        public Guid CustomerUsageTransactionTypeId { get; set; }
        public Guid SupplierUsageTransactionTypeId { get; set; }
        public UpdateAccountBalanceSettings UpdateAccountBalanceSettings { get; set; }

        protected override void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context)
        {
            dynamic mainCDR = context.Record;

            DateTime attemptTime = mainCDR.AttemptDateTime;

            int? saleFinancialAccountId = mainCDR.SaleFinancialAccount;
            if (saleFinancialAccountId.HasValue)
            {
                Decimal? saleAmount = mainCDR.SaleNet;
                if (saleAmount.HasValue && saleAmount.Value > 0)
                {
                    int? saleCurrencyId = mainCDR.SaleCurrencyId;
                    WHSCarrierFinancialAccountData saleFinancialAccountData = s_financialAccountManager.GetCustCarrierFinancialByFinAccId(saleFinancialAccountId.Value);

                    if (saleCurrencyId.HasValue && saleFinancialAccountData.AccountBalanceData != null)
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = saleFinancialAccountData.AccountBalanceData.AccountTypeId,
                            TransactionTypeId = this.CustomerUsageTransactionTypeId,
                            AccountId = saleFinancialAccountId.Value.ToString(),
                            Amount = saleAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = saleCurrencyId.Value
                        });
                    }
                }
            }

            int? costFinancialAccountId = mainCDR.CostFinancialAccount;
            if (costFinancialAccountId.HasValue)
            {
                Decimal? costAmount = mainCDR.CostNet;
                if (costAmount.HasValue && costAmount.Value > 0)
                {
                    int? costCurrencyId = mainCDR.CostCurrencyId;
                    WHSCarrierFinancialAccountData costFinancialAccountData = s_financialAccountManager.GetSuppCarrierFinancialByFinAccId(costFinancialAccountId.Value);

                    if (costCurrencyId.HasValue && costFinancialAccountData.AccountBalanceData != null)
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = costFinancialAccountData.AccountBalanceData.AccountTypeId,
                            TransactionTypeId = this.SupplierUsageTransactionTypeId,
                            AccountId = costFinancialAccountId.Value.ToString(),
                            Amount = costAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = costCurrencyId.Value
                        });
                    }
                }
            }
        }

        protected override List<AccountBalanceType> GetAccountBalanceTypeCombinations(IGetAccountBalanceTypeCombinationsContext context)
        {
            return base.GetAccountBalanceTypeCombinations(UpdateAccountBalanceSettings);
        }
    }
}