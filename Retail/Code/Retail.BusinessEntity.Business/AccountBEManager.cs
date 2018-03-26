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
using Vanrise.Security.Business;
using Retail.BusinessEntity.APIEntities;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEManager : BaseBusinessEntityManager, IAccountBEManager
    {
        #region Ctor/Properties

        static AccountBEDefinitionManager _accountBEDefinitionManager;
        static AccountTypeManager s_accountTypeManager = new AccountTypeManager();
        static Vanrise.Common.Business.StatusDefinitionManager s_statusDefinitionManager = new Vanrise.Common.Business.StatusDefinitionManager();
        static AccountPartDefinitionManager s_partDefinitionManager = new AccountPartDefinitionManager();
        public AccountBEManager()
        {
            _accountBEDefinitionManager = new AccountBEDefinitionManager();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountClientDetail> GetFilteredClientAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(input.Query.AccountBEDefinitionId);

            Func<Account, bool> filterExpression = (account) =>
            {
                if (input.Query.ParentAccountId.HasValue && (!account.ParentAccountId.HasValue || (account.ParentAccountId.HasValue && input.Query.ParentAccountId.Value != account.ParentAccountId.Value)))
                    return false;
                if (input.Query.StatusIds != null && !input.Query.StatusIds.Contains(account.StatusId))
                {
                    return false;
                }
                return true;
            };
            if (input.SortByColumnName != null && input.SortByColumnName.Contains("FieldValues"))
            {
                string[] fieldProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].{2}", fieldProperty[0], fieldProperty[1], fieldProperty[2]);
            }

            var bigResult = cachedAccounts.ToBigResult(input, filterExpression, account => AccountDetailMapperStep1(input.Query.AccountBEDefinitionId, account, input.Query.Columns));
            BigResult<AccountClientDetail> finalResult = new BigResult<AccountClientDetail>
            {
                ResultKey = bigResult.ResultKey,
                TotalCount = bigResult.TotalCount
            };
            List<AccountClientDetail> accountClientDetails = null;
            if (bigResult != null && bigResult.Data != null)
            {
                 accountClientDetails = new List<AccountClientDetail>();
                foreach (var accountDetail in bigResult.Data)
                {
                    accountClientDetails.Add(new AccountClientDetail
                    {
                        AccountId = accountDetail.AccountId,
                        FieldValues = accountDetail.FieldValues,
                        HasSubAccounts = accountDetail.TotalSubAccountCount != 0
                    });
                }
                finalResult.Data = accountClientDetails;
            }

            ResultProcessingHandler<AccountClientDetail> handler = new ResultProcessingHandler<AccountClientDetail>()
            {
                ExportExcelHandler = new AccountClientExcelExportHandler(input.Query)
            };
            return DataRetrievalManager.Instance.ProcessResult(input, finalResult, handler);
        }
        public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        {
            var recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();
            var accountTypeManager = new AccountTypeManager();

            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(input.Query.AccountBEDefinitionId);

            Func<Account, bool> filterExpression = (account) =>
            {
                if (input.Query.Name != null && !account.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.AccountTypeIds != null && !input.Query.AccountTypeIds.Contains(account.TypeId))
                    return false;

                if (input.Query.OnlyRootAccount)
                {
                    AccountType accountType = accountTypeManager.GetAccountType(account.TypeId);
                    accountType.ThrowIfNull("accountType", account.TypeId);
                    accountType.Settings.ThrowIfNull("accountType.Settings", account.TypeId);
                    if (!accountType.Settings.CanBeRootAccount)
                        return false;
                }

                if (input.Query.ParentAccountId.HasValue && (!account.ParentAccountId.HasValue || (account.ParentAccountId.HasValue && input.Query.ParentAccountId.Value != account.ParentAccountId.Value)))
                    return false;

                if (!recordFilterManager.IsFilterGroupMatch(input.Query.FilterGroup, new AccountRecordFilterGenericFieldMatchContext(input.Query.AccountBEDefinitionId, account)))
                    return false;
                if (input.Query.StatusIds != null && !input.Query.StatusIds.Contains(account.StatusId))
                {
                    return false;
                }
                return true;
            };

            if (input.SortByColumnName != null && input.SortByColumnName.Contains("FieldValues"))
            {
                string[] fieldProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].{2}", fieldProperty[0], fieldProperty[1], fieldProperty[2]);
            }

            var bigResult = cachedAccounts.ToBigResult(input, filterExpression, account => AccountDetailMapperStep1(input.Query.AccountBEDefinitionId, account, input.Query.Columns));

            var filtredActionIds = _accountBEDefinitionManager.GetLoggedInUserAllowedActionIds(input.Query.AccountBEDefinitionId);
            var filterdViewIds = _accountBEDefinitionManager.GetLoggedInUserAllowedViewIds(input.Query.AccountBEDefinitionId);

            if (bigResult != null && bigResult.Data != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                foreach (var accountDetail in bigResult.Data)
                {
                    AccountDetailMapperStep2(input.Query.AccountBEDefinitionId, accountDetail, accountDetail.AccountId, filtredActionIds, filterdViewIds);
                }
            }

            ResultProcessingHandler<AccountDetail> handler = new ResultProcessingHandler<AccountDetail>()
            {
                ExportExcelHandler = new AccountExcelExportHandler(input.Query)
            };
            VRActionLogger.Current.LogGetFilteredAction(new AccountBELoggableEntity(input.Query.AccountBEDefinitionId), input);
            return DataRetrievalManager.Instance.ProcessResult(input, bigResult, handler);
        }
        public Account GetAccount(Guid accountBEDefinitionId, long accountId)
        {
            Dictionary<long, Account> cachedAccounts = this.GetCachedAccounts(accountBEDefinitionId);
            return cachedAccounts.GetRecord(accountId);
        }
        public string GetAccountName(Guid accountBEDefinitionId, long accountId)
        {
            Account account = this.GetAccount(accountBEDefinitionId, accountId);
            if (account != null)
            {
                return GetAccountName(account);
            }
            return null;
        }
        public string GetAccountName(Account account)
        {
            if (account != null)
            {
                AccountType accountType = new AccountTypeManager().GetAccountType(account.TypeId);
                accountType.ThrowIfNull("accountType", account.TypeId);
                string name = account.Name;
                if (accountType.Settings.ShowConcatenatedName && account.ParentAccountId.HasValue)
                {
                    name = BuildAccountFullName(accountType.AccountBEDefinitionId, account.ParentAccountId.Value, name);
                }
                return name;
            }
            return null;
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
                        partDefinition.ThrowIfNull("partDefinition", partSettings.PartDefinitionId);

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

            Account accountBE;          
            if (TryAddAccount(accountToInsert, out accountId, false, out accountBE))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                accountToInsert.AccountId = accountId;
                insertOperationOutput.InsertedObject = AccountDetailMapper(accountBE);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit accountToEdit)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;            
            Account account;        
            if (TryUpdateAccount(accountToEdit, out account))
            {                
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountDetailMapper(account);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
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
            AccountConditionEvaluationContext context = PrepareAccountConditionEvaluationContext(account, accountCondition);
            return accountCondition.Evaluate(context);
        }
        public bool EvaluateAccountCondition(Guid accountBEDefinitionId, long accountId, AccountCondition accountCondition)
        {
            if (accountCondition == null)
                return true;
            AccountConditionEvaluationContext context = PrepareAccountConditionEvaluationContext(accountBEDefinitionId, accountId, accountCondition);
            return accountCondition.Evaluate(context);
        }
        private AccountConditionEvaluationContext PrepareAccountConditionEvaluationContext(Account account, AccountCondition accountCondition)
        {
            switch (accountCondition.TargetType)
            {
                case TargetType.Parent:
                    var parentAccount = GetParentAccount(account);
                    return new AccountConditionEvaluationContext { Account = parentAccount };
                case TargetType.Self:
                    return new AccountConditionEvaluationContext { Account = account };
            }
            return null;
        }
        private AccountConditionEvaluationContext PrepareAccountConditionEvaluationContext(Guid accountBEDefinitionId, long accountId, AccountCondition accountCondition)
        {
            switch (accountCondition.TargetType)
            {
                case TargetType.Parent:
                    var parentAccount = GetParentAccount(accountBEDefinitionId, accountId);
                    return new AccountConditionEvaluationContext { Account = parentAccount };
                case TargetType.Self: return new AccountConditionEvaluationContext(accountBEDefinitionId, accountId);
            }
            return null;
        }
        public Account GetParentAccount(Account account)
        {
            if (!account.ParentAccountId.HasValue)
                return null;
            var accountBEDefinitionId = new AccountTypeManager().GetAccountBEDefinitionId(account.TypeId);
            return GetAccount(accountBEDefinitionId, account.ParentAccountId.Value);
        }
        public Account GetParentAccount(Guid accountBEDefinitionId, long accountId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            if (!account.ParentAccountId.HasValue)
                return null;
            return GetAccount(accountBEDefinitionId, account.ParentAccountId.Value);
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

        public int GetCityId(Guid accountBEDefinitionId, long accountId, bool getInherited)
        {
            IAccountProfile accountProfile;
            if (!HasAccountProfile(accountBEDefinitionId, accountId, getInherited, out accountProfile))
                throw new NullReferenceException("accountProfile");

            if (!accountProfile.CityId.HasValue)
                throw new NullReferenceException("accountProfile.CityId");
            return accountProfile.CityId.Value;

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
        public IEnumerable<Guid> GetBankDetailsIds(long accountId)
        {
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            var companySettings = GetCompanySetting(accountId);
            return companySettings.BankDetails;
        }

        public Account GetAccountBySourceId(Guid accountBEDefinitionId, string sourceId)
        {
            Dictionary<string, Account> cachedAccounts = this.GetCachedAccountsBySourceId(accountBEDefinitionId);
            return cachedAccounts.GetRecord(sourceId);
        }
        //public bool UpdateStatus(Guid accountBEDefinitionId, long accountId, Guid statusId, string actionName = null)
        //{
        //    var existingAccount = GetAccount(accountBEDefinitionId, accountId);
        //    existingAccount.ThrowIfNull("existingAccount", accountId);
        //    Guid previousStatusId = existingAccount.StatusId;
        //    IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
        //    bool updateStatus = dataManager.UpdateStatus(accountId, statusId);
        //    if (updateStatus)
        //    {
        //        new AccountStatusHistoryManager().AddAccountStatusHistory(accountBEDefinitionId, accountId, statusId, previousStatusId);
        //        if (actionName != null)
        //        {
        //            VRActionLogger.Current.LogObjectCustomAction(new AccountBELoggableEntity(accountBEDefinitionId), actionName, true, GetAccount(accountBEDefinitionId, accountId));
        //        }
        //        else
        //        {
        //            var status = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinition(statusId);
        //            VRActionLogger.Current.LogObjectCustomAction(new AccountBELoggableEntity(accountBEDefinitionId), string.Format("Update Status"), true, GetAccount(accountBEDefinitionId, accountId), string.Format("Status Changed to {0}", status.Name));
        //        }

        //        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
        //    }
        //    return updateStatus;
        //}
        public void UpdateStatuses(Guid accountBEDefinitionId, List<UpdateAccountStatusInput> accountsStatusesInfo, string actionName = null)
        {
            Dictionary<Guid, List<Account>> accountsByStatusId = new Dictionary<Guid, List<Account>>();
            foreach (var accountStatusInfo in accountsStatusesInfo)
            {
                var account = GetAccount(accountBEDefinitionId, accountStatusInfo.AccountId);
                account.ThrowIfNull("account", accountStatusInfo.AccountId);
                var accounts = accountsByStatusId.GetOrCreateItem(accountStatusInfo.StatusId);
                accounts.Add(account);
            }
            foreach (var accountsByStatus in accountsByStatusId)
            {
                string errorMessage;
                UpdateStatuses(accountBEDefinitionId, accountsByStatus.Value, accountsByStatus.Key, DateTime.Now, false, null, out errorMessage, false, actionName);
            }
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
        }
        public void UpdateStatuses(Guid accountBEDefinitionId, List<long> accountIds, Guid statusId, out string errorMessage, string actionName = null)
        {
            List<Account> accounts = new List<Account>();
            foreach (var accountId in accountIds.Distinct())
            {
                var account = GetAccount(accountBEDefinitionId, accountId);
                account.ThrowIfNull("account", accountId);
                accounts.Add(account);
            }
            UpdateStatuses(accountBEDefinitionId, accounts, statusId, DateTime.Now, false, null, out errorMessage, true, actionName);
        }
        //public void UpdateStatuses(Guid accountBEDefinitionId, List<long> accountIds, Guid statusId, DateTime statusChangedDate, bool allowOverlapping, List<Guid> applicableOnStatuses, bool withCacheExpire = true, string actionName = null)
        //{
        //    List<Account> accounts = new List<Account>();
        //    foreach (var accountId in accountIds.Distinct())
        //    {
        //        var account = GetAccount(accountBEDefinitionId, accountId);
        //        account.ThrowIfNull("account", accountId);
        //        accounts.Add(account);
        //    }
        //    UpdateStatuses(accountBEDefinitionId, accounts, statusId, statusChangedDate, allowOverlapping, applicableOnStatuses, withCacheExpire, actionName);
        //}

        public void UpdateStatuses(Guid accountBEDefinitionId, List<Account> accounts, Guid statusId, DateTime statusChangedDate, bool allowOverlapping, List<Guid> applicableOnStatuses,out string errorMessage, bool withCacheExpire = true, string actionName = null)
        {
            errorMessage = null;
            if (accounts != null)
            {
                if(statusChangedDate.Date > DateTime.Today)
                {
                    errorMessage = "Date cannot be greater than today.";
                    return;
                }
                var accountIds = accounts.Select(x => x.AccountId).ToList();
                Dictionary<long, IOrderedEnumerable<AccountStatusHistory>> accountStatusesByAccountId = GetAccountStatusHistoryDicByAccountId(accountBEDefinitionId, accountIds, statusChangedDate);

                if (accountStatusesByAccountId != null)
                {
                    foreach(var account in accountStatusesByAccountId)
                    {
                        if (!CheckIfAllowStatusesOverlapping(account.Value, allowOverlapping,statusId, applicableOnStatuses, statusChangedDate, out errorMessage))
                            return; 
                    }
                }

                var accountStatusHistoryManager = new AccountStatusHistoryManager();
                FinancialAccountManager financialAccountManager = new Business.FinancialAccountManager();
                var status = s_statusDefinitionManager.GetStatusDefinition(statusId);
                status.ThrowIfNull("status", statusId);
                status.Settings.ThrowIfNull("status.Settings", statusId);
             
                VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
                VRAccountStatus vrInvoiceAccountStatus = status.Settings.IsInvoiceActive ? VRAccountStatus.Active : VRAccountStatus.InActive;
                VRAccountStatus vrBalanceAccountStatus = status.Settings.IsAccountBalanceActive ? VRAccountStatus.Active : VRAccountStatus.InActive;
                if (!status.Settings.IsActive)
                {
                    vrAccountStatus = VRAccountStatus.InActive;
                }
                string statusName = s_statusDefinitionManager.GetStatusDefinitionName(statusId);
                IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();

                foreach (var account in accounts)
                {
                 
                    IOrderedEnumerable<AccountStatusHistory> accountStatusHistories = null;
                    if (accountStatusesByAccountId != null)
                        accountStatusHistories = accountStatusesByAccountId.GetRecord(account.AccountId);

                    bool shouldUpdateAccount = false;
                    Guid? previousStatusId = null;
                    bool shouldInsertAccountHistory = true;
                    List<long> accountStatusHistoryIdsToDelete = null;

                    if (accountStatusHistories != null && accountStatusHistories.Count() > 0)
                    {
                        shouldUpdateAccount = true;
                        foreach (var accountStatusHistory in accountStatusHistories)
                        {
                            if (accountStatusHistoryIdsToDelete == null)
                            {
                                accountStatusHistoryIdsToDelete = new List<long>();
                                previousStatusId = accountStatusHistory.PreviousStatusId;
                                if (accountStatusHistory.PreviousStatusId == statusId)
                                    shouldInsertAccountHistory = false;
                            }
                            accountStatusHistoryIdsToDelete.Add(accountStatusHistory.AccountStatusHistoryId);
                        }
                    }
                    else
                    {
                        if (statusId != account.StatusId)
                        {
                            previousStatusId = account.StatusId;
                            shouldUpdateAccount = true;
                        }
                    }
                    if (shouldUpdateAccount)
                    {
                        string previousStatusName = null;
                        if (previousStatusId.HasValue)
                            previousStatusName = s_statusDefinitionManager.GetStatusDefinitionName(previousStatusId.Value);
                        
                        int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
                        bool updateStatus = dataManager.UpdateStatus(account.AccountId, statusId, lastModifiedBy);
                        if (updateStatus)
                        {
                            if (shouldInsertAccountHistory)
                                accountStatusHistoryManager.AddAccountStatusHistory(accountBEDefinitionId, account.AccountId, statusId, previousStatusId, statusChangedDate);
                            accountStatusHistoryManager.DeleteAccountStatusHistories(accountStatusHistoryIdsToDelete);
                            string actionDescription = null;
                            string dateFormat = Utilities.GetDateTimeFormat(DateTimeType.Date);
                            if (actionName != null)
                            {
                                actionDescription = string.Format("Effective on '{0}'", statusChangedDate.ToString(dateFormat));
                                VRActionLogger.Current.LogObjectCustomAction(new AccountBELoggableEntity(accountBEDefinitionId), actionName, true, account, actionDescription);
                            }
                            else
                            {
                                actionDescription = string.Format("Status Changed to '{0}' and effective on '{1}'", statusName, statusChangedDate.ToString(dateFormat));
                                if (previousStatusName != null)
                                {
                                    actionDescription = string.Format("Status Changed from '{0}' to '{1}' and effective on '{2}'", previousStatusName, statusName, statusChangedDate.ToString(dateFormat));
                                }
                                VRActionLogger.Current.LogObjectCustomAction(new AccountBELoggableEntity(accountBEDefinitionId), string.Format("Update Status"), true, account, actionDescription);
                            }
                        }
                        var financialAccountData = financialAccountManager.GetFinancialAccounts(accountBEDefinitionId, account.AccountId, false);
                        financialAccountManager.ReflectStatusToInvoiceAndBalanceAccounts(accountBEDefinitionId, vrAccountStatus, financialAccountData, vrInvoiceAccountStatus, vrBalanceAccountStatus);
                    }

                }
  
                if (withCacheExpire)
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
            }
        }
        private Dictionary<long, IOrderedEnumerable<AccountStatusHistory>> GetAccountStatusHistoryDicByAccountId(Guid accountBEDefinitionId, List<long> accountIds, DateTime statusChangedDate)
        {
            IEnumerable<AccountStatusHistory> accountStatusHistoryList = new AccountStatusHistoryManager().GetAccountStatusHistoriesAfterDate(accountBEDefinitionId, accountIds, statusChangedDate);
            if (accountStatusHistoryList == null)
                return null;

            Dictionary<long, List<AccountStatusHistory>> accountStatusHistoryDicByAccountId = new Dictionary<long, List<AccountStatusHistory>>();
            foreach (var accountStatusHistory in accountStatusHistoryList)
            {
                List<AccountStatusHistory> tempAccountStatusHistoryList = accountStatusHistoryDicByAccountId.GetOrCreateItem(accountStatusHistory.AccountId);
                tempAccountStatusHistoryList.Add(accountStatusHistory);
            }
            return accountStatusHistoryDicByAccountId.ToDictionary(itm => itm.Key, itm => itm.Value.OrderBy(historyItem => historyItem.StatusChangedDate));
        }
        private bool CheckIfAllowStatusesOverlapping(IOrderedEnumerable<AccountStatusHistory> accountStatusHistories, bool allowOverlapping,Guid statusId, List<Guid> applicableOnStatuses, DateTime statusChangedDate, out string errorMessage)
        {
            errorMessage = null;
            if (accountStatusHistories != null)
            {
                if (allowOverlapping)
                {
                    if (applicableOnStatuses != null && applicableOnStatuses.Count > 0)
                    {

                        foreach (var accountStatusHistory in accountStatusHistories)
                        {
                            if (accountStatusHistory.StatusId != statusId && !applicableOnStatuses.Contains(accountStatusHistory.StatusId))
                            {
                                errorMessage = string.Format("Account status is overlapped with not applicable status '{0}'.", s_statusDefinitionManager.GetStatusDefinitionName((accountStatusHistory.StatusId)));
                                return false;
                            }
                        }
                    }
                }
                else if (accountStatusHistories != null && accountStatusHistories.Count() > 0)
                {
                    errorMessage = string.Format("Account status is overlapped with other account statuses.");
                    return false;

                }
            }
            return true;
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
            if (accounts == null)
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
                throw new NullReferenceException(String.Format("Account of Id: '{0}' and AccountBEDefinitionId: '{1}'", account, accountBEDefinitionId));

            if (account.TypeId == accountTypeId)
                return account;

            if (account.ParentAccountId.HasValue)
                return GetSelfOrParentAccountOfType(accountBEDefinitionId, account.ParentAccountId.Value, accountTypeId);

            return null;
        }
        public Dictionary<long, Account> GetAccounts(Guid accountBEDefinitionId)
        {
            return GetCachedAccounts(accountBEDefinitionId);
        }
        public List<Account> GetChildAccounts(Account account, bool withSubChildren)
        {
            var accountBEDefinitionId = s_accountTypeManager.GetAccountBEDefinitionId(account.TypeId);
            return GetChildAccounts( accountBEDefinitionId, account.AccountId,  withSubChildren);
        }
        public List<Account> GetChildAccounts(Guid accountBEDefinitionId, long accountId, bool withSubChildren)
        {
            var accountTreeNode = GetCacheAccountTreeNodes(accountBEDefinitionId).GetRecord(accountId);
            accountTreeNode.ThrowIfNull("accountTreeNode", accountId);
            if (accountTreeNode.ChildNodes != null)
            {
                List<Account> childAccounts = new List<Account>();
                foreach (var childNode in accountTreeNode.ChildNodes)
                {
                    childAccounts.Add(childNode.Account);
                    if (withSubChildren)
                    {
                        var subChildren = GetChildAccounts(accountBEDefinitionId, childNode.Account.AccountId, withSubChildren);
                        if (subChildren != null)
                            childAccounts.AddRange(subChildren);
                    }
                }
                return childAccounts;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<ClientAccountInfo> GetClientChildAccountsInfo(Guid accountBEDefinitionId, long accountId, bool withSubChildren)
        {
            var childAccounts = GetChildAccounts(accountBEDefinitionId, accountId, withSubChildren);
            if (childAccounts == null)
                return null;
            return childAccounts.MapRecords(ClientChildAccountInfoMapper);
           
        }
        public List<long> GetChildAccountIds(Guid accountBEDefinitionId, long accountId, bool withSubChildren)
        {
            List<Account> accounts = GetChildAccounts(accountBEDefinitionId, accountId, withSubChildren);
            if (accounts == null)
                return null;

            return accounts.Select(itm => itm.AccountId).ToList();
        }

        public dynamic GetAccountGenericFieldValue(Guid accountBEDefinitionId, long accountId, string fieldName)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return GetAccountGenericFieldValue(accountBEDefinitionId, account, fieldName);
        }

        public dynamic GetAccountGenericFieldValue(Guid accountBEDefinitionId, Account account, string fieldName)
        {
            var accountGenericField = s_accountTypeManager.GetAccountGenericField(accountBEDefinitionId, fieldName);
            accountGenericField.ThrowIfNull("accountGenericField", fieldName);
            return accountGenericField.GetValue(new AccountGenericFieldContext(account));
        }

        public bool IsMobileOperator(Guid accountBEDefinitionId, long accountId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);

            if (account.Settings != null && account.Settings.Parts != null)
            {
                foreach (var part in account.Settings.Parts)
                {
                    IOperatorSetting operatorSetting = part.Value.Settings as IOperatorSetting;
                    if (operatorSetting != null)
                        return operatorSetting.IsMobileOperator;
                }
            }
            return false;
        }

        public VRLoggableEntityBase GetAccountLoggableEntity(Guid accountBEDefinitionId)
        {
            return new AccountBELoggableEntity(accountBEDefinitionId);
        }

        public bool TryGetCurrencyId(Guid accountDefinitionId, Account account, out int currencyId)
        {
            IAccountPayment paymentPart;
            if (HasAccountPayment(accountDefinitionId, true, account, out paymentPart))
            {
                currencyId = paymentPart.CurrencyId;
                return true;
            }
            else
            {
                currencyId = 0;
                return false;
            }
        }

        public bool TryGetCurrencyId(Guid accountDefinitionId, long accountId, out int currencyId)
        {
            Account account = GetAccount(accountDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return TryGetCurrencyId(accountDefinitionId, account, out currencyId);
        }

        public int GetCurrencyId(Guid accountDefinitionId, long accountId)
        {
            Account account = GetAccount(accountDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return GetCurrencyId(accountDefinitionId, account);
        }

        public int GetCurrencyId(Guid accountDefinitionId, Account account)
        {
            int currencyId;
            if (!TryGetCurrencyId(accountDefinitionId, account, out currencyId))
                throw new Exception(string.Format("Account {0} does not have currency", account.AccountId));
            return currencyId;
        }

        public bool IsAccountAssignableToPackage(Guid accountBEDefinitionId, long accountId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            return IsAccountAssignableToPackage(account);
        }
        public bool IsAccountAssignableToPackage(Account account)
        {
            account.ThrowIfNull("account");
            var accountDefinitionId = new AccountTypeManager().GetAccountBEDefinitionId(account.TypeId);
            var packageAssignmentCondition = new AccountBEDefinitionManager().GetPackageAssignmentCondition(accountDefinitionId);
            if (packageAssignmentCondition == null)
                return true;
            return EvaluateAccountCondition(account, packageAssignmentCondition);
        }


        #region Account Status

        public bool IsAccountActive(Guid accountBEDefinitionId, long accountId)
        {

            var account = GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return IsAccountActive(account);
        }
        public bool IsAccountActive(Account account)
        {
            var accountStatusDefinition = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinition(account.StatusId);
            accountStatusDefinition.ThrowIfNull("accountStatusDefinition", account.StatusId);
            accountStatusDefinition.Settings.ThrowIfNull("account.Settings");
            if (accountStatusDefinition.Settings.IsActive)
                return true;
            return false;
        }
        public bool IsAccountBalanceActive(Guid accountBEDefinitionId, long accountId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return IsAccountBalanceActive(account);
        }
        public bool IsAccountBalanceActive(Account account)
        {
            var accountStatusDefinition = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinition(account.StatusId);
            accountStatusDefinition.ThrowIfNull("accountStatusDefinition", account.StatusId);
            accountStatusDefinition.Settings.ThrowIfNull("account.Settings");
            if (accountStatusDefinition.Settings.IsAccountBalanceActive)
                return true;
            return false;
        }
        public bool IsAccountInvoiceActive(Guid accountBEDefinitionId, long accountId)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return IsAccountInvoiceActive(account);
        }
        public bool IsAccountInvoiceActive(Account account)
        {
            var accountStatusDefinition = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinition(account.StatusId);
            accountStatusDefinition.ThrowIfNull("accountStatusDefinition", account.StatusId);
            accountStatusDefinition.Settings.ThrowIfNull("account.Settings");
            if (accountStatusDefinition.Settings.IsInvoiceActive)
                return true;
            return false;
        }
        #endregion


        /// <summary>
        /// this method is called from the data transformation
        /// </summary>
        /// <param name="accountBEDefinitionId"></param>
        /// <param name="account1Id"></param>
        /// <param name="account2Id"></param>
        /// <returns></returns>
        public bool AreAccountsInLocalService(Guid accountBEDefinitionId, long account1Id, long account2Id)
        {
            var accountBEDefinitionSetting = _accountBEDefinitionManager.GetAccountBEDefinitionSettings(accountBEDefinitionId);
            accountBEDefinitionSetting.ThrowIfNull("accountBEDefinitionSetting", accountBEDefinitionId);
            if (!accountBEDefinitionSetting.LocalServiceAccountTypeId.HasValue)
                return false;

            if (account1Id == account2Id)
                return true;

            var account1Parent = GetSelfOrParentAccountOfType(accountBEDefinitionId, account1Id, accountBEDefinitionSetting.LocalServiceAccountTypeId.Value);
            var account2Parent = GetSelfOrParentAccountOfType(accountBEDefinitionId, account2Id, accountBEDefinitionSetting.LocalServiceAccountTypeId.Value);
            return account1Parent != null && account2Parent != null && account1Parent.AccountId == account2Parent.AccountId;
        }

        /// <summary>
        /// this method is called from the data transformation
        /// </summary>
        /// <param name="accountBEDefinitionId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Guid? GetAccountTypeId(Guid accountBEDefinitionId, long? accountId)
        {
            if (accountId.HasValue)
            {
                Account account = GetAccount(accountBEDefinitionId, accountId.Value);
                if (account != null)
                    return account.TypeId;
            }
            return null;
        }

        #endregion

        #region ExtendedSettings

        public bool DeleteAccountExtendedSetting<T>(Guid accountBEDefinitionId, long accountId) where T : BaseAccountExtendedSettings
        {
            return UpdateAccountExtendedSetting<T>(accountBEDefinitionId, accountId, null);
        }
        public bool UpdateAccountExtendedSetting<T>(Guid accountBEDefinitionId, long accountId, T extendedSettings) where T : BaseAccountExtendedSettings
        {
            Account account = GetAccount(accountBEDefinitionId, accountId);
            SetExtendedSettings<T>(extendedSettings, account);
            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            
            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            if (dataManager.UpdateExtendedSettings(accountId, account.ExtendedSettings, lastModifiedBy))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountBEDefinitionId);
                return true;
            }
            return false;
        }

        public void SetExtendedSettings<T>(T extendedSettings, Account account) where T : BaseAccountExtendedSettings
        {
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
        }
        public T GetExtendedSettings<T>(Guid accountBEDefinitionId, long accountId) where T : BaseAccountExtendedSettings
        {
            Account account = GetAccount(accountBEDefinitionId, accountId);
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
        public void TrackAndLogObjectCustomAction(Guid accountBEDefinitionId, long accountId, string action, string actionDescription, Object technicalInfo)
        {
            VRActionLogger.Current.LogObjectCustomAction(new AccountBELoggableEntity(accountBEDefinitionId), action, true, GetAccount(accountBEDefinitionId, accountId), actionDescription, technicalInfo);
        }
        #endregion

        #region Private Methods

        private class CachedAccountsByDefId
        {
            public Dictionary<long, Account> AllAccounts { get; set; }

            public byte[] LastTimeStamp { get; set; }
        }

        static Dictionary<Guid, CachedAccountsByDefId> s_allAccountsByDefId = new Dictionary<Guid,CachedAccountsByDefId>();

        public Dictionary<long, Account> GetCachedAccounts(Guid accountBEDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccounts", accountBEDefinitionId,
                () =>
                {
                    CachedAccountsByDefId cachedAccountsByDefId;
                    lock(s_allAccountsByDefId)
                    {
                        cachedAccountsByDefId = s_allAccountsByDefId.GetOrCreateItem(accountBEDefinitionId);
                    }
                    IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
                    List<Guid> accountTypeIds = s_accountTypeManager.GetAccountTypeIds(accountBEDefinitionId);
                    byte[] newMaxTimeStamp = dataManager.GetMaxTimeStamp(accountTypeIds);
                    IEnumerable<Account> accounts = dataManager.GetAccounts(accountTypeIds, cachedAccountsByDefId.LastTimeStamp);
                    if (cachedAccountsByDefId.AllAccounts == null)
                    {
                        cachedAccountsByDefId.AllAccounts = new Dictionary<long, Account>(accounts.Count());
                        foreach(var a in accounts)
                        {
                            cachedAccountsByDefId.AllAccounts.Add(a.AccountId, a);
                        }
                    }
                    else
                    {
                        Dictionary<long, Account> finalAccounts = new Dictionary<long, Account>(cachedAccountsByDefId.AllAccounts.Count + accounts.Count());
                        foreach(var entry in cachedAccountsByDefId.AllAccounts)
                        {
                            finalAccounts.Add(entry.Key, entry.Value);
                        }
                        foreach(var a in accounts)
                        {
                            if (finalAccounts.ContainsKey(a.AccountId))
                            {
                                finalAccounts[a.AccountId] = a;
                            }
                            else
                            {
                                finalAccounts.Add(a.AccountId, a);
                            }
                        }
                        cachedAccountsByDefId.AllAccounts = finalAccounts;
                    }
                    cachedAccountsByDefId.LastTimeStamp = newMaxTimeStamp;
                    return cachedAccountsByDefId.AllAccounts;
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
        internal Dictionary<long, AccountTreeNode> GetCacheAccountTreeNodes(Guid accountBEDefinitionId)
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
        internal bool TryUpdateAccount(AccountToEdit accountToEdit, out Account account)
        {
            ValidateAccountToEdit(accountToEdit);
            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();

            accountToEdit.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            if (dataManager.Update(accountToEdit))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountToEdit.AccountBEDefinitionId);
                account = GetAccount(accountToEdit.AccountBEDefinitionId, accountToEdit.AccountId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(new AccountBELoggableEntity(accountToEdit.AccountBEDefinitionId), account);
                return true;
            }
            else
            {
                account = null;
                return false;
            }
        }
        internal bool TryAddAccount(AccountToInsert accountToInsert, out long accountId, bool donotValidateParent, out Account account)
        {
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

            ValidateAccountToAdd(accountToInsert, donotValidateParent);

            IAccountBEDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
          
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            accountToInsert.CreatedBy = loggedInUserId;
            accountToInsert.LastModifiedBy = loggedInUserId;

            if (dataManager.Insert(accountToInsert, out accountId))
            {
                new AccountStatusHistoryManager().AddAccountStatusHistory(accountToInsert.AccountBEDefinitionId, accountId, accountToInsert.StatusId, null,DateTime.Today);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(accountToInsert.AccountBEDefinitionId);
                account = GetAccount(accountToInsert.AccountBEDefinitionId, accountId);
                VRActionLogger.Current.TrackAndLogObjectAdded(new AccountBELoggableEntity(accountToInsert.AccountBEDefinitionId), account);
                return true;
            }
            else
            {
                account = null;
                return false;
            }
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IAccountBEDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountBEDataManager>();
            AccountTypeManager _accountTypeManager = new AccountTypeManager();
            ConcurrentDictionary<Guid, Object> _updateHandlesByBEDefinitionId = new ConcurrentDictionary<Guid, Object>();
            protected override bool ShouldSetCacheExpired(Guid accountBEDefinitionId)
            {
                object _updateHandle;

                _updateHandlesByBEDefinitionId.TryGetValue(accountBEDefinitionId, out _updateHandle);
                bool isCacheExpired = _dataManager.AreAccountsUpdated(_accountTypeManager.GetAccountTypeIds(accountBEDefinitionId), ref _updateHandle);
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
        private class AccountClientExcelExportHandler : ExcelExportHandler<AccountClientDetail>
        {
            private AccountQuery _query;
            public AccountClientExcelExportHandler(AccountQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountClientDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                var fieldDefinitions = new AccountTypeManager().GetGenericFieldDefinitionsInfo(_query.AccountBEDefinitionId);
                if (fieldDefinitions != null && _query.Columns  != null)
                {
                    foreach (var exportColumn in _query.Columns)
                    {
                        var fieldDefinition = fieldDefinitions.FindRecord(x=>x.Name == exportColumn);
                        if (fieldDefinition != null)
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = fieldDefinition.Title });
                    }
                    sheet.Rows = new List<ExportExcelRow>();

                    if (context.BigResult != null && context.BigResult.Data != null)
                    {
                        foreach (AccountClientDetail accountDetail in context.BigResult.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);

                            foreach (var exportColumn in _query.Columns)
                            {
                                DataRecordFieldValue dataRecordFieldValue;
                                if (accountDetail.FieldValues.TryGetValue(exportColumn, out dataRecordFieldValue))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = dataRecordFieldValue.Description });
                                }
                                else
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                                }
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class AccountExcelExportHandler : ExcelExportHandler<AccountDetail>
        {
            private AccountQuery _query;
            public AccountExcelExportHandler(AccountQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                var accountBEDefinitionSettings = new AccountBEDefinitionManager().GetAccountBEDefinitionSettings(_query.AccountBEDefinitionId);
                if (accountBEDefinitionSettings.GridDefinition.ExportColumnDefinitions != null)
                {
                    foreach (AccountGridExportColumnDefinition exportColumn in accountBEDefinitionSettings.GridDefinition.ExportColumnDefinitions)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = exportColumn.Header });
                    }
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Account Type" });
                    sheet.Rows = new List<ExportExcelRow>();

                    if (context.BigResult != null && context.BigResult.Data != null)
                    {
                        foreach (AccountDetail accountDetail in context.BigResult.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);

                            foreach (AccountGridExportColumnDefinition exportColumn in accountBEDefinitionSettings.GridDefinition.ExportColumnDefinitions)
                            {
                                DataRecordFieldValue dataRecordFieldValue;
                                if (accountDetail.FieldValues.TryGetValue(exportColumn.FieldName, out dataRecordFieldValue))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = dataRecordFieldValue.Description });
                                }
                                else
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                                }
                            }
                            row.Cells.Add(new ExportExcelCell { Value = accountDetail.AccountTypeTitle });
                        }
                    }
                }
                else
                {
                    foreach (AccountGridColumnDefinition exportColumn in accountBEDefinitionSettings.GridDefinition.ColumnDefinitions)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = exportColumn.Header });
                    }
                    sheet.Rows = new List<ExportExcelRow>();

                    if (context.BigResult != null && context.BigResult.Data != null)
                    {
                        foreach (AccountDetail accountDetail in context.BigResult.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);

                            foreach (AccountGridColumnDefinition exportColumn in accountBEDefinitionSettings.GridDefinition.ColumnDefinitions)
                            {
                                DataRecordFieldValue dataRecordFieldValue;
                                if (accountDetail.FieldValues.TryGetValue(exportColumn.FieldName, out dataRecordFieldValue))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = dataRecordFieldValue.Description });
                                }
                                else
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                                }
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        public class AccountBELoggableEntity : VRLoggableEntityBase
        {
            Guid _accountBEDefinitionId;
            static AccountBEDefinitionManager s_accountBEDefinitionManager = new AccountBEDefinitionManager();
            static AccountBEManager s_accountBEManager = new AccountBEManager();

            public AccountBELoggableEntity(Guid accountBEDefinitionId)
            {
                _accountBEDefinitionId = accountBEDefinitionId;
            }

            public override string EntityUniqueName
            {
                get { return String.Format("Retail_BusinessEntity_AccountBE_{0}", _accountBEDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get { return s_accountBEDefinitionManager.GetAccountBEDefinitionName(_accountBEDefinitionId); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Retail_BusinessEntity_AccountBE_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Account account = context.Object.CastWithValidate<Account>("context.Object");
                return account.AccountId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Account account = context.Object.CastWithValidate<Account>("context.Object");
                return s_accountBEManager.GetAccountName(_accountBEDefinitionId, account.AccountId);
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }
        }

        private class AccountPartDefinitionIsPartValidContext : IAccountPartDefinitionIsPartValidContext
        {
            public AccountPartSettings AccountPartSettings
            {
                get;
                set;
            }

            public AccountPartSettings ExistingAccountPartSettings
            {
                get;
                set;
            }

            public string ErrorMessage
            {
                get;
                set;
            }
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
            ValidateAccount(accountToInsert, accountToInsert.AccountBEDefinitionId, accountToInsert.StatusId, accountToInsert.ParentAccountId, donotValidateParent, null);
        }

        private void ValidateAccountToEdit(AccountToEdit accountToEdit)
        {
            Account accountEntity = this.GetAccount(accountToEdit.AccountBEDefinitionId, accountToEdit.AccountId);

            if (accountEntity == null)
                throw new DataIntegrityValidationException(String.Format("Account '{0}' does not exist", accountToEdit.AccountId));

            ValidateAccount(accountToEdit, accountToEdit.AccountBEDefinitionId, null, null, false, accountEntity);

        }

        private void ValidateAccount(BaseAccount account, Guid accountBEDefinitionId, Guid? statusDefinitionId, long? parentAccountId, bool donotValidateParent, Account existingAccount)
        {
            if (String.IsNullOrWhiteSpace(account.Name))
                throw new MissingArgumentValidationException("account.Name");

            if (statusDefinitionId.HasValue)
            {
                var statusDefinition = s_statusDefinitionManager.GetStatusDefinition(statusDefinitionId.Value);
                statusDefinition.ThrowIfNull("statusDefinition", statusDefinitionId.Value);
            }

            var accountType = new AccountTypeManager().GetAccountType(account.TypeId);
            accountType.ThrowIfNull("accountType", account.TypeId);
            if (accountType.AccountBEDefinitionId != accountBEDefinitionId)
                throw new DataIntegrityValidationException(string.Format("accountType.AccountBEDefinitionId '{0}' is different than passed accountBEDefinitionId '{1}'", accountType.AccountBEDefinitionId, accountBEDefinitionId));

            if (account.Settings != null && account.Settings.Parts != null && account.Settings.Parts.Count > 0)
            {
                accountType.Settings.ThrowIfNull("accountType.Settings", accountType.AccountTypeId);
                accountType.Settings.PartDefinitionSettings.ThrowIfNull("accountType.Settings.PartDefinitionSettings", accountType.AccountTypeId);
                foreach (var partEntry in account.Settings.Parts)
                {
                    Guid partDefinitionId = partEntry.Key;
                    AccountPart part = partEntry.Value;
                    part.ThrowIfNull("part", partDefinitionId);
                    part.Settings.ThrowIfNull("part.Settings", partDefinitionId);
                    if (!accountType.Settings.PartDefinitionSettings.Any(itm => itm.PartDefinitionId == partDefinitionId))
                        throw new Exception(String.Format("Part Definition Id '{0}' is not available in AccountType", partDefinitionId));
                    var partDefinition = s_partDefinitionManager.GetAccountPartDefinition(partDefinitionId);
                    partDefinition.ThrowIfNull("partDefinition", partDefinitionId);
                    partDefinition.Settings.ThrowIfNull("partDefinition.Settings", partDefinitionId);

                    AccountPart existingAccountPart = null;
                    if (existingAccount != null && existingAccount.Settings != null && existingAccount.Settings.Parts != null)
                        existingAccount.Settings.Parts.TryGetValue(partDefinitionId, out existingAccountPart);

                    var isPartValidContext = new AccountPartDefinitionIsPartValidContext
                    {
                        AccountPartSettings = part.Settings,
                        ExistingAccountPartSettings = existingAccountPart != null ? existingAccountPart.Settings : null
                    };
                    if (!partDefinition.Settings.IsPartValid(isPartValidContext))
                        throw new Exception(String.Format("Part '{0}' error: {1}", partDefinition.Name, isPartValidContext.ErrorMessage));
                }
            }
            if (accountType.Settings != null && accountType.Settings.PartDefinitionSettings != null && accountType.Settings.PartDefinitionSettings.Count > 0)
            {
                account.Settings.ThrowIfNull("account.Settings");
                account.Settings.Parts.ThrowIfNull("account.Settings.Parts");
                foreach (var accountTypePart in accountType.Settings.PartDefinitionSettings)
                {
                    if (accountTypePart.RequiredSettings == AccountPartRequiredOptions.Required)
                    {
                        if (!account.Settings.Parts.ContainsKey(accountTypePart.PartDefinitionId))
                            throw new Exception(String.Format("Part '{0}' is not supplied", accountTypePart.PartDefinitionId));
                    }
                }
            }

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
            var filtredActionIds = _accountBEDefinitionManager.GetLoggedInUserAllowedActionIds(accountType.AccountBEDefinitionId);
            var filtredViewIds = _accountBEDefinitionManager.GetLoggedInUserAllowedViewIds(accountType.AccountBEDefinitionId);
            AccountDetailMapperStep2(accountType.AccountBEDefinitionId, accountDetail, account, filtredActionIds, filtredViewIds);
            return accountDetail;
        }
        private AccountDetail AccountDetailMapperStep1(Guid accountBEDefinitionId, Account account, List<string> columns)
        {
            var statusDefinitionManager = new Vanrise.Common.Business.StatusDefinitionManager();
            var accountTypeManager = new AccountTypeManager();
            var accountServices = new AccountServiceManager();
            var accountPackages = new AccountPackageManager();

            var accountTreeNode = GetCacheAccountTreeNodes(accountBEDefinitionId).GetRecord(account.AccountId);

            //Dynamic Part
            Dictionary<string, DataRecordFieldValue> fieldValues = new Dictionary<string, DataRecordFieldValue>();
            Dictionary<string, AccountGenericField> accountGenericFields = accountTypeManager.GetAccountGenericFields(accountBEDefinitionId);

            if(columns != null )
            {
                foreach(var col in columns)
                {
                    AccountGenericField field;
                    if (!accountGenericFields.TryGetValue(col, out field))
                        throw new NullReferenceException(String.Format("field '{0}'. accountBEDefinitionId '{1}'", col, accountBEDefinitionId));
                    object value = field.GetValue(new AccountGenericFieldContext(account));

                    DataRecordFieldValue dataRecordFieldValue = new DataRecordFieldValue();
                    dataRecordFieldValue.Value = value;
                    if (value != null)
                        dataRecordFieldValue.Description = field.FieldType.GetDescription(value);

                    fieldValues.Add(field.Name, dataRecordFieldValue);
                }
            }
            else
            {
                foreach (var field in accountGenericFields.Values)
                {
                    if (columns != null && !columns.Contains(field.Name))
                        continue;

                    object value = field.GetValue(new AccountGenericFieldContext(account));

                    DataRecordFieldValue dataRecordFieldValue = new DataRecordFieldValue();
                    dataRecordFieldValue.Value = value;
                    if (value != null)
                        dataRecordFieldValue.Description = field.FieldType.GetDescription(value);

                    fieldValues.Add(field.Name, dataRecordFieldValue);
                }
            }

            

            return new AccountDetail()
            {
                AccountId = account.AccountId,
                AccountTypeTitle = accountTypeManager.GetAccountTypeName(account.TypeId),
                DirectSubAccountCount = accountTreeNode.ChildNodes.Count,
                TotalSubAccountCount = accountTreeNode.TotalSubAccountsCount,
                StatusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId),
                FieldValues = fieldValues
            };
        }
        private void AccountDetailMapperStep2(Guid accountBEDefinitionId, AccountDetail accountDetail, Account account, HashSet<Guid> ActionDefinitionIds, HashSet<Guid> ViewDefinitionIds)
        {
            IEnumerable<AccountTypeInfo> accountTypeInfoEntities = new AccountTypeManager().GetAccountTypesInfo(new AccountTypeFilter() { ParentAccountId = account.AccountId });

            var accountBEDefinitionManager = new AccountBEDefinitionManager();
            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionManager.GetAccountViewDefinitionsByAccount(accountBEDefinitionId, account);
            List<AccountActionDefinition> accountActionDefinitions = accountBEDefinitionManager.GetAccountActionDefinitionsByAccount(accountBEDefinitionId, account);

            accountDetail.CanAddSubAccounts = (accountTypeInfoEntities != null && accountTypeInfoEntities.Count() > 0);

            accountDetail.AvailableAccountViews = accountViewDefinitions != null ? accountViewDefinitions.Where(x => ViewDefinitionIds.Contains(x.AccountViewDefinitionId)).Select(itm => itm.AccountViewDefinitionId).ToList() : null;

            accountDetail.AvailableAccountActions = accountActionDefinitions != null ? accountActionDefinitions.Where(x => ActionDefinitionIds.Contains(x.AccountActionDefinitionId)).Select(itm => itm.AccountActionDefinitionId).ToList() : null;
            accountDetail.Style = GetStatuStyle(account.StatusId);
        }
        private void AccountDetailMapperStep2(Guid accountBEDefinitionId, AccountDetail accountDetail, long accountId, HashSet<Guid> ActionDefinitionIds, HashSet<Guid> ViewDefinitionIds)
        {
            Account account = this.GetAccount(accountBEDefinitionId, accountId);
            AccountDetailMapperStep2(accountBEDefinitionId, accountDetail, account, ActionDefinitionIds, ViewDefinitionIds);
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
                Name = GetAccountName(account)
            };
        }

        private string BuildAccountFullName(Guid accountBEDefinitionId, long accountId, string name)
        {
            var account = GetAccount(accountBEDefinitionId, accountId);
            if (account == null)
                return null;
            name = string.Format("{0} -> {1}", account.Name, name);
            if (account.ParentAccountId.HasValue)
            {
                return BuildAccountFullName(accountBEDefinitionId, account.ParentAccountId.Value, name);
            }
            return name;
        }
        private ClientAccountInfo ClientChildAccountInfoMapper(Account account)
        {
            return new ClientAccountInfo
            {
                AccountId = account.AccountId,
                Name = GetAccountName(account),
                TypeId = account.TypeId
            };
        }
        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedAccounts = GetCachedAccounts(context.EntityDefinitionId);
            if (cachedAccounts != null)
                return cachedAccounts.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }
        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetAccount(context.EntityDefinitionId, context.EntityId);
        }
        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetAccountName(context.EntityDefinition.BusinessEntityDefinitionId, Convert.ToInt64(context.EntityId));
        }
        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var account = context.Entity as Account;
            return account.AccountId;
        }
        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(context.EntityDefinitionId, ref lastCheckTime);
        }
        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            switch (context.InfoType)
            {
                case Vanrise.AccountBalance.Entities.AccountInfo.BEInfoType:
                    {
                        var account = context.Entity as Account;
                        Vanrise.Common.Business.StatusDefinitionManager statusDefinitionManager = new Vanrise.Common.Business.StatusDefinitionManager();
                        var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId);
                        Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
                        {
                            Name = account.Name,
                            StatusDescription = statusDesciption,
                            CurrencyId = GetCurrencyId(context.EntityDefinitionId, account)
                        };
                        return accountInfo;
                    }
                default: return null;
            }
        }

        #endregion
    }

    internal class AccountTreeNode
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
}