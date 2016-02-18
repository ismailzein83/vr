using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Demo.Module.Business
{
    public class OperatorAccountManager
    {

        #region ctor/Local Variables
        OperatorProfileManager _operatorProfileManager;
        public OperatorAccountManager()
        {
            _operatorProfileManager = new OperatorProfileManager();
        }

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorAccountDetail> GetFilteredOperatorAccounts(Vanrise.Entities.DataRetrievalInput<OperatorAccountQuery> input)
        {
            var allOperatorAccounts = GetCachedOperatorAccounts();

            Func<OperatorAccount, bool> filterExpression = (item) =>
                 (input.Query.Name == null || item.NameSuffix.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.OperatorProfilesIds == null || input.Query.OperatorProfilesIds.Contains(item.OperatorProfileId))
                  &&
                 (input.Query.OperatorAccountsIds == null || input.Query.OperatorAccountsIds.Contains(item.OperatorAccountId));
               

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorAccounts.ToBigResult(input, filterExpression, OperatorAccountDetailMapper));
        }

        public IEnumerable<OperatorAccountInfo> GetOperatorAccountsInfo()
        {
            var OperatorAccounts = GetCachedOperatorAccounts();
            return OperatorAccounts.MapRecords(OperatorAccountInfoMapper);
        }

        public OperatorAccount GetOperatorAccount(int operatorAccountId)
        {
            var OperatorAccounts = GetCachedOperatorAccounts();
            return OperatorAccounts.GetRecord(operatorAccountId);
        }

        public Vanrise.Entities.InsertOperationOutput<OperatorAccountDetail> AddOperatorAccount(OperatorAccount operatorAccount)
        {
            Vanrise.Entities.InsertOperationOutput<OperatorAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int operatorAccountId = -1;

            IOperatorAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorAccount, out operatorAccountId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                operatorAccount.OperatorAccountId = operatorAccountId;
                OperatorAccountDetail operatorAccountDetail = OperatorAccountDetailMapper(operatorAccount);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = operatorAccountDetail;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;


            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<OperatorAccountDetail> UpdateOperatorAccount(OperatorAccount operatorAccount)
        {
            IOperatorAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();

            bool updateActionSucc = dataManager.Update(operatorAccount);
            Vanrise.Entities.UpdateOperationOutput<OperatorAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                OperatorAccountDetail operatorAccountDetail = OperatorAccountDetailMapper(operatorAccount);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = operatorAccountDetail;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        Dictionary<int, OperatorAccount> GetCachedOperatorAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorAccounts",
               () =>
               {
                   IOperatorAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
                   IEnumerable<OperatorAccount> operatorAccounts = dataManager.GetOperatorAccounts();
                   return operatorAccounts.ToDictionary(kvp => kvp.OperatorAccountId, kvp => kvp);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorAccountDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorAccountsUpdated(ref _updateHandle);
            }
        }
        private static string GetOperatorAccountName(string profileName, string nameSuffix)
        {
            return string.Format("{0}{1}", profileName, string.IsNullOrEmpty(nameSuffix) ? string.Empty : " (" + nameSuffix + ")");
        }

        #endregion

        #region  Mappers

        private OperatorAccountDetail OperatorAccountDetailMapper(OperatorAccount operatorAccount)
        {
            OperatorAccountDetail operatorAccountDetail = new OperatorAccountDetail();
            operatorAccountDetail.Entity = operatorAccount;

            var operatorProfile = _operatorProfileManager.GetOperatorProfile(operatorAccount.OperatorProfileId);

            if (operatorProfile != null)
            {
                operatorAccountDetail.OperatorProfileName = operatorProfile.Name;
                operatorAccountDetail.OperatorAccountName = GetOperatorAccountName(operatorProfile.Name, operatorAccountDetail.Entity.NameSuffix);
            }

            return operatorAccountDetail;
        }

        private OperatorAccountInfo OperatorAccountInfoMapper(OperatorAccount operatorAccount)
        {
            var operatorProfile = _operatorProfileManager.GetOperatorProfile(operatorAccount.OperatorProfileId);

            return new OperatorAccountInfo()
            {
                OperatorAccountId = operatorAccount.OperatorAccountId,
                Name = GetOperatorAccountName(operatorProfile.Name, operatorAccount.NameSuffix)
            };
        }
        #endregion


    }
}
