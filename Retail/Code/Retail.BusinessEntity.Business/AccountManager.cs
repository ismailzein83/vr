using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountDetail> GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {
            Dictionary<int, Account> cachedAccounts = this.GetCachedAccounts();

            Func<Account, bool> filterExpression = (account) =>
                (input.Query.Name == null || account.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.AccountTypes == null || input.Query.AccountTypes.Contains(account.Type)) &&
                (
                    (!input.Query.ParentAccountId.HasValue && !account.ParentAccountId.HasValue) ||
                    (input.Query.ParentAccountId.HasValue && account.ParentAccountId.HasValue && account.ParentAccountId.Value == input.Query.ParentAccountId.Value)
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccounts.ToBigResult(input, filterExpression, AccountDetailMapper));
        }

        public Account GetAccount(int accountId)
        {
            Dictionary<int, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.GetRecord(accountId);
        }

        public string GetAccountName(int accountId)
        {
            Account account = this.GetAccount(accountId);
            return (account != null) ? account.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Account account)
        {
            ValidateAccountToAdd(account);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
            int accountId = -1;

            if (dataManager.Insert(account, out accountId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                account.AccountId = accountId;
                insertOperationOutput.InsertedObject = AccountDetailMapper(account);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit account)
        {
            int? parentId;
            ValidateAccountToEdit(account, out parentId);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();

            if (dataManager.Update(account, parentId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountDetailMapper(this.GetAccount(account.AccountId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Validation Methods

        private void ValidateAccountToAdd(Account account)
        {
            ValidateAccount(account.AccountId, account.Name, account.ParentAccountId);
        }

        private void ValidateAccountToEdit(AccountToEdit account, out int? parentAccountId)
        {
            Account accountEntity = this.GetAccount(account.AccountId);

            if (accountEntity == null)
                throw new DataIntegrityValidationException(String.Format("Account '{0}' does not exist", account.AccountId));

            parentAccountId = accountEntity.ParentAccountId;
            ValidateAccount(account.AccountId, account.Name, accountEntity.ParentAccountId);

            if (parentAccountId.HasValue)
            {
                IEnumerable<int> subAccountIds = this.GetSubAccountIds(parentAccountId.Value);
                if (subAccountIds == null || subAccountIds.Count() == 0)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not have any sub accounts", parentAccountId));
                if (!subAccountIds.Contains(account.AccountId))
                    throw new DataIntegrityValidationException(String.Format("Account '{0}' is not a sub account of Account '{1}'", account.AccountId, parentAccountId));
            }
        }

        private void ValidateAccount(int accountId, string name, int? parentAccountId)
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

        private IEnumerable<int> GetSubAccountIds(int parentAccountId)
        {
            Dictionary<int, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.MapRecords(itm => itm.Value.AccountId, itm => itm.Value.ParentAccountId.HasValue && itm.Value.ParentAccountId == parentAccountId);
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

        Dictionary<int, Account> GetCachedAccounts()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccounts", () =>
            {
                IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
                IEnumerable<Account> accounts = dataManager.GetAccounts();
                return accounts.ToDictionary(kvp => kvp.AccountId, kvp => kvp);
            });
        }

        Dictionary<int, List<Account>> GetCachedAccountsByParent()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountsByParent", () =>
            {
                IEnumerable<Account> accounts = GetCachedAccounts().Values;
                Dictionary<int, List<Account>> accountsByParent = new Dictionary<int, List<Account>>();
                foreach (var account in accounts)
                {
                    if (account.ParentAccountId != null)
                    {
                        List<Account> accountsofParent;
                        if (accountsByParent.TryGetValue((int)account.ParentAccountId, out accountsofParent))
                        {
                            accountsofParent.Add(account);
                        }
                        else
                        {
                            accountsofParent = new List<Account>() { account };
                            accountsByParent.Add((int)account.ParentAccountId, accountsofParent);

                        }
                    }

                }
                return accountsByParent;
            });
        }

        #endregion

        #region Mappers

        private AccountDetail AccountDetailMapper(Account account)
        {
            var accounts = GetCachedAccounts().Values;
            var accountsByParent = GetCachedAccountsByParent();
            return new AccountDetail()
            {
                Entity = account,
                DirectSubAccountCount = GetSubAccountsCount(account.AccountId, accounts, false),
                TotalSubAccountCount = GetSubAccountsCount(account.AccountId, accounts, true, accountsByParent)
            };
        }
        private int GetSubAccountsCount(int accountId, IEnumerable<Account> accounts, bool isTotalSubAccountsInclude, Dictionary<int, List<Account>> accountsByParent = null)
        {
            int count = 0;
            foreach (var account in accounts)
            {
                if (account.ParentAccountId == accountId)
                {
                    count++;
                    if (isTotalSubAccountsInclude)
                     count += GetTotalSubAccountsCountRecursively(account, accountsByParent);
                }
            }
            return count;
        }
        private int GetTotalSubAccountsCountRecursively(Account account, Dictionary<int, List<Account>> accountsByParent)
        {
            if(accountsByParent == null)
            {
                throw new NullReferenceException("accountsByParent");
            }
            List<Account> accountsForParents;
            if (accountsByParent.TryGetValue(account.AccountId, out accountsForParents))
            {
                foreach (var accountofParent in accountsForParents)
                {
                    return 1 + GetTotalSubAccountsCountRecursively(accountofParent, accountsByParent);
                }
            }
            return 0;
        }
        #endregion
    }
}
