using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceManager
    {
        #region ctor/Local Variables

        #endregion


        #region Public Methods

        public IDataRetrievalResult<AccountServiceDetail> GetFilteredAccountServices(Vanrise.Entities.DataRetrievalInput<AccountServiceQuery> input)
        {
            var allAccountServices = GetCachedAccountServices();

            Func<AccountService, bool> filterExpression = (prod) =>
                 (input.Query.AccountId == null || prod.AccountId == input.Query.AccountId);

            return DataRetrievalManager.Instance.ProcessResult(input, allAccountServices.ToBigResult(input, filterExpression,
                       (accountService) => AccountServiceDetailMapper(input.Query.AccountBEDefinitionId, accountService)));
        }

        public AccountService GetAccountService(long AccountServiceId)
        {
            var AccountServices = GetCachedAccountServices();
            return AccountServices.GetRecord(AccountServiceId);
        }
        public AccountService GetAccountService(long accountId, Guid serviceTypeId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo != null)
                return accountInfo.AccountServices.FindRecord(itm => itm.ServiceTypeId == serviceTypeId);
            else
                return null;
        }
        public AccountServiceDetail GetAccountServiceDetail(Guid accountBEDefinition, long accountServiceId)
        {
            var accountService = GetAccountService(accountServiceId);
            return accountService != null ? AccountServiceDetailMapper(accountBEDefinition, accountService) : null;
        }
        public int GetAccountServicesCount(long accountId)
        {
            var accountInfo = GetAccountInfo(accountId);
            if (accountInfo != null)
                return accountInfo.AccountServices.Count;
            else
                return 0;
        }

        public bool UpdateStatus(long accountServiceId, Guid statusId)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool updateStatus = dataManager.UpdateStatus(accountServiceId, statusId);
            if (updateStatus)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateStatus;
        }
        public bool UpdateExecutedActions(long accountServiceId, ExecutedActions executedActions)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool updateExecutedAction = dataManager.UpdateExecutedActions(accountServiceId, executedActions);
            if (updateExecutedAction)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return updateExecutedAction;
        }
        public InsertOperationOutput<AccountServiceDetail> AddAccountService(AccountServiceToAdd accountServiceToAdd)
        {
            InsertOperationOutput<AccountServiceDetail> insertOperationOutput = new InsertOperationOutput<AccountServiceDetail>();
            insertOperationOutput.Result =InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long AccountServiceId = -1;

            var serviceTypeManager = new ServiceTypeManager();
            var serviceType = serviceTypeManager.GetServiceType(accountServiceToAdd.ServiceTypeId);
            if (serviceType == null)
                throw new NullReferenceException("ServiceType is null");
            if (serviceType.Settings == null)
                throw new NullReferenceException("ServiceType settings is null");

            accountServiceToAdd.StatusId = serviceType.Settings.InitialStatusId;

            bool insertActionSucc = AddAccountService(accountServiceToAdd, out AccountServiceId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result =InsertOperationResult.Succeeded;
                accountServiceToAdd.AccountServiceId = AccountServiceId;
                insertOperationOutput.InsertedObject = AccountServiceDetailMapper(accountServiceToAdd.AccountBEDefinitionId, accountServiceToAdd);
            }
            else
                insertOperationOutput.Result =InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public bool AddAccountService(AccountService accountService, out long accountServiceId)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            return dataManager.Insert(accountService, out accountServiceId);
        }

        public UpdateOperationOutput<AccountServiceDetail> UpdateAccountService(AccountServiceToEdit accountServiceToEdit)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool updateActionSucc = UpdateAccountServiceEntity(accountServiceToEdit);
            UpdateOperationOutput<AccountServiceDetail> updateOperationOutput = new UpdateOperationOutput<AccountServiceDetail>();

            updateOperationOutput.Result =UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result =UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountServiceDetailMapper(accountServiceToEdit.AccountBEDefinitionId, this.GetAccountService(accountServiceToEdit.AccountServiceId));
            }
            else
                updateOperationOutput.Result =UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        public bool UpdateAccountServiceEntity(AccountService accountService)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            return dataManager.Update(accountService);
        }


        public bool IsAccountServiceMatchWithFilterGroup(Account account, AccountService accountService, RecordFilterGroup filterGroup)
        {
            return new Vanrise.GenericData.Business.RecordFilterManager().IsFilterGroupMatch(filterGroup, new AccountServiceRecordFilterGenericFieldMatchContext(account, accountService));
        }

        #endregion


        #region Private Members

        private Dictionary<long, AccountService> GetCachedAccountServices()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountServices",
               () =>
               {
                   IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
                   IEnumerable<AccountService> AccountServices = dataManager.GetAccountServices();
                   return AccountServices.ToDictionary(cn => cn.AccountServiceId, cn => cn);
               });
        }

        private AccountInfo GetAccountInfo(long accountId)
        {
            return GetCachedAccountInfoByAccountId().GetRecord(accountId);
        }

        private Dictionary<long, AccountInfo> GetCachedAccountInfoByAccountId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountInfoByAccountId",
              () =>
              {
                  Dictionary<long, AccountInfo> accountInfos = new Dictionary<long, AccountInfo>();
                  foreach (var accountService in GetCachedAccountServices().Values)
                  {
                      accountInfos.GetOrCreateItem(accountService.AccountId).AccountServices.Add(accountService);
                  }
                  return accountInfos;
              });
        }

        #endregion


        #region  Mappers

        private AccountServiceDetail AccountServiceDetailMapper(Guid accountBEDefinitionId, AccountService accountService)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            ServiceTypeManager serviceTypeManager = new Business.ServiceTypeManager();
            ChargingPolicyManager chargingPolicyManager = new ChargingPolicyManager();
            StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();

            ActionDefinitionManager manager = new ActionDefinitionManager();
            IEnumerable<ActionDefinitionInfo> actionDefinitions = manager.GetActionDefinitionInfoByEntityType(EntityType.AccountService, accountService.StatusId, accountService.ServiceTypeId);
            var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(accountService.StatusId);


            return new AccountServiceDetail()
           {
               Entity = accountService,
               AccountName = accountBEManager.GetAccountName(accountBEDefinitionId, accountService.AccountId),
               ServiceChargingPolicyName = accountService.ServiceChargingPolicyId.HasValue ? chargingPolicyManager.GetChargingPolicyName(accountService.ServiceChargingPolicyId.Value) : null,
               ServiceTypeTitle = serviceTypeManager.GetServiceTypeName(accountService.ServiceTypeId),
               ActionDefinitions = actionDefinitions,
               StatusDesciption = statusDesciption,
               Style = GetStatuStyle(accountService.StatusId),
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
        private string GetStatusColor(string statusDesciption)
        {
            switch (statusDesciption)
            {
                case "Active": return "label label-success";
                case "Suspended": return "label label-warning";
                case "Terminated": return "label label-danger";
                case "Blocked": return "label label-danger";
                default: return "label label-primary";
            }
        }

        #endregion


        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountServicesUpdated(ref _updateHandle);
            }
        }

        private class AccountServiceRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            Account _account;
            AccountService _accountService;
            ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();

            public AccountServiceRecordFilterGenericFieldMatchContext(Account account, AccountService accountService)
            {
                _account = account;
                _accountService = accountService;
            }

            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                var accountServiceGenericField = _serviceTypeManager.GetAccountServiceGenericField(_accountService.ServiceTypeId, fieldName);
                if (accountServiceGenericField == null)
                    throw new NullReferenceException(String.Format("accountServiceGenericField '{0}'", fieldName));
                fieldType = accountServiceGenericField.FieldType;
                return accountServiceGenericField.GetValue(new AccountServiceGenericFieldContext(_account, _accountService));
            }
        }

        private class AccountInfo
        {
            List<AccountService> _accountServices = new List<AccountService>();
            public List<AccountService> AccountServices
            {
                get
                {
                    return _accountServices;
                }
            }
        }

        #endregion
    }
}
