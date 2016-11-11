using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class LiveBalanceManager
    {
        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.GetLiveBalanceAccountsInfo(accountTypeId);
        }
        public LiveBalance GetLiveBalance(long accountId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.GetLiveBalance(accountId);
        }

        public CurrentAccountBalance GetCurrentAccountBalance(long accountId)
        {
            var liveBalance = GetLiveBalance(accountId);
            CurrencyManager currencyManager = new CurrencyManager();
            if (liveBalance == null)
                return null;
            return new CurrentAccountBalance
            {
                CurrencyDescription = currencyManager.GetCurrencyName(liveBalance.CurrencyId),
                CurrentBalance = liveBalance.CurrentBalance
            };
        }
    }
}
