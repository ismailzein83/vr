using System;
using Vanrise.AccountBalance.Business;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateAccountBalancesQueueActivator : BaseUpdateAccountBalancesQueueActivator
    {
        public Guid AccountTypeId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public string AccountId { get; set; }
        public string EffectiveOn { get; set; }
        public string Amount { get; set; }
        public string CurrencyId { get; set; }

        protected override void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context)
        {
            decimal? amount = Vanrise.Common.Utilities.GetPropValueReader(this.Amount).GetPropertyValue(context.Record);

            if (amount.HasValue && amount.Value > 0)
            {
                BalanceUpdatePayload balanceUpdateInfo = new BalanceUpdatePayload();
                balanceUpdateInfo.AccountTypeId = this.AccountTypeId;
                balanceUpdateInfo.TransactionTypeId = this.TransactionTypeId;

                balanceUpdateInfo.Amount = amount.Value;
                balanceUpdateInfo.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(context.Record).ToString();
                balanceUpdateInfo.EffectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOn).GetPropertyValue(context.Record);
                balanceUpdateInfo.CurrencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyId).GetPropertyValue(context.Record);

                context.SubmitBalanceUpdate(balanceUpdateInfo);
            }
        }

        protected override void FinalizeEmptyBatches(IFinalizeEmptyBatchesContext context)
        {
            AccountBalanceType accountBalanceType = new AccountBalanceType() { AccountTypeId = this.AccountTypeId, TransactionTypeId = this.TransactionTypeId };
            context.GenerateEmptyBatch(accountBalanceType);
        }
    }
}