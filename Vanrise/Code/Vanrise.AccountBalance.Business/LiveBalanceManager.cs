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
        public LiveBalance GetLiveBalance(Guid accountTypeId, String accountId)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.GetLiveBalance(accountTypeId, accountId);
        }

        public CurrentAccountBalance GetCurrentAccountBalance(Guid accountTypeId, String accountId)
        {
            var liveBalance = GetLiveBalance(accountTypeId,accountId);
            CurrencyManager currencyManager = new CurrencyManager();
            if (liveBalance == null)
                return null;
            return new CurrentAccountBalance
            {
                CurrencyDescription = currencyManager.GetCurrencyName(liveBalance.CurrencyId),
                CurrentBalance = liveBalance.CurrentBalance
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountBalanceDetail> GetFilteredAccountBalances(Vanrise.Entities.DataRetrievalInput<AccountBalanceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountBalanceRequestHandler());
        }

        private AccountBalanceDetail AccountBalanceDetailMapper(Vanrise.AccountBalance.Entities.AccountBalance accountBalance)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            AccountManager accountManager = new AccountManager();
            return new AccountBalanceDetail
            {
                Entity = accountBalance,
                CurrencyDescription = currencyManager.GetCurrencyName(accountBalance.CurrencyId),
                AccountInfo = accountManager.GetAccountInfo(accountBalance.AccountTypeId, accountBalance.AccountId)
            };
        }

        #region Private Classes
        private class AccountBalanceRequestHandler : BigDataRequestHandler<AccountBalanceQuery, Vanrise.AccountBalance.Entities.AccountBalance, AccountBalanceDetail>
        {
            public override AccountBalanceDetail EntityDetailMapper(Vanrise.AccountBalance.Entities.AccountBalance entity)
            {
                LiveBalanceManager manager = new LiveBalanceManager();
                return manager.AccountBalanceDetailMapper(entity);
            }

            public override IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountBalanceQuery> input)
            {
                ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
                return dataManager.GetFilteredAccountBalances(input.Query);
            }
        }

        #endregion
    }
}
