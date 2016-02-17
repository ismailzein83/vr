using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class NationalNumberingPlanManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<NationalNumberingPlanDetail> GetFilteredNationalNumberingPlans(Vanrise.Entities.DataRetrievalInput<NationalNumberingPlanQuery> input)
        {
            var allNationalNumberingPlans = GetCachedNationalNumberingPlans();

            Func<NationalNumberingPlan, bool> filterExpression = (prod) =>
                 (input.Query.FromDate == null || input.Query.FromDate < prod.FromDate)
                 &&

                  (input.Query.ToDate == null || input.Query.ToDate >= prod.ToDate)
                 &&

                 (input.Query.OperatorIds == null || input.Query.OperatorIds.Count == 0 || input.Query.OperatorIds.Contains(prod.OperatorId))
                 &&
                 (input.Query.NationalNumberingPlanIds == null || input.Query.NationalNumberingPlanIds.Contains(prod.NationalNumberingPlanId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allNationalNumberingPlans.ToBigResult(input, filterExpression, NationalNumberingPlanDetailMapper));
        }
        public NationalNumberingPlan GetNationalNumberingPlan(int nationalNumberingPlanId)
        {
            var plans = GetCachedNationalNumberingPlans();
            return plans.GetRecord(nationalNumberingPlanId);
        }
        public Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> AddNationalNumberingPlan(NationalNumberingPlan nationalNumberingPlan)
        {
            Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int planId = -1;

            INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
            bool insertActionSucc = dataManager.Insert(nationalNumberingPlan, out planId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                nationalNumberingPlan.NationalNumberingPlanId = planId;
                insertOperationOutput.InsertedObject = NationalNumberingPlanDetailMapper(nationalNumberingPlan);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> UpdateNationalNumberingPlan(NationalNumberingPlan nationalNumberingPlan)
        {
            INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();

            bool updateActionSucc = dataManager.Update(nationalNumberingPlan);
            Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = NationalNumberingPlanDetailMapper(nationalNumberingPlan);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, NationalNumberingPlan> GetCachedNationalNumberingPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetNationalNumberingPlans",
               () =>
               {
                   INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
                   IEnumerable<NationalNumberingPlan> plans = dataManager.GetNationalNumberingPlans();
                   return plans.ToDictionary(cn => cn.NationalNumberingPlanId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            INationalNumberingPlanDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreNationalNumberingPlansUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers

        private NationalNumberingPlanDetail NationalNumberingPlanDetailMapper(NationalNumberingPlan plan)
        {
            NationalNumberingPlanDetail planDetail = new NationalNumberingPlanDetail();

            planDetail.Entity = plan;

            OperatorProfileManager manager = new OperatorProfileManager();
            planDetail.OperatorName = manager.GetOperatorProfile(plan.OperatorId).Name;

            return planDetail;
        }
        #endregion
    }

}
