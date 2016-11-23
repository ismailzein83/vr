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

        public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();

            Func<Account, bool> filterExpression = (account) =>
                (input.Query.Name == null || account.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.AccountTypeIds == null || input.Query.AccountTypeIds.Contains(account.TypeId)) &&
                (
                    (!input.Query.ParentAccountId.HasValue && !account.ParentAccountId.HasValue) ||
                    (input.Query.ParentAccountId.HasValue && account.ParentAccountId.HasValue && account.ParentAccountId.Value == input.Query.ParentAccountId.Value)
                );

            return DataRetrievalManager.Instance.ProcessResult(input, cachedAccounts.ToBigResult(input, filterExpression, AccountDetailMapper));
        }

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
        public AccountDetail GetAccountDetail(long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.MapRecord(AccountDetailMapper, x => x.AccountId == accountId);
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
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long accountId;

            if (TryAddAccount(account, out accountId))
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
            bool updateExecutedAction = dataManager.UpdateExecutedActions(accountId,executedActions);
            if (updateExecutedAction)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateExecutedAction;
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit account)
        {

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;


            if (TryUpdateAccount(account))
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

        internal bool TryUpdateAccount(AccountToEdit account)
        {
            long? parentId;
            ValidateAccountToEdit(account, out parentId);

            IAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountDataManager>();

            return dataManager.Update(account, parentId);
        }

        public bool HasAccountPayment(long accountId, bool getInherited, out IAccountPayment accountPayment)
        {
            var account = GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));

            if (account.Settings == null)
                throw new NullReferenceException(String.Format("account.Settings '{0}'", accountId));

            if (account.Settings.Parts != null)
            {
                foreach (var part in account.Settings.Parts)
                {
                    accountPayment = part.Value as IAccountPayment;
                    if (accountPayment != null)
                        return true;
                }
            }
            if (getInherited && account.ParentAccountId.HasValue)
                return HasAccountPayment(account.ParentAccountId.Value, true, out accountPayment);
            else
            {
                accountPayment = null;
                return false;
            }
        }

        public bool IsAccountMatchWithFilterGroup(Account account, RecordFilterGroup filterGroup)
        {
            return new Vanrise.GenericData.Business.RecordFilterManager().IsFilterGroupMatch(filterGroup, new AccountRecordFilterGenericFieldMatchContext(account));
        }

        #region Get Account Editor Runtime

        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountTypeId, int? parentAccountId)
        {
            var accountEditorRuntime = new AccountEditorRuntime();

            var accountPartDefinitionManager = new AccountPartDefinitionManager();

            IEnumerable<AccountTypePartSettings> partSettingsList = GetAccountTypePartDefinitionSettingsList(accountTypeId);

            var runtimeParts = new List<AccountPartRuntime>();

            foreach (AccountTypePartSettings partSettings in partSettingsList)
            {
                bool isPartFoundOrInherited = this.IsPartFoundOrInherited(parentAccountId, partSettings.PartDefinitionId);

                if (partSettings.AvailabilitySettings == AccountPartAvailabilityOptions.AlwaysAvailable || !isPartFoundOrInherited)
                {
                    var accountPartRuntime = new AccountPartRuntime();

                    AccountPartDefinition partDefinition = accountPartDefinitionManager.GetAccountPartDefinition(partSettings.PartDefinitionId);
                    if (partDefinition == null)
                        throw new NullReferenceException("partDefinition");

                    accountPartRuntime.PartDefinition = partDefinition;

                    if (partSettings.RequiredSettings == AccountPartRequiredOptions.RequiredIfNotInherited)
                        accountPartRuntime.IsRequired = !isPartFoundOrInherited;
                    else
                        accountPartRuntime.IsRequired = (partSettings.RequiredSettings == AccountPartRequiredOptions.Required);

                    runtimeParts.Add(accountPartRuntime);
                }
            }

            if (runtimeParts.Count > 0)
                accountEditorRuntime.Parts = runtimeParts;
            AccountTypeManager manager = new AccountTypeManager();


            return accountEditorRuntime;
        }

        private IEnumerable<AccountTypePartSettings> GetAccountTypePartDefinitionSettingsList(Guid accountTypeId)
        {
            var accountTypeManager = new AccountTypeManager();
            AccountType accountType = accountTypeManager.GetAccountType(accountTypeId);
            if (accountType == null)
                throw new NullReferenceException("accountType");
            if (accountType.Settings == null)
                throw new NullReferenceException("accountType.Settings");
            if (accountType.Settings.PartDefinitionSettings == null)
                throw new NullReferenceException("accountType.Settings.PartDefinitionSettings");
            return accountType.Settings.PartDefinitionSettings;
        }

        private bool IsPartFoundOrInherited(long? accountId, Guid partDefinitionId)
        {
            if (!accountId.HasValue)
                return false;

            Account account = this.GetAccount(accountId.Value);
            if (account == null)
                throw new NullReferenceException("account");
            if (account.Settings == null)
                throw new NullReferenceException("account.Settings");

            if (account.Settings.Parts == null)
                return false;

            AccountPart accountPart;
            bool isPartFound = account.Settings.Parts.TryGetValue(partDefinitionId, out accountPart);

            if (isPartFound)
                return true;

            return IsPartFoundOrInherited(account.ParentAccountId, partDefinitionId);
        }

        #endregion

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

        private class AccountRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            Account _account;
            AccountTypeManager _accountTypeManager = new AccountTypeManager();
            public AccountRecordFilterGenericFieldMatchContext(Account account)
            {
                _account = account;
            }

            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                Guid? partDefinitionId;
                string partFieldName;
                _accountTypeManager.ParseAccountGenericFieldName(fieldName, out partDefinitionId, out partFieldName);
                fieldType = _accountTypeManager.GetAccountGenericFieldType(fieldName);
                if (partDefinitionId.HasValue)
                    return GetAccountPartFieldValue(partDefinitionId.Value, partFieldName);
                else
                    return GetAccountStaticFieldValue(partFieldName);
            }

            dynamic GetAccountStaticFieldValue(string fieldName)
            {
                throw new NotImplementedException();
            }

            dynamic GetAccountPartFieldValue(Guid partDefinitionId, string fieldName)
            {
                AccountPart accountPart;
                if (_account.Settings.Parts != null && _account.Settings.Parts.TryGetValue(partDefinitionId, out accountPart))
                    return accountPart.Settings.GetFieldValue(new AccountPartGetFieldValueContext(fieldName, partDefinitionId));
                return null;
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

        Dictionary<string, Account> GetCachedAccountsBySourceId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountsBySource", () =>
            {
                return GetCachedAccounts().Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value);
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
            var accountTypeManager = new AccountTypeManager();

            var accounts = GetCachedAccounts().Values;
            var accountsByParent = GetCachedAccountsByParent();

            IEnumerable<AccountTypeInfo> accountTypeInfoEntities =
                accountTypeManager.GetAccountTypesInfo(new AccountTypeFilter() { ParentAccountId = account.AccountId });
            ActionDefinitionManager manager = new ActionDefinitionManager();
            IEnumerable<ActionDefinitionInfo> actionDefinitions = manager.GetActionDefinitionInfoByEntityType(EntityType.Account, account.StatusId);

            StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();

            var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId);

            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            return new AccountDetail()
            {
                Entity = account,
                AccountTypeTitle = accountTypeManager.GetAccountTypeName(account.TypeId),
                DirectSubAccountCount = GetSubAccountsCount(account.AccountId, accounts, false),
                TotalSubAccountCount = GetSubAccountsCount(account.AccountId, accounts, true, accountsByParent),
                CanAddSubAccounts = (accountTypeInfoEntities != null && accountTypeInfoEntities.Count() > 0),
                ActionDefinitions = actionDefinitions,
                StatusDesciption = statusDesciption,
                Style = GetStatuStyle(account.StatusId),
                NumberOfServices = accountServices.GetAccountServicesCount(account.AccountId),
                NumberOfPackages = accountPackages.GetAccountPackagesCount(account.AccountId)
            };
        }
        private StyleFormatingSettings GetStatuStyle(Guid statusID)
        {
            StatusDefinitionManager statusDefinitionManager = new StatusDefinitionManager();
            StyleDefinitionManager styleDefinitionManager = new StyleDefinitionManager();
            var status = statusDefinitionManager.GetStatusDefinition(statusID);
            var style = styleDefinitionManager.GetStyleDefinition(status.Settings.StyleDefinitionId);
            return style.StyleDefinitionSettings.StyleFormatingSettings;
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
            return GetAccountName((long)context.EntityId);
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        #endregion


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
    }
}
