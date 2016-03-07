using InterConnect.BusinessEntity.Data;
using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace InterConnect.BusinessEntity.Business
{
    public class OperatorAccountManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<OperatorAccountDetail> GetFilteredOperatorAccounts(Vanrise.Entities.DataRetrievalInput<OperatorAccountQuery> input)
        {
            var allOperatorAccounts = GetCachedOperatorAccounts();

            Func<OperatorAccount, bool> filterExpression = (prod) =>
                 (input.Query.Suffix == null || prod.Suffix.ToLower().Contains(input.Query.Suffix.ToLower()))
              && (input.Query.OperatorProfileIds == null || input.Query.OperatorProfileIds.Count == 0 || input.Query.OperatorProfileIds.Contains(prod.ProfileId)); 

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorAccounts.ToBigResult(input, filterExpression, OperatorAccountDetailMapper));
        }

        public OperatorAccount GetOperatorAccount(int operatorAccountId)
        {
            var operatorProfiles = GetCachedOperatorAccounts();
            return operatorProfiles.GetRecord(operatorAccountId);
        }

        public string GetOperatorAccountName(int operatorAccountId)
        {
            OperatorAccount operatorAccount = GetOperatorAccount(operatorAccountId);
            return (operatorAccount != null) ? operatorAccount.Suffix : null;
        }
        public Vanrise.Entities.InsertOperationOutput<OperatorAccountDetail> AddOperatorAccount(OperatorAccount operatorAccount)
        {
            InsertOperationOutput<OperatorAccountDetail> insertOperationOutput = new InsertOperationOutput<OperatorAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int operatorAccountId = -1;

            IOperatorAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorAccount, out operatorAccountId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                operatorAccount.OperatorAccountId = operatorAccountId;
                insertOperationOutput.InsertedObject = OperatorAccountDetailMapper(operatorAccount);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<OperatorAccountDetail> UpdateOperatorAccount(OperatorAccount operatorAccount)
        {
            IOperatorAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();

            bool updateActionSucc = dataManager.Update(operatorAccount);
            UpdateOperationOutput<OperatorAccountDetail> updateOperationOutput = new UpdateOperationOutput<OperatorAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorAccountDetailMapper(operatorAccount);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var operatorAccountNames = new List<string>();
            foreach (var entityId in context.EntityIds)
            {
                string operatorAccountName = GetOperatorAccountName(Convert.ToInt32(entityId));
                if (operatorAccountName == null) throw new NullReferenceException("operatorAccountName");
                operatorAccountNames.Add(operatorAccountName);
            }
            return String.Join(",", operatorAccountNames);
        }

        public bool IsMatched(IBusinessEntityMatchContext context)
        {
            if (context.FieldValueIds == null || context.FilterIds == null) return true;

            var fieldValueIds = context.FieldValueIds.MapRecords(itm => Convert.ToInt32(itm));
            var filterIds = context.FilterIds.MapRecords(itm => Convert.ToInt32(itm));
            foreach (var filterId in filterIds)
            {
                if (fieldValueIds.Contains(filterId))
                    return true;
            }
            return false;
        }

        #endregion

        #region Private Members

        private Dictionary<int, OperatorAccount> GetCachedOperatorAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorAccounts",
               () =>
               {
                   IOperatorAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
                   IEnumerable<OperatorAccount> accounts = dataManager.GetOperatorAccounts();
                   return accounts.ToDictionary(cn => cn.OperatorAccountId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<IOperatorAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorAccountsUpdated(ref _updateHandle);
            }
        }

        private OperatorAccountDetail OperatorAccountDetailMapper(OperatorAccount operatorAccount)
        {
            if (operatorAccount == null)
                return null;

            OperatorProfileManager operatorProfileManager = new OperatorProfileManager();
            string operatorProfileName = operatorProfileManager.GetOperatorProfileName(operatorAccount.ProfileId);

            return new OperatorAccountDetail()
            {
                Entity = operatorAccount,
                OperatorProfileName = operatorProfileName
            };
        }

        #endregion
    }
}