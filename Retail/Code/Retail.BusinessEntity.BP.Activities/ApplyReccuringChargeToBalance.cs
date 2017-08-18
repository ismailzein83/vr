using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
using Vanrise.Entities;
using System.Linq;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class ApplyReccuringChargeToBalance : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Apply Reccuring Charge To Balance has started", null);

            DateTime effectiveDate = context.GetValue(this.EffectiveDate);
            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargeManager.GetAccountPackageRecurChargesNotSent(effectiveDate);

            if (accountPackageRecurChargeList != null && accountPackageRecurChargeList.Count > 0)
            {
                Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>> accountPackageRecurChargeDict = new Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>();
                foreach (AccountPackageRecurCharge accountPackageRecurCharge in accountPackageRecurChargeList)
                {
                    List<AccountPackageRecurCharge> tempAccountPackageRecurChargeList = accountPackageRecurChargeDict.GetOrCreateItem(BuildAccountPackageRecurChargeKey(accountPackageRecurCharge));
                    tempAccountPackageRecurChargeList.Add(accountPackageRecurCharge);
                }

                CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
                int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
                Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

                foreach (var item in accountPackageRecurChargeDict)
                {
                    AccountPackageRecurChargeKey accountPackageRecurChargeItemKey = item.Key;
                    List<AccountPackageRecurCharge> accountPackageRecurChargeItemList = item.Value;

                    if (!accountPackageRecurChargeItemKey.BalanceAccountTypeID.HasValue)
                        continue;

                    CorrectUsageBalancePayload payload = new CorrectUsageBalancePayload()
                    {
                        CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                        PeriodDate = accountPackageRecurChargeItemKey.ChargeDay,
                        CorrectionProcessId = correctionProcessId,
                        TransactionTypeId = accountPackageRecurChargeItemKey.TransactionTypeId
                    };

                    Dictionary<string, AccountAmount> accountBalanceAmount = new Dictionary<string, AccountAmount>();

                    foreach (AccountPackageRecurCharge accountPackageRecurChargeItem in accountPackageRecurChargeItemList)
                    {
                        if (!string.IsNullOrEmpty(accountPackageRecurChargeItem.BalanceAccountID))
                        {
                            decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(accountPackageRecurChargeItem.ChargeAmount, accountPackageRecurChargeItem.CurrencyID, systemCurrencyId, accountPackageRecurChargeItemKey.ChargeDay);
                            AccountAmount accountAmount = accountBalanceAmount.GetOrCreateItem(accountPackageRecurChargeItem.BalanceAccountID, () => { return new AccountAmount() { Amount = 0 }; });
                            accountAmount.Amount += convertedAmount;
                        }
                    }

                    if (accountBalanceAmount.Count > 0)
                    {
                        payload.CorrectUsageBalanceItems.AddRange(accountBalanceAmount.Select(itm => new CorrectUsageBalanceItem() { AccountId = itm.Key, CurrencyId = systemCurrencyId, Value = itm.Value.Amount }));
                        usageBalanceManager.CorrectUsageBalance(accountPackageRecurChargeItemKey.BalanceAccountTypeID.Value, payload);
                    }
                }
            }

            accountPackageRecurChargeManager.UpdateAccountPackageRecurChargeToSent(effectiveDate);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Apply Reccuring Charge To Balance is done", null);
        }

        private AccountPackageRecurChargeKey BuildAccountPackageRecurChargeKey(AccountPackageRecurCharge accountPackageRecurCharge)
        {
            return new AccountPackageRecurChargeKey()
            {
                BalanceAccountTypeID = accountPackageRecurCharge.BalanceAccountTypeID,
                ChargeDay = accountPackageRecurCharge.ChargeDay,
                TransactionTypeId = accountPackageRecurCharge.TransactionTypeID
            };
        }

        private class AccountAmount
        {
            public decimal Amount { get; set; }
        }
    }
}