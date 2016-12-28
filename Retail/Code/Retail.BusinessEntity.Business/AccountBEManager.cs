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

                if (!recordFilterManager.IsFilterGroupMatch(input.Query.FilterGroup, new AccountRecordFilterGenericFieldMatchContext(account)))
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
                    AccountDetailMapperStep2(accountDetail, accountDetail.Entity);
                }
            }

            return DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        public Account GetAccount(Guid accountDefinitionId, long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(accountDefinitionId);
            return cachedAccounts.GetRecord(accountId);
        }

        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Guid accountDefinitionId, Account account)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long accountId;

            if (TryAddAccount(accountDefinitionId, account, out accountId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountDefinitionId);
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
        internal bool TryAddAccount(Guid accountDefinitionId, Account account, out long accountId)
        {
            ValidateAccountToAdd(accountDefinitionId, account);

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

            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();

            return dataManager.Insert(account, out accountId);
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(Guid accountDefinitionId, AccountToEdit account)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (TryUpdateAccount(accountDefinitionId, account))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountDefinitionId);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountDetailMapper(this.GetAccount(accountDefinitionId, account.AccountId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        internal bool TryUpdateAccount(Guid accountDefinitionId, AccountToEdit account)
        {
            long? parentId;
            ValidateAccountToEdit(accountDefinitionId, account, out parentId);

            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();

            return dataManager.Update(account, parentId);
        }

        public string GetAccountName(Guid accountDefinitionId, long accountId)
        {
            Account account = this.GetAccount(accountDefinitionId, accountId);
            return (account != null) ? account.Name : null;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IAccountBEDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(Guid accountDefinitionId)
            {
                return _dataManager.AreAccountsUpdated(accountDefinitionId, ref _updateHandle);
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

        public Dictionary<long, Account> GetCachedAccounts(Guid accountDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetAccounts_{0}", accountDefinitionId), accountDefinitionId,
                () =>
                {
                    IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
                    IEnumerable<Account> accounts = dataManager.GetAccounts(accountDefinitionId);
                    return accounts.ToDictionary(kvp => kvp.AccountId, kvp => kvp);
                });
        }

        Dictionary<long, AccountTreeNode> GetCacheAccountTreeNodes(Guid accountDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCacheAccountTreeNodes_{0}", accountDefinitionId), accountDefinitionId,
                () =>
                {
                    Dictionary<long, AccountTreeNode> treeNodes = new Dictionary<long, AccountTreeNode>();
                    foreach (var account in GetCachedAccounts(accountDefinitionId).Values)
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

        #region Validation Methods

        private void ValidateAccountToAdd(Guid accountDefinitionId, Account account)
        {
            ValidateAccount(accountDefinitionId, account.AccountId, account.Name, account.ParentAccountId);
        }

        private void ValidateAccountToEdit(Guid accountDefinitionId, AccountToEdit account, out long? parentAccountId)
        {
            Account accountEntity = this.GetAccount(accountDefinitionId, account.AccountId);

            if (accountEntity == null)
                throw new DataIntegrityValidationException(String.Format("Account '{0}' does not exist", account.AccountId));

            parentAccountId = accountEntity.ParentAccountId;
            ValidateAccount(accountDefinitionId, account.AccountId, account.Name, accountEntity.ParentAccountId);

            if (parentAccountId.HasValue)
            {
                IEnumerable<long> subAccountIds = this.GetSubAccountIds(accountDefinitionId, parentAccountId.Value);
                if (subAccountIds == null || subAccountIds.Count() == 0)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not have any sub accounts", parentAccountId));
                if (!subAccountIds.Contains(account.AccountId))
                    throw new DataIntegrityValidationException(String.Format("Account '{0}' is not a sub account of Account '{1}'", account.AccountId, parentAccountId));
            }
        }

        private void ValidateAccount(Guid accountDefinitionId, long accountId, string name, long? parentAccountId)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("Account.Name");

            if (parentAccountId.HasValue)
            {
                Account parentAccount = this.GetAccount(accountDefinitionId, parentAccountId.Value);
                if (parentAccount == null)
                    throw new DataIntegrityValidationException(String.Format("ParentAccount '{0}' does not exist", parentAccountId));
            }
        }

        private IEnumerable<long> GetSubAccountIds(Guid accountDefinitionId, long parentAccountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(accountDefinitionId);
            return cachedAccounts.MapRecords(itm => itm.Value.AccountId, itm => itm.Value.ParentAccountId.HasValue && itm.Value.ParentAccountId == parentAccountId);
        }

        #endregion

        #region Mappers

        private AccountDetail AccountDetailMapper(Account account)
        {
            AccountType accountType = new AccountTypeManager().GetAccountType(account.TypeId);
            var accountDetail = AccountDetailMapperStep1(accountType.AccountBEDefinitionId, account, null);
            AccountDetailMapperStep2(accountDetail, account);
            return accountDetail;
        }

        private AccountDetail AccountDetailMapperStep1(Guid accountBEDefinitionId, Account account, List<string> columns)
        {
            var statusDefinitionManager = new StatusDefinitionManager();
            var accountTypeManager = new AccountTypeManager();
            var accountBEDefinitionManager = new AccountBEDefinitionManager();
            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            var accountTreeNode = GetCacheAccountTreeNodes(accountBEDefinitionId).GetRecord(account.AccountId);

            //Dynamic Part
            IEnumerable<GenericFieldDefinitionInfo> genericFieldDefinitionInfos = accountTypeManager.GetGenericFieldDefinitionsInfo();
            Dictionary<string, AccountFieldValue> fieldValues = new Dictionary<string, AccountFieldValue>();

            foreach (var field in genericFieldDefinitionInfos)
            {
                if (columns != null && !columns.Contains(field.Name))
                    continue;

                AccountFieldValue accountFieldValue = new AccountFieldValue();

                AccountGenericField accountGenericField = new AccountTypeManager().GetAccountGenericField(field.Name);
                if (accountGenericField == null)
                    throw new NullReferenceException(String.Format("accountGenericField '{0}'", field.Name));

                object value = accountGenericField.GetValue(new AccountGenericFieldContext(account));
                accountFieldValue.Value = value;
                accountFieldValue.Description = field.FieldType.GetDescription(value);

                fieldValues.Add(field.Name, accountFieldValue);
            }

            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionManager.GetAccountViewDefinitionsByAccount(accountBEDefinitionId, account);
            List<AccountActionDefinition> accountActionDefinitions = accountBEDefinitionManager.GetAccountActionDefinitionsByAccount(accountBEDefinitionId, account);

            return new AccountDetail()
            {
                Entity = account,
                AccountTypeTitle = accountTypeManager.GetAccountTypeName(account.TypeId),
                DirectSubAccountCount = accountTreeNode.ChildNodes.Count,
                TotalSubAccountCount = accountTreeNode.TotalSubAccountsCount,
                StatusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId),
                NumberOfServices = accountServices.GetAccountServicesCount(account.AccountId),
                NumberOfPackages = accountPackages.GetAccountPackagesCount(account.AccountId),
                FieldValues = fieldValues,
                AvailableAccountViews = accountViewDefinitions != null ? accountViewDefinitions.Select(itm => itm.AccountViewDefinitionId).ToList() : null,
                AvailableAccountActions = accountActionDefinitions != null ? accountActionDefinitions.Select(itm => itm.AccountActionDefinitionId).ToList() : null
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
