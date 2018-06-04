using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateWhSBalancesQueueActivator : BaseUpdateAccountBalancesQueueActivator
    {
        //const string BillingCdrRecordTypeId = "6cf5f7ad-5123-45d2-b47f-eca613d454f7";
        //const string BillingStatsRecordTypeId = "7df45dea-6052-4f99-88e3-93f0562f2ffa";
        static WHSFinancialAccountManager s_financialAccountManager = new WHSFinancialAccountManager();

        public Guid CustomerUsageTransactionTypeId { get; set; }
        public Guid SupplierUsageTransactionTypeId { get; set; }
        public UpdateAccountBalanceSettings UpdateAccountBalanceSettings { get; set; }


        protected override void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context)
        {
            DataRecordType dataRecordType = new DataRecordTypeManager().GetDataRecordType(context.DataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", context.DataRecordTypeId);
            dataRecordType.Settings.ThrowIfNull("dataRecordType.Settings", context.DataRecordTypeId);

            dynamic cdrRecord = context.Record;
            DateTime attemptTime = cdrRecord.GetFieldValue(dataRecordType.Settings.DateTimeField);

            //switch (context.DataRecordTypeId.ToString().ToLower())
            //{
            //    case BillingCdrRecordTypeId: attemptTime = cdrRecord.AttemptDateTime; break;
            //    case BillingStatsRecordTypeId: attemptTime = cdrRecord.BatchStart; break;
            //    default: throw new NotSupportedException(string.Format("Data Record Type: {0} is not supported", context.DataRecordTypeId));
            //}

            int? saleFinancialAccountId = cdrRecord.SaleFinancialAccount;
            if (saleFinancialAccountId.HasValue)
            {
                Decimal? saleAmount = cdrRecord.SaleNet;
                if (saleAmount.HasValue && saleAmount.Value > 0)
                {
                    int? saleCurrencyId = cdrRecord.SaleCurrencyId;
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

            int? costFinancialAccountId = cdrRecord.CostFinancialAccount;
            if (costFinancialAccountId.HasValue)
            {
                Decimal? costAmount = cdrRecord.CostNet;
                if (costAmount.HasValue && costAmount.Value > 0)
                {
                    int? costCurrencyId = cdrRecord.CostCurrencyId;
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