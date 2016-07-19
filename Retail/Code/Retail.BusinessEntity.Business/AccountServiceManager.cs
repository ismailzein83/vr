using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
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

        public AccountService GetAccountService(long accountId, int serviceTypeId)
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
        public InsertOperationOutput<AccountServiceDetail> AddAccountService(AccountService AccountService)
        {
            InsertOperationOutput<AccountServiceDetail> insertOperationOutput = new InsertOperationOutput<AccountServiceDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long AccountServiceId = -1;
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();
            bool insertActionSucc = dataManager.Insert(AccountService, out AccountServiceId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                AccountService.AccountServiceId = AccountServiceId;
                insertOperationOutput.InsertedObject = AccountServiceDetailMapper(AccountService);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public UpdateOperationOutput<AccountServiceDetail> UpdateAccountService(AccountService AccountService)
        {
            IAccountServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountServiceDataManager>();

            bool updateActionSucc = dataManager.Update(AccountService);
            UpdateOperationOutput<AccountServiceDetail> updateOperationOutput = new UpdateOperationOutput<AccountServiceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountServiceDetailMapper(AccountService);
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
            IEnumerable<ActionDefinitionInfo> actionDefinitions = manager.GetActionDefinitionInfoByEntityType(EntityType.AccountService,accountService.StatusId);
             var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(accountService.StatusId);


             return new AccountServiceDetail()
            {
                Entity = accountService,
                AccountName = accountManager.GetAccountName(accountService.AccountId),
                ServiceChargingPolicyName = chargingPolicyManager.GetChargingPolicyName(accountService.ServiceChargingPolicyId),
                ServiceTypeTitle = serviceTypeManager.GetServiceTypeName(accountService.ServiceTypeId),
                ActionDefinitions = actionDefinitions,
                StatusDesciption =statusDesciption,
                StatusColor = GetStatusColor(statusDesciption)
            };
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
