using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEDefinitionManager
    {
        #region Public Methods

        public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        {
            var recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();

            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts();

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

            var bigResult = cachedAccounts.ToBigResult(input, filterExpression, account => AccountDetailMapperStep1(account, input.Query.Columns));
            if (bigResult != null && bigResult.Data != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                foreach (var accountDetail in bigResult.Data)
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

        public IEnumerable<AccountInfo> GetAccountsInfo(string nameFilter, AccountFilter filter)
        {
            IEnumerable<Account> allAccounts = GetCachedAccounts().Values;
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            Func<Account, bool> filterFunc = null;

            filterFunc = (account) =>
            {
                if (nameFilterLower != null && !account.Name.Trim().ToLower().StartsWith(nameFilterLower))
                    return false;

                if (filter != null && filter.Filters != null)
                {
                    var context = new AccountFilterContext() { Account = account };
                    if (filter.Filters.Any(x => x.IsExcluded(context)))
                        return false;
                }

                return true;
            };
            return allAccounts.MapRecords(AccountInfoMapper, filterFunc).OrderBy(x => x.Name);
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

        Dictionary<long, AccountTreeNode> GetCacheAccountTreeNodes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCacheAccountTreeNodes", () =>
            {
                Dictionary<long, AccountTreeNode> treeNodes = new Dictionary<long, AccountTreeNode>();
                foreach (var account in GetCachedAccounts().Values)
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
            public List<AccountTreeNode> ChildNodes { get { return _childNodes; } }

            public int TotalSubAccountsCount { get; set; }
        }

        #endregion

        #region Mappers

        private AccountDetail AccountDetailMapper(Account account)
        {
            var accountDetail = AccountDetailMapperStep1(account, null);
            AccountDetailMapperStep2(accountDetail, account);
            return accountDetail;
        }

        private AccountDetail AccountDetailMapperStep1(Account account, List<string> columns)
        {
            var statusDefinitionManager = new StatusDefinitionManager();
            var accountTypeManager = new AccountTypeManager();
            var accountDefinitionManager = new AccountDefinitionManager();
            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            var accountTreeNode = GetCacheAccountTreeNodes().GetRecord(account.AccountId);

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

            List<AccountViewDefinition> accountViewDefinitions = accountDefinitionManager.GetAccountViewDefinitionsByAccount(account);

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
                AvailableAccountViews = accountViewDefinitions != null ? accountViewDefinitions.Select(itm => itm.AccountViewDefinitionId).ToList() : null
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
    }
}
