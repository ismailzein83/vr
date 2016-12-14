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
            var bigResult = cachedAccounts.ToBigResult(input, filterExpression, AccountDetailMapperStep1);
            if (bigResult != null && bigResult.Data != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                foreach(var accountDetail in bigResult.Data)
                {
                    AccountDetailMapperStep2(accountDetail, accountDetail.Entity);
                }
            }

            return DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        public Account GetAccount(long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();
            return cachedAccounts.GetRecord(accountId);
        }

        public Account GetSelfOrParentAccountOfType(long accountId, Guid accountTypeId)
        {
            var account = GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("Account '{0}'", account));
            if (account.TypeId == accountTypeId)
                return account;
            else if (account.ParentAccountId.HasValue)
                return GetSelfOrParentAccountOfType(account.ParentAccountId.Value, accountTypeId);
            else return null;
        }

        public Account GetAccountBySourceId(string sourceId)
        {
            Dictionary<string, Account> cachedAccounts = this.GetCachedAccountsBySourceId();
            return cachedAccounts.GetRecord(sourceId);
        }
        public AccountDetail GetAccountDetail(long accountId)
        {
            var account = GetAccount(accountId);
            return account != null ? AccountDetailMapper(account) : null;
        }


        public IEnumerable<AccountInfo> GetAccountsInfo(string nameFilter, AccountFilter filter)
        {
            IEnumerable<Account> allAccounts = GetCachedAccounts().Values;
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            Func<Account, bool> filterFunc = null;
            if (filter != null)
            {
                filterFunc = (account) =>
                {
                    if (filter.Filters != null)
                    {
                        var context = new AccountFilterContext() {  Account = account };
                        if (filter.Filters.Any(x => x.IsExcluded(context)))
                            return false;
                    }

                    if (nameFilterLower != null && !account.Name.ToLower().Contains(nameFilterLower))
                        return false;

                    return true;
                };
            }
            return allAccounts.MapRecords(AccountInfoMapper, filterFunc).OrderBy(x => x.Name);
        }

        public IEnumerable<AccountInfo> GetAccountsInfoByIds(HashSet<long> accountIds)
        {
            List<AccountInfo> accountInfos = new List<AccountInfo>();
            var accounts = GetCachedAccounts();
            foreach(var accountId in accountIds)
            {
                var account = accounts.GetRecord(accountId);
                if (account != null)
                    accountInfos.Add(AccountInfoMapper(account));
            }
            return accountInfos.OrderBy(x => x.Name);            
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
            bool updateExecutedAction = dataManager.UpdateExecutedActions(accountId, executedActions);
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

        public bool TryGetAccountPart(long accountId, Guid partDefinitionId, bool getInherited, out AccountPart accountPart)
        {
            var account = GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));
            return TryGetAccountPart(account, partDefinitionId, getInherited, out accountPart);
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

        public int GetAccountDuePeriod(long accountId)
        {
            return 0;
        }

        #region Get Account Editor Runtime

        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountTypeId, int? parentAccountId)
        {
            var accountEditorRuntime = new AccountEditorRuntime();

            var accountPartDefinitionManager = new AccountPartDefinitionManager();

            var runtimeParts = new List<AccountPartRuntime>();

            IEnumerable<AccountTypePartSettings> partSettingsList = GetAccountTypePartDefinitionSettingsList(accountTypeId);
            if (partSettingsList != null && partSettingsList.Count() > 0)
            {
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
                var accountGenericField = _accountTypeManager.GetAccountGenericField(fieldName);
                if (accountGenericField == null)
                    throw new NullReferenceException(String.Format("accountGenericField '{0}'", fieldName));
                fieldType = accountGenericField.FieldType;
                return accountGenericField.GetValue(new AccountGenericFieldContext(_account));
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

        Dictionary<long, Account> GetCachedAccounts()
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

        Dictionary<long, AccountTreeNode> GetCacheAccountTreeNodes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCacheAccountTreeNodes", () =>
              {
                  Dictionary<long, AccountTreeNode> treeNodes = new Dictionary<long, AccountTreeNode>();
                  foreach(var account in GetCachedAccounts().Values)
                  {
                      AccountTreeNode node = new AccountTreeNode
                      {
                          Account = account
                      };                      
                      treeNodes.Add(account.AccountId, node);
                  }
                  //updating nodes parent info
                  foreach(var node in treeNodes.Values)
                  {
                      var account = node.Account;
                      if (account.ParentAccountId.HasValue)
                      {
                          AccountTreeNode parentNode;
                          if (treeNodes.TryGetValue(account.ParentAccountId.Value, out parentNode))
                          {
                              node.ParentNode = parentNode;
                              parentNode.ChildNodes.Add(node);
                              parentNode.TotalSubAccountsCount++;
                              while (parentNode.ParentNode != null)
                              {
                                  parentNode = parentNode.ParentNode;
                                  parentNode.TotalSubAccountsCount++;
                              }
                          }
                      }
                  }
                  return treeNodes;
              });
        }

        #endregion

        #region Mappers

        private AccountDetail AccountDetailMapper(Account account)
        {
            var accountDetail = AccountDetailMapperStep1(account);
            AccountDetailMapperStep2(accountDetail, account);
            return accountDetail;
        }

        private AccountDetail AccountDetailMapperStep1(Account account)
        {
            var accountTypeManager = new AccountTypeManager();            
            StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();

            var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId);

            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            var accountTreeNode = GetCacheAccountTreeNodes().GetRecord(account.AccountId);
            return new AccountDetail()
            {
                Entity = account,
                AccountTypeTitle = accountTypeManager.GetAccountTypeName(account.TypeId),
                DirectSubAccountCount = accountTreeNode.ChildNodes.Count,
                TotalSubAccountCount = accountTreeNode.TotalSubAccountsCount,
                StatusDesciption = statusDesciption,
                NumberOfServices = accountServices.GetAccountServicesCount(account.AccountId),
                NumberOfPackages = accountPackages.GetAccountPackagesCount(account.AccountId)
            };
        }

        private void AccountDetailMapperStep2(AccountDetail accountDetail, Account account)
        {
            var accountTypeManager = new AccountTypeManager();
            IEnumerable<AccountTypeInfo> accountTypeInfoEntities = accountTypeManager.GetAccountTypesInfo(new AccountTypeFilter() { ParentAccountId = account.AccountId });
            accountDetail.CanAddSubAccounts = (accountTypeInfoEntities != null && accountTypeInfoEntities.Count() > 0);
            ActionDefinitionManager actionDefinitionManager = new ActionDefinitionManager();
            accountDetail.ActionDefinitions = actionDefinitionManager.GetActionDefinitionInfoByEntityType(EntityType.Account, account.StatusId);
            accountDetail.Style = GetStatuStyle(account.StatusId);
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
