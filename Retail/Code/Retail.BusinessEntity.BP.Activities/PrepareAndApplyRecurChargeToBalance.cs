using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Business;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class PrepareAndApplyRecurChargeToBalance : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> ChargeDay { get; set; }

        [RequiredArgument]
        public InArgument<List<AccountPackageRecurCharge>> AccountPackageRecurChargeList { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<AccountPackageRecurChargeKey>> ExistingAccountPackageRecurChargeKeys { get; set; }

        [RequiredArgument]
        public InArgument<List<RecurChargeBalanceUpdateSummary>> PreviousRecurChargeBalanceUpdateSummaryList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<AccountPackageRecurCharge> accountPackageRecurChargeList = this.AccountPackageRecurChargeList.Get(context);

            HashSet<AccountPackageRecurChargeKey> existingAccountPackageRecurChargeKeys = this.ExistingAccountPackageRecurChargeKeys.Get(context);
            List<RecurChargeBalanceUpdateSummary> previousRecurChargeBalanceUpdateSummaryList = this.PreviousRecurChargeBalanceUpdateSummaryList.Get(context);

            HashSet<AccountPackageRecurChargeKey> itemsToBeDeleted = new HashSet<AccountPackageRecurChargeKey>();
            if (existingAccountPackageRecurChargeKeys != null)
                itemsToBeDeleted.UnionWith(existingAccountPackageRecurChargeKeys);

            if (previousRecurChargeBalanceUpdateSummaryList != null)
                itemsToBeDeleted.UnionWith(previousRecurChargeBalanceUpdateSummaryList.SelectMany(itm => itm.AccountPackageRecurChargeKeys));

            UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
            

            if (accountPackageRecurChargeList != null && accountPackageRecurChargeList.Count > 0)
            {
                Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>> accountPackageRecurChargeDict = new Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>();
                foreach (AccountPackageRecurCharge accountPackageRecurCharge in accountPackageRecurChargeList)
                {
                    AccountPackageRecurChargeKey accountPackageRecurChargeKey = BuildAccountPackageRecurChargeKey(accountPackageRecurCharge);
                    List<AccountPackageRecurCharge> tempAccountPackageRecurChargeList = accountPackageRecurChargeDict.GetOrCreateItem(accountPackageRecurChargeKey);
                    itemsToBeDeleted.Remove(accountPackageRecurChargeKey);
                    tempAccountPackageRecurChargeList.Add(accountPackageRecurCharge);
                }

                CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
                int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

                foreach (var item in accountPackageRecurChargeDict)
                {
                    AccountPackageRecurChargeKey accountPackageRecurChargeItemKey = item.Key;
                    List<AccountPackageRecurCharge> accountPackageRecurChargeItemList = item.Value;

                    if (!accountPackageRecurChargeItemKey.BalanceAccountTypeID.HasValue)
                        continue;

                    Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();
                    CorrectUsageBalancePayload payload = new CorrectUsageBalancePayload()
                    {
                        CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                        PeriodDate = accountPackageRecurChargeItemKey.ChargeDay,
                        CorrectionProcessId = correctionProcessId,
                        TransactionTypeId = accountPackageRecurChargeItemKey.TransactionTypeId,
                        IsLastBatch = true
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
            if (itemsToBeDeleted.Count > 0)
            {
                foreach (AccountPackageRecurChargeKey accountPackageRecurChargeKey in itemsToBeDeleted)
                {
                    if (accountPackageRecurChargeKey.BalanceAccountTypeID.HasValue)
                    {
                        Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();
                        CorrectUsageBalancePayload payload = new CorrectUsageBalancePayload()
                        {
                            CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                            PeriodDate = accountPackageRecurChargeKey.ChargeDay,
                            CorrectionProcessId = correctionProcessId,
                            TransactionTypeId = accountPackageRecurChargeKey.TransactionTypeId,
                            IsLastBatch = true
                        };
                        usageBalanceManager.CorrectUsageBalance(accountPackageRecurChargeKey.BalanceAccountTypeID.Value, payload);
                    }
                }
            }
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