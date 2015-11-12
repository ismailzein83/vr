using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

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

        #endregion
    }
}
