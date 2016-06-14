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
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountDetail> GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();

            Func<Account, bool> filterExpression = (account) =>
                (input.Query.Name == null || account.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.AccountTypes == null || input.Query.AccountTypes.Contains(account.Type)) &&
                (
                    (!input.Query.ParentAccountId.HasValue && !account.ParentAccountId.HasValue) ||
                    (input.Query.ParentAccountId.HasValue && account.ParentAccountId.HasValue && account.ParentAccountId.Value == input.Query.ParentAccountId.Value)
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccounts.ToBigResult(input, filterExpression, AccountDetailMapper));
        }

        public Account GetAccount(long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.GetRecord(accountId);
        }

        public IEnumerable<AccountInfo> GetAccountsInfo(string nameFilter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<Account> accounts = GetCachedAccounts().Values;

            Func<Account, bool> accountFilter = (account) =>
             {
                 if (nameFilterLower != null && !account.Name.ToLower().Contains(nameFilterLower))
                     return false;
                 return true;
             };
            return accounts.MapRecords(AccountInfoMapper, accountFilter).OrderBy(x => x.Name);
        }


        public IEnumerable<AccountInfo> GetAccountsInfoByIds(HashSet<long> accountIds)
        {
            IEnumerable<Account> accounts = GetCachedAccounts().Values;
            Func<Account, bool> accountFilter = (account) =>
            {
                if (!accountIds.Contains(account.AccountId))
                    return false;
                return true;
            };
            return accounts.MapRecords(AccountInfoMapper, accountFilter).OrderBy(x => x.Name);
        }


        public string GetAccountName(long accountId)
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
            long accountId = -1;

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
            long? parentId;
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

        Dictionary<long, Account> GetCachedAccounts()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccounts", () =>
            {
                IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();
                IEnumerable<Account> accounts = dataManager.GetAccounts();
                return accounts.ToDictionary(kvp => kvp.AccountId, kvp => kvp);
            });
        }

        Dictionary<long, List<Account>> GetCachedAccountsByParent()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountsByParent", () =>
            {
                IEnumerable<Account> accounts = GetCachedAccounts().Values;
                Dictionary<long, List<Account>> accountsByParent = new Dictionary<long, List<Account>>();
                foreach (var account in accounts)
                {
                    if (account.ParentAccountId != null)
                    {
                        List<Account> accountsofParent;
                        if (accountsByParent.TryGetValue(account.ParentAccountId.Value, out accountsofParent))
                        {
                            accountsofParent.Add(account);
                        }
                        else
                        {
                            accountsofParent = new List<Account>() { account };
                            accountsByParent.Add(account.ParentAccountId.Value, accountsofParent);

                        }
                    }

                }
                return accountsByParent;
            });
        }

        IEnumerable<GenericRuleDefinition> GetAccountsMappingRuleDefinitions()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            var subscriberAccountBEDefinitionId = beDefinitionManager.GetBusinessEntityDefinitionId(Account.BUSINESSENTITY_DEFINITION_NAME);
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            var allMappingRuleDefinitions = ruleDefinitionManager.GetGenericRuleDefinitionsByType(MappingRule.RULE_DEFINITION_TYPE_NAME);
            return allMappingRuleDefinitions.FindAllRecords(itm =>
            {
                var mappingRuleDefinitionSettings = itm.SettingsDefinition as MappingRuleDefinitionSettings;
                if (mappingRuleDefinitionSettings != null)
                {
                    var businessEntityFieldType = mappingRuleDefinitionSettings.FieldType as Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType;
                    if (businessEntityFieldType != null)
                        return businessEntityFieldType.BusinessEntityDefinitionId == subscriberAccountBEDefinitionId;
                }
                return false;
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

        private AccountInfo AccountInfoMapper(Account account)
        {
            return new AccountInfo
            {
                AccountId = account.AccountId,
                Name = account.Name
            };
        }
        private int GetSubAccountsCount(long accountId, IEnumerable<Account> accounts, bool isTotalSubAccountsInclude, Dictionary<long, List<Account>> accountsByParent = null)
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
        private int GetTotalSubAccountsCountRecursively(Account account, Dictionary<long, List<Account>> accountsByParent)
        {
            if (accountsByParent == null)
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
            var accountNames = new List<string>();
            foreach (var entityId in context.EntityIds)
            {
                string accountName = GetAccountName((long)entityId);
                if (accountName == null) throw new NullReferenceException("accountName");
                accountNames.Add(accountName);
            }
            return String.Join(",", accountNames);
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public bool IsMatched(IBusinessEntityMatchContext context)
        {
            if (context.FieldValueIds == null || context.FilterIds == null) return true;

            var fieldValueIds = context.FieldValueIds.MapRecords(itm =>(long)(itm));
            var filterIds = context.FilterIds.MapRecords(itm => (long)(itm));
            foreach (var filterId in filterIds)
            {
                if (fieldValueIds.Contains(filterId))
                    return true;
            }
            return false;
        }

        #endregion
    }
}
