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
            AccountServiceDetail accountServiceDetail = new AccountServiceDetail();
            AccountManager accountManager = new AccountManager();
            ServiceTypeManager serviceTypeManager = new Business.ServiceTypeManager();
            ChargingPolicyManager chargingPolicyManager = new ChargingPolicyManager();
            accountServiceDetail.Entity = accountService;
            accountServiceDetail.AccountName = accountManager.GetAccountName(accountService.AccountId);
            accountServiceDetail.ServiceChargingPolicyName = chargingPolicyManager.GetChargingPolicyName(accountService.ServiceChargingPolicyId);
            accountServiceDetail.ServiceTypeTitle = serviceTypeManager.GetServiceTypeName(accountService.ServiceTypeId);
            return accountServiceDetail;
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
