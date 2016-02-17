using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class OperatorDeclaredInformationManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorDeclaredInformationDetail> GetFilteredOperatorDeclaredInformations(Vanrise.Entities.DataRetrievalInput<OperatorDeclaredInformationQuery> input)
        {
            var allOperatorDeclaredInformations = GetCachedOperatorDeclaredInformations();

            Func<OperatorDeclaredInformation, bool> filterExpression = (prod) =>
                 (input.Query.FromDate == null || input.Query.FromDate < prod.FromDate)
                 &&

                  (input.Query.ToDate == null || input.Query.ToDate >= prod.ToDate)
                 &&

                 (input.Query.OperatorIds == null || input.Query.OperatorIds.Count == 0 || input.Query.OperatorIds.Contains(prod.OperatorId))
                 &&
                 (input.Query.OperatorDeclaredInformationIds == null || input.Query.OperatorDeclaredInformationIds.Contains(prod.OperatorDeclaredInformationId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorDeclaredInformations.ToBigResult(input, filterExpression, OperatorDeclaredInformationDetailMapper));
        }
        public OperatorDeclaredInformation GetOperatorDeclaredInformation(int OperatorDeclaredInformationId)
        {
            var plans = GetCachedOperatorDeclaredInformations();
            return plans.GetRecord(OperatorDeclaredInformationId);
        }
        public Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail> AddOperatorDeclaredInformation(OperatorDeclaredInformation OperatorDeclaredInformation)
        {
            Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int planId = -1;

            IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
            bool insertActionSucc = dataManager.Insert(OperatorDeclaredInformation, out planId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                OperatorDeclaredInformation.OperatorDeclaredInformationId = planId;
                insertOperationOutput.InsertedObject = OperatorDeclaredInformationDetailMapper(OperatorDeclaredInformation);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail> UpdateOperatorDeclaredInformation(OperatorDeclaredInformation OperatorDeclaredInformation)
        {
            IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();

            bool updateActionSucc = dataManager.Update(OperatorDeclaredInformation);
            Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorDeclaredInformationDetailMapper(OperatorDeclaredInformation);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, OperatorDeclaredInformation> GetCachedOperatorDeclaredInformations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorDeclaredInformations",
               () =>
               {
                   IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
                   IEnumerable<OperatorDeclaredInformation> plans = dataManager.GetOperatorDeclaredInformations();
                   return plans.ToDictionary(cn => cn.OperatorDeclaredInformationId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorDeclaredInformationDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorDeclaredInformationsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers
      
        private OperatorDeclaredInformationDetail OperatorDeclaredInformationDetailMapper(OperatorDeclaredInformation plan)
        {
            OperatorDeclaredInformationDetail planDetail = new OperatorDeclaredInformationDetail();

            planDetail.Entity = plan;

            OperatorProfileManager manager = new OperatorProfileManager();
            if (plan.Settings != null)
            {
                planDetail.OperatorName = manager.GetOperatorProfile(plan.OperatorId).Name;
            }

            return planDetail;
        }
        #endregion
    }

}
