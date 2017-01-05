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
    public class AccountManager : IBusinessEntityManager
    {
        #region Public Methods

        #region Sync
        internal bool TryUpdateAccount(AccountToEdit account)
        {
            long? parentId;
            ValidateAccountToEdit(account, out parentId);

            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();

            return dataManager.Update(account, parentId);
        }
        internal bool TryAddAccount(Account account, out long accountId)
        {
            ValidateAccountToAdd(account);

            if (account.StatusId == Guid.Empty)
            {
                var accountTypeManager = new AccountTypeManager();
                var accountType = accountTypeManager.GetAccountType(account.TypeId);
                if (accountType == null)
                    throw new NullReferenceException("AccountType is null");
                if (accountType.Settings == null)
                    throw new NullReferenceException("AccountType settings is null");

                account.StatusId = accountType.Settings.InitialStatusId;
            }

            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();

            return dataManager.Insert(account, out accountId);
        }
        private void ValidateAccountToAdd(Account account)
        {
            ValidateAccount(account.AccountId, account.Name, account.ParentAccountId);
        }
        private void ValidateAccountToEdit(AccountToEdit account, out long? parentAccountId)
        {
            Account accountEntity = this.GetAccount(account.AccountId);

            if (accountEntity == null)
                throw new DataIntegrityValidationException(String.Format("Account '{0}' does not exist", account.AccountId));

            parentAccountId = accountEntity.ParentAccountId;
            ValidateAccount(account.AccountId, account.Name, accountEntity.ParentAccountId);

            if (parentAccountId.HasValue)
            {
                IEnumerable<long> subAccountIds = this.GetSubAccountIds(parentAccountId.Value);
                if (subAccountIds == null || subAccountIds.Count() == 0)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not have any sub accounts", parentAccountId));
                if (!subAccountIds.Contains(account.AccountId))
                    throw new DataIntegrityValidationException(String.Format("Account '{0}' is not a sub account of Account '{1}'", account.AccountId, parentAccountId));
            }
        }
        private void ValidateAccount(long accountId, string name, long? parentAccountId)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("Account.Name");

            if (parentAccountId.HasValue)
            {
                Account parentAccount = this.GetAccount(parentAccountId.Value);
                if (parentAccount == null)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not exist", parentAccountId));
            }
        }
        private IEnumerable<long> GetSubAccountIds(long parentAccountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.MapRecords(itm => itm.Value.AccountId, itm => itm.Value.ParentAccountId.HasValue && itm.Value.ParentAccountId == parentAccountId);
        }
        #endregion

        public Account GetAccount(long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.GetRecord(accountId);
        } 
        public string GetAccountName(long accountId)
        {
            Account account = this.GetAccount(accountId);
            return (account != null) ? account.Name : null;
        }
        public Account GetAccountBySourceId(string sourceId)
        {
            Dictionary<string, Account> cachedAccounts = this.GetCachedAccountsBySourceId();
            return cachedAccounts.GetRecord(sourceId);
        }
        public bool UpdateStatus(long accountId, Guid statusId)
        {
            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
            bool updateStatus = dataManager.UpdateStatus(accountId, statusId);
            if (updateStatus)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateStatus;
        }
        public bool UpdateExecutedActions(long accountId, ExecutedActions executedActions)
        {
            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
            bool updateExecutedAction = dataManager.UpdateExecutedActions(accountId, executedActions);
            if (updateExecutedAction)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateExecutedAction;
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

        private class AccountTreeNode
        {
            public Account Account { get; set; }

            public AccountTreeNode ParentNode { get; set; }

            List<AccountTreeNode> _childNodes = new List<AccountTreeNode>();
            public List<AccountTreeNode> ChildNodes
            {
                get
                {
                    return _childNodes;
                }
            }

            public int TotalSubAccountsCount { get; set; }
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

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedAccounts = GetCachedAccounts();
            if (cachedAccounts != null)
                return cachedAccounts.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetAccount(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetAccountName(Convert.ToInt64(context.EntityId));
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            switch (context.InfoType)
            {
                case Vanrise.AccountBalance.Entities.AccountInfo.BEInfoType:
                    {
                        var account = context.Entity as Account;
                        StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();
                        var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId);
                        Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
                        {
                            Name = account.Name,
                            StatusDescription = statusDesciption,
                        };
                        var currency = GetCurrencyId(account.Settings.Parts.Values);
                        if (currency.HasValue)
                        {
                            accountInfo.CurrencyId = currency.Value;
                        }
                        else
                        {
                            throw new Exception(string.Format("Account {0} does not have currency", accountInfo.Name));
                        }
                        return accountInfo;
                    }
                default: return null;
            }
        }

        private int? GetCurrencyId(IEnumerable<AccountPart> parts)
        {
            foreach (AccountPart part in parts)
            {
                var actionpartSetting = part.Settings as IAccountPayment;
                if (actionpartSetting != null)
                {
                    return actionpartSetting.CurrencyId;
                }
            }
            return null;
        }

        #endregion
    }
}
