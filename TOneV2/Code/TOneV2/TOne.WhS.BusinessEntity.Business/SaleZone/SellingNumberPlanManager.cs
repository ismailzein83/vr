using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SellingNumberPlanManager
    {
        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingNumberPlanDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingNumberPlansUpdated(ref _updateHandle);
            }
        }

        #endregion

        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            return GetCachedSellingNumberPlans().Values.ToList();

        }

        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId)
        {
            return GetCachedSellingNumberPlans().GetRecord(numberPlanId);
        }

        public IDataRetrievalResult<SellingNumberPlanDetail> GetFilteredSellingNumberPlans(DataRetrievalInput<SellingNumberPlanQuery> input)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            Func<SellingNumberPlan, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingNumberPlans.ToBigResult(input, filterExpression, SellingNumberPlanDetailMapper));

        }


        public TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingNumberPlanId = -1;

            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            bool insertActionSucc = dataManager.Insert(sellingNumberPlan, out sellingNumberPlanId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                sellingNumberPlan.SellingNumberPlanId = sellingNumberPlanId;
                insertOperationOutput.InsertedObject = SellingNumberPlanDetailMapper(sellingNumberPlan);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();

            bool updateActionSucc = dataManager.Update(sellingNumberPlan);
            TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingNumberPlanDetailMapper(sellingNumberPlan);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }



        #region Private Method
        Dictionary<int ,SellingNumberPlan> GetCachedSellingNumberPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingNumberPlans",
               () =>
               {
                   ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
                   return dataManager.GetSellingNumberPlans().ToDictionary(x=> x.SellingNumberPlanId , x => x);
               });

        }
        public SellingNumberPlanDetail SellingNumberPlanDetailMapper(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanDetail sellingNumberPlanDetail = new SellingNumberPlanDetail()
            {
                Entity = sellingNumberPlan
            };
            return sellingNumberPlanDetail;
        }

        #endregion
    }
}
