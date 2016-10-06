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

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceManager
    {
        #region ctor/Local Variables

        #endregion


        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountServiceDetail> GetFilteredAccountServices(Vanrise.Entities.DataRetrievalInput<AccountServiceQuery> input)
        {
            var allAccountServices = GetCachedAccountServices();

            Func<AccountService, bool> filterExpression = (prod) =>
                 (input.Query.AccountId == null || prod.AccountId == input.Query.AccountId);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccountServices.ToBigResult(input, filterExpression, AccountServiceDetailMapper));
        }

        public AccountService GetAccountService(long AccountServiceId)
        {
            var AccountServices = GetCachedAccountServices();
            return AccountServices.GetRecord(AccountServiceId);
        }
        public AccountService GetAccountService(long accountId, Guid serviceTypeId)
        {
            var accountServices = GetCachedAccountServices();
            if (accountServices != null)
                return accountServices.FindRecord(itm => itm.AccountId == accountId && itm.ServiceTypeId == serviceTypeId);
            else
                return null;
        }
        public AccountServiceDetail GetAccountServiceDetail(long AccountServiceId)
        {
            var accountServices = GetCachedAccountServices();
            if (accountServices != null)
                return accountServices.MapRecord(AccountServiceDetailMapper, itm => itm.AccountServiceId == AccountServiceId);
            else
                return null;
        }
        public int GetAccountServicesCount(long accountId)
        {
            Dictionary<long , AccountService> accountServicesCount = GetCachedAccountServices();
            if (accountServicesCount != null)
                return accountServicesCount.Where(x => x.Value.AccountId == accountId).Count();
            else
                return -1;
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
        public InsertOperationOutput<AccountServiceDetail> AddAccountService(AccountService accountService)
        {
            InsertOperationOutput<AccountServiceDetail> insertOperationOutput = new InsertOperationOutput<AccountServiceDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long AccountServiceId = -1;

            var serviceTypeManager = new ServiceTypeManager();
            var serviceType = serviceTypeManager.GetServiceType(accountService.ServiceTypeId);
            if (serviceType == null)
                throw new NullReferenceException("ServiceType is null");
            if (serviceType.Settings == null)
                throw new NullReferenceException("ServiceType settings is null");

            accountService.StatusId = serviceType.Settings.InitialStatusId;

            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool insertActionSucc = dataManager.Insert(accountService, out AccountServiceId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                accountService.AccountServiceId = AccountServiceId;
                insertOperationOutput.InsertedObject = AccountServiceDetailMapper(accountService);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public UpdateOperationOutput<AccountServiceDetail> UpdateAccountService(AccountService accountService)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool updateActionSucc = dataManager.Update(accountService);
            UpdateOperationOutput<AccountServiceDetail> updateOperationOutput = new UpdateOperationOutput<AccountServiceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountServiceDetailMapper(this.GetAccountService(accountService.AccountServiceId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
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

        #endregion


        #region  Mappers

        private AccountServiceDetail AccountServiceDetailMapper(AccountService accountService)
        {
            AccountManager accountManager = new AccountManager();
            ServiceTypeManager serviceTypeManager = new Business.ServiceTypeManager();
            ChargingPolicyManager chargingPolicyManager = new ChargingPolicyManager();
            StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();
        
            ActionDefinitionManager manager = new ActionDefinitionManager();
            IEnumerable<ActionDefinitionInfo> actionDefinitions = manager.GetActionDefinitionInfoByEntityType(EntityType.AccountService, accountService.StatusId, accountService.ServiceTypeId);
             var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(accountService.StatusId);


             return new AccountServiceDetail()
            {
                Entity = accountService,
                AccountName = accountManager.GetAccountName(accountService.AccountId),
                ServiceChargingPolicyName = chargingPolicyManager.GetChargingPolicyName(accountService.ServiceChargingPolicyId),
                ServiceTypeTitle = serviceTypeManager.GetServiceTypeName(accountService.ServiceTypeId),
                ActionDefinitions = actionDefinitions,
                StatusDesciption =statusDesciption,
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

        #endregion
    }
}
