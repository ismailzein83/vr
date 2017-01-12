using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountManager
    {
        #region Public Methods

        public Account GetAccount(long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.GetRecord(accountId);
        }

        public Account GetAccountBySourceId(string sourceId)
        {
            Dictionary<string, Account> cachedAccounts = this.GetCachedAccountsBySourceId();
            return cachedAccounts.GetRecord(sourceId);
        }

        public bool TryGetAccountPart(Account account, Guid partDefinitionId, bool getInherited, out AccountPart accountPart)
        {
            if (account.Settings != null && account.Settings.Parts != null && account.Settings.Parts.TryGetValue(partDefinitionId, out accountPart))
                return true;
            else if (getInherited && account.ParentAccountId.HasValue)
            {
                var parentAccount = GetAccount(account.ParentAccountId.Value);
                if (parentAccount == null)
                    throw new NullReferenceException(String.Format("parentAccount '{0}'", account.ParentAccountId.Value));
                return TryGetAccountPart(parentAccount, partDefinitionId, getInherited, out accountPart);
            }
            else
            {
                accountPart = null;
                return false;
            }
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountsUpdated(ref _updateHandle);
            }
        }

         #endregion

        #region Private Methods

        public Dictionary<long, Account> GetCachedAccounts()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccounts", () =>
            {
                IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
                IEnumerable<Account> accounts = dataManager.GetAccounts();
                return accounts.ToDictionary(kvp => kvp.AccountId, kvp => kvp);
            });
        }

        public Dictionary<string, Account> GetCachedAccountsBySourceId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountsBySourceId", () =>
            {
                return GetCachedAccounts().Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value);
            });
        }

        #endregion
    }
}
