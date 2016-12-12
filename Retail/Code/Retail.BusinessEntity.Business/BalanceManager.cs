using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class BalanceManager
    {
        public void CreateVolumeBalance(long accountId, string itemKey, Decimal balance)
        {
            throw new NotImplementedException();
        }

        public Decimal GetAccountRemainingBalance(long accountId, string itemKey)
        {
            AccountItemBalances accountItemBalances = GetAccountBalances(accountId);
            if(accountItemBalances != null)
            {
                AccountItemBalance itemBalance;
                if (accountItemBalances.ItemBalances.TryGetValue(itemKey, out itemBalance))
                    return itemBalance.RemainingBalance;
            }
            return 0;
        }

        public void DeductFromAccountBalance(long accountId, string itemKey, Decimal amountToDeduct)
        {
            AccountItemBalances accountItemBalances = GetAccountBalances(accountId);
            if (accountItemBalances != null)
            {
                AccountItemBalance itemBalance;
                if (accountItemBalances.ItemBalances.TryGetValue(itemKey, out itemBalance))
                {
                    itemBalance.RemainingBalance -= amountToDeduct;
                    InsertAmountToDeductToDB(accountId, itemKey, amountToDeduct);
                }
            }
        }

        private void InsertAmountToDeductToDB(long accountId, string itemKey, decimal amountToDeduct)
        {
            throw new NotImplementedException();
        }

        AccountItemBalances GetAccountBalances(long accountId)
        {
            throw new NotImplementedException();
        }
    }

    public class AccountItemBalances
    {
        public long AccountId { get; set; }

        public Dictionary<string, AccountItemBalance> ItemBalances { get; set; }
    }

    public class AccountItemBalance
    {
        public string ItemKey { get; set; }

        public Decimal RemainingBalance { get; set; }
    }
}
