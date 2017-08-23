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
        public UpdateAccountBalanceSettings UpdateAccountBalanceSettings { get; set; }
        public Guid CustomerUsageTransactionTypeId { get; set; }
        public Guid SupplierUsageTransactionTypeId { get; set; }
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
                    int? saleCurrencyId = mainCDR.SaleCurrencyId;
                    WHSCarrierFinancialAccountData customerFinancialAccountData = null;
                    if (saleCurrencyId.HasValue && s_financialAccountManager.TryGetCustAccFinancialAccountData(customerId.Value, attemptTime, out customerFinancialAccountData) && customerFinancialAccountData.AccountBalanceData != null)
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = customerFinancialAccountData.AccountBalanceData.AccountTypeId,
                            TransactionTypeId = this.CustomerUsageTransactionTypeId,
                            AccountId = customerFinancialAccountData.FinancialAccount.FinancialAccountId.ToString(),
                            Amount = saleAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = saleCurrencyId.Value
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
                    int? costCurrencyId = mainCDR.CostCurrencyId;
                    WHSCarrierFinancialAccountData supplierFinancialAccountData = null;
                    if (costCurrencyId.HasValue && s_financialAccountManager.TryGetSuppAccFinancialAccountData(supplierId.Value, attemptTime, out supplierFinancialAccountData) && supplierFinancialAccountData.AccountBalanceData != null)
                    {
                        context.SubmitBalanceUpdate(new BalanceUpdatePayload
                        {
                            AccountTypeId = supplierFinancialAccountData.AccountBalanceData.AccountTypeId,
                            TransactionTypeId = this.SupplierUsageTransactionTypeId,
                            AccountId = supplierFinancialAccountData.FinancialAccount.FinancialAccountId.ToString(),
                            Amount = costAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = costCurrencyId.Value
                        });
                    }
                }
            }
        }

        protected override void FinalizeEmptyBatches(IFinalizeEmptyBatchesContext context)
        {
            IEnumerable<AccountBalanceType> unFinalizedAccountBalanceTypes;
            List<AccountBalanceType> finalizedAccountBalanceTypes = context.FinalizedAccountBalanceTypes;
            List<AccountBalanceType> allAccountBalanceTypeCombinations = GetAccountBalanceTypeCombinations(UpdateAccountBalanceSettings);

            if (finalizedAccountBalanceTypes == null || finalizedAccountBalanceTypes.Count == 0)
                unFinalizedAccountBalanceTypes = allAccountBalanceTypeCombinations;
            else
                unFinalizedAccountBalanceTypes = allAccountBalanceTypeCombinations.FindAllRecords(itm => !finalizedAccountBalanceTypes.Contains(itm));

            if (unFinalizedAccountBalanceTypes != null)
            {
                foreach (var unFinalizedAccountBalanceType in unFinalizedAccountBalanceTypes)
                {
                    context.GenerateEmptyBatch(unFinalizedAccountBalanceType);
                }
            }
        }
    }
}