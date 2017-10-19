using System;
using System.Collections.Generic;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Vanrise.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateAccountBalancesQueueActivator : BaseUpdateAccountBalancesQueueActivator
    {
        public Guid? BalanceAccountTypeId { get; set; }
        public Guid? TransactionTypeId { get; set; }
        public string AccountIdFieldName { get; set; }
        public string EffectiveOnFieldName { get; set; }
        public string AmountFieldName { get; set; }
        public string CurrencyIdFieldName { get; set; }
        public string BalanceAccountTypeIdFieldName { get; set; }
        public string TransactionTypeIdFieldName { get; set; }
        public UpdateAccountBalanceSettings UpdateAccountBalanceSettings { get; set; }

        protected override void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context)
        {
            decimal? amount = Vanrise.Common.Utilities.GetPropValueReader(this.AmountFieldName).GetPropertyValue(context.Record);

            if (amount.HasValue && amount.Value > 0)
            {
                BalanceUpdatePayload balanceUpdateInfo = new BalanceUpdatePayload();

                if (!String.IsNullOrEmpty(this.BalanceAccountTypeIdFieldName))
                {
                    balanceUpdateInfo.AccountTypeId = Vanrise.Common.Utilities.GetPropValueReader(this.BalanceAccountTypeIdFieldName).GetPropertyValue(context.Record);
                }
                else
                {
                    if (!this.BalanceAccountTypeId.HasValue)
                        throw new NullReferenceException("UpdateAccountBalancesQueueActivator.BalanceAccountTypeId");

                    balanceUpdateInfo.AccountTypeId = this.BalanceAccountTypeId.Value;
                }

                if (!String.IsNullOrEmpty(this.TransactionTypeIdFieldName))
                {
                    balanceUpdateInfo.TransactionTypeId = Vanrise.Common.Utilities.GetPropValueReader(this.TransactionTypeIdFieldName).GetPropertyValue(context.Record);
                }
                else
                {
                    if (!this.TransactionTypeId.HasValue)
                        throw new NullReferenceException("UpdateAccountBalancesQueueActivator.TransactionTypeId");

                    balanceUpdateInfo.TransactionTypeId = this.TransactionTypeId.Value;
                }

                balanceUpdateInfo.Amount = amount.Value;
                balanceUpdateInfo.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountIdFieldName).GetPropertyValue(context.Record).ToString();
                balanceUpdateInfo.EffectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOnFieldName).GetPropertyValue(context.Record);
                balanceUpdateInfo.CurrencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyIdFieldName).GetPropertyValue(context.Record);

                context.SubmitBalanceUpdate(balanceUpdateInfo);
            }
        }

        protected override List<AccountBalanceType> GetAccountBalanceTypeCombinations(IGetAccountBalanceTypeCombinationsContext context)
        {
            return base.GetAccountBalanceTypeCombinations(this.UpdateAccountBalanceSettings);
        }
    }
}