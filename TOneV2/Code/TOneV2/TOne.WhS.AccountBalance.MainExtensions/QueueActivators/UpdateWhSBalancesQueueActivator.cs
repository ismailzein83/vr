using System;
using System.Collections.Generic;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateWhSBalancesQueueActivator : BaseUpdateAccountBalancesQueueActivator
    {
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        protected override void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context)
        {
            dynamic mainCDR = context.Record;

            DateTime attemptTime = mainCDR.AttemptDateTime;

            int? customerId = mainCDR.CustomerId;
            if (customerId.HasValue)
            {
                Decimal? saleAmount = mainCDR.SaleNet;
                if (saleAmount.HasValue && saleAmount.Value > 0)
                {
                    CarrierFinancialAccountData customerFinancialAccountData = null;
                    if (s_financialAccountManager.TryGetCustAccFinancialAccountData(customerId.Value, attemptTime, out customerFinancialAccountData))
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = customerFinancialAccountData.AccountTypeId,
                            TransactionTypeId = customerFinancialAccountData.UsageTransactionTypeId,
                            AccountId = customerFinancialAccountData.FinancialAccountId.ToString(),
                            Amount = saleAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = mainCDR.SaleCurrencyId.Value
                        });
                    }
                }
            }

            int? supplierId = mainCDR.SupplierId;
            if (supplierId.HasValue)
            {
                Decimal? costAmount = mainCDR.CostNet;
                if (costAmount.HasValue && costAmount.Value > 0)
                {
                    CarrierFinancialAccountData supplierFinancialAccountData = null;
                    if (s_financialAccountManager.TryGetSuppAccFinancialAccountData(supplierId.Value, attemptTime, out supplierFinancialAccountData))
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = supplierFinancialAccountData.AccountTypeId,
                            TransactionTypeId = supplierFinancialAccountData.UsageTransactionTypeId,
                            AccountId = supplierFinancialAccountData.FinancialAccountId.ToString(),
                            Amount = costAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = mainCDR.CostCurrencyId.Value
                        });
                    }
                }
            }
        }

        protected override void FinalizeEmptyBatches(IFinalizeEmptyBatchesContext context)
        {
            AccountTypeFilter accountTypeFilter = new AccountTypeFilter()
            {
                Filters = new List<IAccountTypeExtendedSettingsFilter>()
                {
                    new AccountTypeExtendedSettingsFilter<AccountBalanceSettings>()
                }
            };

            var accountTypes = new AccountTypeManager().GetAccountTypes(accountTypeFilter);
            if (accountTypes != null)
            {
                foreach (var accountType in accountTypes)
                {
                    var accountBalanceSettings = accountType.Settings.ExtendedSettings as AccountBalanceSettings;
                    var usageTransactionTypeIds = accountBalanceSettings.GetUsageTransactionTypes(new GetUsageTransactionTypesContext());
                    if (usageTransactionTypeIds == null)
                        throw new NullReferenceException(string.Format("usageTransactionTypeIds of accountTypeId {0}", accountType.VRComponentTypeId));

                    foreach (var usageTransactionTypeId in usageTransactionTypeIds)
                    {
                        AccountBalanceType accountBalanceType = new AccountBalanceType() { AccountTypeId = accountType.VRComponentTypeId, TransactionTypeId = usageTransactionTypeId };
                        context.GenerateEmptyBatch(accountBalanceType);
                    }
                }
            }
        }
    }
}
