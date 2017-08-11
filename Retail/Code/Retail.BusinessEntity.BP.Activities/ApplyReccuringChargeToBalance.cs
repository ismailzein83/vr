using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class ApplyReccuringChargeToBalance : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //DateTime effectiveDate = context.GetValue(this.EffectiveDate);
            //AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            //List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargeManager.GetAccountPackageRecurChargesNotSent(effectiveDate);

            //if (accountPackageRecurChargeList != null && accountPackageRecurChargeList.Count > 0)
            //{
            //    Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>> accountPackageRecurChargeDict = new Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>();
            //    foreach (AccountPackageRecurCharge accountPackageRecurCharge in accountPackageRecurChargeList)
            //    {
            //        List<AccountPackageRecurCharge> tempAccountPackageRecurChargeList = accountPackageRecurChargeDict.GetOrCreateItem(BuildAccountPackageRecurChargeKey(accountPackageRecurCharge));
            //        tempAccountPackageRecurChargeList.Add(accountPackageRecurCharge);
            //    }

            //    CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            //    int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

            //    UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
            //    Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

            //    foreach (var item in accountPackageRecurChargeDict)
            //    {
            //        AccountPackageRecurChargeKey accountPackageRecurChargeItemKey = item.Key;
            //        List<AccountPackageRecurCharge> accountPackageRecurChargeItemList = item.Value;
            //        if (accountPackageRecurChargeItemList == null || accountPackageRecurChargeItemList.Count == 0)
            //            continue;

            //        CorrectUsageBalancePayload payload = new CorrectUsageBalancePayload()
            //        {
            //            CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
            //            PeriodDate = accountPackageRecurChargeItemKey.ChargeDay,
            //            CorrectionProcessId = correctionProcessId,
            //            TransactionTypeId = accountPackageRecurChargeItemKey.TransactionTypeId
            //        };

            //        decimal convertedAmount = 0;
            //        foreach (AccountPackageRecurCharge accountPackageRecurChargeItem in accountPackageRecurChargeItemList)
            //        {
            //            convertedAmount += currencyExchangeRateManager.ConvertValueToCurrency(accountPackageRecurChargeItem.ChargeAmount, accountPackageRecurChargeItem.CurrencyID, systemCurrencyId, accountPackageRecurChargeItemKey.ChargeDay);

            //        }
            //        CorrectUsageBalanceItem correctUsageBalanceItem = new CorrectUsageBalanceItem() { AccountId = accountPackageRecurChargeItemKey.BalanceAccountID, CurrencyId = systemCurrencyId, Value = convertedAmount };
            //        payload.CorrectUsageBalanceItems.Add(correctUsageBalanceItem);
            //    }
            //}
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
    }
}