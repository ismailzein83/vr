using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Retail.BusinessEntity.Data;
using Vanrise.Caching;
using Vanrise.Common.Business;
using System.Collections.Concurrent;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEManager : IBusinessEntityManager
    {
        #region Public Methods

        public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        {
            var recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();

            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(input.Query.AccountBEDefinitionId);

            Func<Account, bool> filterExpression = (account) =>
            {
                if (input.Query.Name != null && !account.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.AccountTypeIds != null && !input.Query.AccountTypeIds.Contains(account.TypeId))
                    return false;

                if (!input.Query.ParentAccountId.HasValue && account.ParentAccountId.HasValue)
                    return false;

                if (input.Query.ParentAccountId.HasValue && (!account.ParentAccountId.HasValue || (account.ParentAccountId.HasValue && input.Query.ParentAccountId.Value != account.ParentAccountId.Value)))
                    return false;

                if (!recordFilterManager.IsFilterGroupMatch(input.Query.FilterGroup, new AccountRecordFilterGenericFieldMatchContext(input.Query.AccountBEDefinitionId, account)))
                    return false;

                return true;
            };

            if (input.SortByColumnName != null && input.SortByColumnName.Contains("FieldValues"))
            {
                string[] fieldProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].Value", fieldProperty[0], fieldProperty[1]);
            }

            var bigResult = cachedAccounts.ToBigResult(input, filterExpression, account => AccountDetailMapperStep1(input.Query.AccountBEDefinitionId, account, input.Query.Columns));
            if (bigResult != null && bigResult.Data != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                foreach (var accountDetail in bigResult.Data)
                {
                    AccountDetailMapperStep2(input.Query.AccountBEDefinitionId, accountDetail, accountDetail.Entity);
                }
            }

            return DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        public Account GetAccount(Guid accountBEDefinitionId, long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(accountBEDefinitionId);
            return cachedAccounts.GetRecord(accountId);
        }
        public string GetAccountName(Guid accountBEDefinitionId, long accountId)
        {
            Account account = this.GetAccount(accountBEDefinitionId, accountId);
            return (account != null) ? account.Name : null;
        }

        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountBEDefinitionId, Guid accountTypeId, int? parentAccountId)
        {
            var accountEditorRuntime = new AccountEditorRuntime();

            var accountPartDefinitionManager = new AccountPartDefinitionManager();

            var runtimeParts = new List<AccountPartRuntime>();

            IEnumerable<AccountTypePartSettings> partSettingsList = GetAccountTypePartDefinitionSettingsList(accountTypeId);
            if (partSettingsList != null && partSettingsList.Count() > 0)
            {
                foreach (AccountTypePartSettings partSettings in partSettingsList)
                {
                    bool isPartFoundOrInherited = this.IsPartFoundOrInherited(accountBEDefinitionId, parentAccountId, partSettings.PartDefinitionId);

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

        public AccountDetail GetAccountDetail(Guid accountBEDefinitionId, long accountId)
        {
            var account = this.GetAccount(accountBEDefinitionId, accountId);
            return account != null ? AccountDetailMapper(account) : null;
        }

        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(AccountToInsert accountToInsert)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountDetail>();
       

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long accountId;

            if (TryAddAccount(accountToInsert, out accountId, false))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountToInsert.AccountBEDefinitionId);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                accountToInsert.AccountId = accountId;
                insertOperationOutput.InsertedObject = AccountDetailMapper(this.GetAccount(accountToInsert.AccountBEDefinitionId, accountToInsert.AccountId));
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        internal bool TryAddAccount(AccountToInsert accountToInsert, out long accountId, bool donotValidateParent)
        {
            ValidateAccountToAdd(accountToInsert, donotValidateParent);

            if (accountToInsert.StatusId == Guid.Empty)
            {
                var accountTypeManager = new AccountTypeManager();
                var accountType = accountTypeManager.GetAccountType(accountToInsert.TypeId);
                if (accountType == null)
                    throw new NullReferenceException("AccountType is null");
                if (accountType.Settings == null)
                    throw new NullReferenceException("AccountType settings is null");

                accountToInsert.StatusId = accountType.Settings.InitialStatusId;
            }

            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();

            return dataManager.Insert(accountToInsert, out accountId);
        }
        
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit accountToEdit)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (TryUpdateAccount(accountToEdit))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountToEdit.AccountBEDefinitionId);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountDetailMapper(this.GetAccount(accountToEdit.AccountBEDefinitionId, accountToEdit.AccountId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        internal bool TryUpdateAccount(AccountToEdit accountToEdit)
        {
            ValidateAccountToEdit(accountToEdit);
            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            return dataManager.Update(accountToEdit);
        }

        public IEnumerable<AccountInfo> GetAccountsInfo(Guid accountBEDefinitionId, string nameFilter, AccountFilter filter)
        {
            IEnumerable<Account> allAccounts = GetCachedAccounts(accountBEDefinitionId).Values;
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            Func<Account, bool> filterFunc = null;

            filterFunc = (account) =>
            {
                if (nameFilterLower != null && !account.Name.Trim().ToLower().StartsWith(nameFilterLower))
                    return false;

                if (filter != null && filter.Filters != null)
                {
                    var context = new AccountFilterContext() { Account = account, AccountBEDefinitionId = accountBEDefinitionId };
                    if (filter.Filters.Any(x => x.IsExcluded(context)))
                        return false;
                }

                return true;
            };
            return allAccounts.MapRecords(AccountInfoMapper, filterFunc).OrderBy(x => x.Name);
        }
        public IEnumerable<AccountInfo> GetAccountsInfoByIds(Guid accountBEDefinitionId, HashSet<long> accountIds)
        {
            List<AccountInfo> accountInfos = new List<AccountInfo>();
            var accounts = GetCachedAccounts(accountBEDefinitionId);
            foreach (var accountId in accountIds)
            {
                var account = accounts.GetRecord(accountId);
                if (account != null)
                    accountInfos.Add(AccountInfoMapper(account));
            }
            return accountInfos.OrderBy(x => x.Name);
        }

        public bool IsAccountMatchWithFilterGroup(Account account, RecordFilterGroup filterGroup)
        {
            AccountType accountType = new AccountTypeManager().GetAccountType(account.TypeId);
            return new Vanrise.GenericData.Business.RecordFilterManager().IsFilterGroupMatch(filterGroup, new AccountRecordFilterGenericFieldMatchContext(accountType.AccountBEDefinitionId, account));
        }
        public bool EvaluateAccountCondition(Account account, AccountCondition accountCondition)
        {
            if (accountCondition == null)
                return true;
            AccountConditionEvaluationContext context = new AccountConditionEvaluationContext();
            context.Account = account;
            return accountCondition.Evaluate(context);
        }
        public bool EvaluateAccountCondition(Guid accountBEDefinitionId, long accountId, AccountCondition accountCondition)
        {
            if (accountCondition == null)
                return true;
            AccountConditionEvaluationContext context = new AccountConditionEvaluationContext(accountBEDefinitionId, accountId);
            return accountCondition.Evaluate(context);
        }

        public bool HasAccountPayment(Guid accountBEDefinitionId, long accountId, bool getInherited, out IAccountPayment accountPayment)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));

            return HasAccountPayment(accountBEDefinitionId, getInherited, account, out accountPayment);
        }
        public bool HasAccountPayment(Guid accountBEDefinitionId, bool getInherited, Account account, out IAccountPayment accountPayment)
        {
            if (account.Settings == null)
            {
                accountPayment = null;
                return false;
            }

            if (account.Settings.Parts != null)
            {
                foreach (var part in account.Settings.Parts)
                {
                    accountPayment = part.Value.Settings as IAccountPayment;
                    if (accountPayment != null)
                        return true;
                }
            }
            if (getInherited && account.ParentAccountId.HasValue)
                return HasAccountPayment(accountBEDefinitionId, account.ParentAccountId.Value, true, out accountPayment);
            else
            {
                accountPayment = null;
                return false;
            }
        }
        public bool HasAccountProfile(Guid accountBEDefinitionId, long accountId, bool getInherited, out IAccountProfile accountProfile)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("account '{0}'", accountId));

            if (account.Settings == null)
            {
                accountProfile = null;
                return false;
            }

            if (account.Settings.Parts != null)
            {
                foreach (var part in account.Settings.Parts)
                {
                    accountProfile = part.Value.Settings as IAccountProfile;
                    if (accountProfile != null)
                        return true;
                }
            }
            if (getInherited && account.ParentAccountId.HasValue)
                return HasAccountProfile(accountBEDefinitionId, account.ParentAccountId.Value, true, out accountProfile);
            else
            {
                accountProfile = null;
                return false;
            }
        }

        public CompanySetting GetCompanySetting(long accountId)
        {
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            return configManager.GetDefaultCompanySetting();
        }

        public Account GetAccountBySourceId(Guid accountBEDefinitionId, string sourceId)
        {
            Dictionary<string, Account> cachedAccounts = this.GetCachedAccountsBySourceId(accountBEDefinitionId);
            return cachedAccounts.GetRecord(sourceId);
        }

        public bool UpdateStatus(Guid accountBEDefinitionId, long accountId, Guid statusId)
        {
            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            bool updateStatus = dataManager.UpdateStatus(accountId, statusId);
            if (updateStatus)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
            }
            return updateStatus;
        }
        public long? GetFinancialAccountId(Guid accountBEDefinitionId, long? accountId)
        {
            if (!accountId.HasValue)
                return null;

            IAccountPayment accountPayment;
            if (HasAccountPayment(accountBEDefinitionId, accountId.Value, false, out accountPayment))
                return accountId;

            var account = GetAccount(accountBEDefinitionId, accountId.Value);
            return GetFinancialAccountId(accountBEDefinitionId, account.ParentAccountId);
        }
        public IEnumerable<Account> GetFinancialAccounts(Guid accountBEDefinitionId)
        {
            var accounts = GetCachedAccounts(accountBEDefinitionId);
            if(accounts == null)
                return null;
           
            var financialAccounts = accounts.Values.FindAllRecords(x =>
                {
                    if (IsFinancial(x))
                        return true;
                    return false;
                });
            return financialAccounts;
        }
        public bool TryGetAccountPart(Guid accountBEDefinitionId, Account account, Guid partDefinitionId, bool getInherited, out AccountPart accountPart)
        {
            if (account.Settings != null && account.Settings.Parts != null && account.Settings.Parts.TryGetValue(partDefinitionId, out accountPart))
                return true;
            else if (getInherited && account.ParentAccountId.HasValue)
            {
                var parentAccount = GetAccount(accountBEDefinitionId, account.ParentAccountId.Value);
                if (parentAccount == null)
                    throw new NullReferenceException(String.Format("parentAccount '{0}'", account.ParentAccountId.Value));
                return TryGetAccountPart(accountBEDefinitionId, parentAccount, partDefinitionId, getInherited, out accountPart);
            }
            else
            {
                accountPart = null;
                return false;
            }
        }
        public bool TryGetAccountPart(Guid accountBEDefinitionId, long accountId, Guid partDefinitionId, bool getInherited, out AccountPart accountPart)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            return TryGetAccountPart(accountBEDefinitionId, account, partDefinitionId, getInherited, out accountPart);
        }
        public bool IsFinancial(Account account)
        {
            IAccountPayment accountPayment;

            if (account != null && account.Settings != null && account.Settings.Parts != null)
            {
                foreach (var part in account.Settings.Parts)
                {
                    accountPayment = part.Value.Settings as IAccountPayment;
                    if (accountPayment != null)
                        return true;
                }
            }
            return false;
        }
        public Account GetSelfOrParentAccountOfType(Guid accountBEDefinitionId, long accountId, Guid accountTypeId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            if (account == null)
                throw new NullReferenceException(String.Format("Account '{0}'", account));
            if (account.TypeId == accountTypeId)
                return account;
            else if (account.ParentAccountId.HasValue)
                return GetSelfOrParentAccountOfType(accountBEDefinitionId, account.ParentAccountId.Value, accountTypeId);
            else return null;
        }
        public bool DeleteAccountExtendedSetting<T>(Guid accountBEDefinitionId, long accountId) where T : BaseAccountExtendedSettings
        {
           return  UpdateAccountExtendedSetting<T>(accountBEDefinitionId, accountId, null);
        }
        public bool UpdateAccountExtendedSetting<T>(Guid accountBEDefinitionId, long accountId, T extendedSettings) where T : BaseAccountExtendedSettings
        {
            Account account = GetAccount(accountBEDefinitionId,accountId);
            if (account.ExtendedSettings == null)
                account.ExtendedSettings = new Dictionary<string, BaseAccountExtendedSettings>();
            string extendedSettingName = typeof(T).FullName;

            BaseAccountExtendedSettings exitingExtendedSettings;
            if (account.ExtendedSettings.TryGetValue(extendedSettingName, out exitingExtendedSettings))
            {
                if (extendedSettings == null)
                    account.ExtendedSettings.Remove(extendedSettingName);
                else
                {
                    account.ExtendedSettings[extendedSettingName] = extendedSettings;
                }
            }
            else if (extendedSettings != null)
            {
               account.ExtendedSettings.Add(extendedSettingName, extendedSettings);
            }
            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            if (dataManager.UpdateExtendedSettings(accountId, account.ExtendedSettings))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
                return true;
            }
            return false;
        }
        public T GetExtendedSettings<T>(Guid accountBEDefinitionId, long accountId) where T : BaseAccountExtendedSettings
        {
            Account account = GetAccount(accountBEDefinitionId,accountId);
            return account != null ? GetExtendedSettings<T>(account) : default(T);
        }
        public T GetExtendedSettings<T>(Account account) where T : BaseAccountExtendedSettings
        {
            string extendedSettingName = typeof(T).FullName;
            BaseAccountExtendedSettings exitingExtendedSettings;
            if (account.ExtendedSettings != null)
            {
                account.ExtendedSettings.TryGetValue(extendedSettingName, out exitingExtendedSettings);
                if (exitingExtendedSettings != null)
                    return exitingExtendedSettings as T;
                else return default(T);
            }
            else
                return default(T);
        }

        public Dictionary<long, Account> GetAccounts(Guid accountBEDefinitionId)
        {
            return GetCachedAccounts(accountBEDefinitionId);
        }
       
        #endregion

        #region Private Methods

        public Dictionary<long, Account> GetCachedAccounts(Guid accountBEDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccounts", accountBEDefinitionId,
                () =>
                {
                    IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
                    IEnumerable<Account> accounts = dataManager.GetAccounts(accountBEDefinitionId);
                    return accounts.ToDictionary(kvp => kvp.AccountId, kvp => kvp);
                });
        }

        public Dictionary<string, Account> GetCachedAccountsBySourceId(Guid accountBEDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountsBySourceId", accountBEDefinitionId,
                () =>
                {
                    return GetCachedAccounts(accountBEDefinitionId).Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value);
                });
        }

        Dictionary<long, AccountTreeNode> GetCacheAccountTreeNodes(Guid accountBEDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCacheAccountTreeNodes_{0}", accountBEDefinitionId), accountBEDefinitionId,
                () =>
                {
                    Dictionary<long, AccountTreeNode> treeNodes = new Dictionary<long, AccountTreeNode>();
                    foreach (var account in GetCachedAccounts(accountBEDefinitionId).Values)
                    {
                        AccountTreeNode node = new AccountTreeNode
                        {
                            Account = account
                        };
                        treeNodes.Add(account.AccountId, node);
                    }

                    //updating nodes parent info
                    foreach (var node in treeNodes.Values)
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

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IAccountBEDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            ConcurrentDictionary<Guid, Object> _updateHandlesByBEDefinitionId = new ConcurrentDictionary<Guid, Object>();


            protected override bool ShouldSetCacheExpired(Guid accountBEDefinitionId)
            {
                object _updateHandle;

                _updateHandlesByBEDefinitionId.TryGetValue(accountBEDefinitionId, out _updateHandle);
                bool isCacheExpired = _dataManager.AreAccountsUpdated(accountBEDefinitionId, ref _updateHandle);
                _updateHandlesByBEDefinitionId.AddOrUpdate(accountBEDefinitionId, _updateHandle, (key, existingHandle) => _updateHandle);

                return isCacheExpired;
            }
        }

        private class AccountRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {

            Guid _accountBEDefinitionId;
            Account _account;
            AccountTypeManager _accountTypeManager = new AccountTypeManager();
            public AccountRecordFilterGenericFieldMatchContext(Guid accountBEDefinitionId, Account account)
            {
                this._accountBEDefinitionId = accountBEDefinitionId;
                this._account = account;
            }

            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                var accountGenericField = _accountTypeManager.GetAccountGenericField(_accountBEDefinitionId, fieldName);
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

        #region Get Account Editor Runtime

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

        private bool IsPartFoundOrInherited(Guid accountBEDefinitionId, long? accountId, Guid partDefinitionId)
        {
            if (!accountId.HasValue)
                return false;

            Account account = this.GetAccount(accountBEDefinitionId, accountId.Value);
            if (account == null)
                throw new NullReferenceException("account");
            if (account.Settings == null)
                return false;

            if (account.Settings.Parts == null)
                return false;

            AccountPart accountPart;
            bool isPartFound = account.Settings.Parts.TryGetValue(partDefinitionId, out accountPart);

            if (isPartFound)
                return true;

            return IsPartFoundOrInherited(accountBEDefinitionId, account.ParentAccountId, partDefinitionId);
        }

        #endregion

        #region Validation Methods

        private void ValidateAccountToAdd(AccountToInsert accountToInsert, bool donotValidateParent)
        {
            ValidateAccount(accountToInsert.AccountBEDefinitionId, accountToInsert.TypeId, accountToInsert.AccountId, accountToInsert.Name, accountToInsert.ParentAccountId, donotValidateParent);
        }

        private void ValidateAccountToEdit(AccountToEdit accountToEdit)
        {
            Account accountEntity = this.GetAccount(accountToEdit.AccountBEDefinitionId, accountToEdit.AccountId);

            if (accountEntity == null)
                throw new DataIntegrityValidationException(String.Format("Account '{0}' does not exist", accountToEdit.AccountId));

            ValidateAccount(accountToEdit.AccountBEDefinitionId, accountToEdit.TypeId, accountToEdit.AccountId, accountToEdit.Name, null, false);

        }

        private void ValidateAccount(Guid accountBEDefinitionId, Guid accountTypeId, long accountId, string name, long? parentAccountId, bool donotValidateParent)
        {
            var accountType = new AccountTypeManager().GetAccountType(accountTypeId);
            if (accountType.AccountBEDefinitionId != accountBEDefinitionId)
                throw new DataIntegrityValidationException(string.Format("accountType.AccountBEDefinitionId '{0}' is different than passed accountBEDefinitionId '{1}'",
                    accountType.AccountBEDefinitionId, accountBEDefinitionId));

            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("Account.Name");

            if (!donotValidateParent && parentAccountId.HasValue)
            {
                Account parentAccount = this.GetAccount(accountBEDefinitionId, parentAccountId.Value);
                if (parentAccount == null)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not exist", parentAccountId));
            }
        }

        #endregion

        #region Mappers

        private AccountDetail AccountDetailMapper(Account account)
        {
            AccountType accountType = new AccountTypeManager().GetAccountType(account.TypeId);
            var accountDetail = AccountDetailMapperStep1(accountType.AccountBEDefinitionId, account, null);
            AccountDetailMapperStep2(accountType.AccountBEDefinitionId, accountDetail, account);
            return accountDetail;
        }

        private AccountDetail AccountDetailMapperStep1(Guid accountBEDefinitionId, Account account, List<string> columns)
        {
            var statusDefinitionManager = new StatusDefinitionManager();
            var accountTypeManager = new AccountTypeManager();
            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            var accountTreeNode = GetCacheAccountTreeNodes(accountBEDefinitionId).GetRecord(account.AccountId);

            //Dynamic Part
            Dictionary<string, AccountFieldValue> fieldValues = new Dictionary<string, AccountFieldValue>();
            Dictionary<string, AccountGenericField> accountGenericFields = accountTypeManager.GetAccountGenericFields(accountBEDefinitionId);

            foreach (var field in accountGenericFields.Values)
            {
                if (columns != null && !columns.Contains(field.Name))
                    continue;

                object value = field.GetValue(new AccountGenericFieldContext(account));

                AccountFieldValue accountFieldValue = new AccountFieldValue();
                accountFieldValue.Value = value;
                accountFieldValue.Description = field.FieldType.GetDescription(value);

                fieldValues.Add(field.Name, accountFieldValue);
            }

            return new AccountDetail()
            {
                Entity = account,
                AccountTypeTitle = accountTypeManager.GetAccountTypeName(account.TypeId),
                DirectSubAccountCount = accountTreeNode.ChildNodes.Count,
                TotalSubAccountCount = accountTreeNode.TotalSubAccountsCount,
                StatusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId),
                NumberOfServices = accountServices.GetAccountServicesCount(account.AccountId),
                NumberOfPackages = accountPackages.GetAccountPackagesCount(account.AccountId),
                FieldValues = fieldValues         
            };
        }

        private void AccountDetailMapperStep2(Guid accountBEDefinitionId, AccountDetail accountDetail, Account account)
        {
            IEnumerable<AccountTypeInfo> accountTypeInfoEntities = new AccountTypeManager().GetAccountTypesInfo(new AccountTypeFilter() { ParentAccountId = account.AccountId });
            
            var accountBEDefinitionManager = new AccountBEDefinitionManager();
            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionManager.GetAccountViewDefinitionsByAccount(accountBEDefinitionId, account);
            List<AccountActionDefinition> accountActionDefinitions = accountBEDefinitionManager.GetAccountActionDefinitionsByAccount(accountBEDefinitionId, account);

            accountDetail.CanAddSubAccounts = (accountTypeInfoEntities != null && accountTypeInfoEntities.Count() > 0);
            accountDetail.AvailableAccountViews = accountViewDefinitions != null ? accountViewDefinitions.Select(itm => itm.AccountViewDefinitionId).ToList() : null;
            accountDetail.AvailableAccountActions = accountActionDefinitions != null ? accountActionDefinitions.Select(itm => itm.AccountActionDefinitionId).ToList() : null;
            accountDetail.Style = GetStatuStyle(account.StatusId);
        }

        private StyleFormatingSettings GetStatuStyle(Guid statusID)
        {
            Vanrise.Common.Business.StatusDefinitionManager statusDefinitionManager = new Vanrise.Common.Business.StatusDefinitionManager();
            StyleDefinitionManager styleDefinitionManager = new StyleDefinitionManager();
            var status = statusDefinitionManager.GetStatusDefinition(statusID);
            if (status != null)
            {
                var style = styleDefinitionManager.GetStyleDefinition(status.Settings.StyleDefinitionId);
                return style.StyleDefinitionSettings.StyleFormatingSettings;
            }
            else
                return null;
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
            var cachedAccounts = GetCachedAccounts(context.EntityDefinitionId);
            if (cachedAccounts != null)
                return cachedAccounts.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetAccount(context.EntityDefinitionId, context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetAccountName(context.EntityDefinition.BusinessEntityDefinitionId, Convert.ToInt64(context.EntityId));
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(context.EntityDefinitionId, ref lastCheckTime);
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
