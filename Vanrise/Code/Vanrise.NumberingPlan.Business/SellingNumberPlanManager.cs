using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;

namespace Vanrise.NumberingPlan.Business
{
    public class SellingNumberPlanManager :  ISellingNumberPlanManager
    {
        #region Public Methods

        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId)
        {
            return GetCachedSellingNumberPlans().GetRecord(numberPlanId);
        }

        public IEnumerable<SellingNumberPlanInfo> GetSellingNumberPlans()
        {
            return GetCachedSellingNumberPlans().Values.MapRecords(SellingNumberPlanInfoMapper).OrderBy(x => x.Name);
        }
       
        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public string GetSellingNumberPlanName(int sellingNumberPlanId)
        {
            SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);

            if (sellingNumberPlan != null)
                return sellingNumberPlan.Name;

            return null;
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingNumberPlanDataManager _dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingNumberPlansUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Mappers

        private SellingNumberPlanInfo SellingNumberPlanInfoMapper(SellingNumberPlan sellingNumberPlan)
        {
            return new SellingNumberPlanInfo
            {
                Name = sellingNumberPlan.Name,
                SellingNumberPlanId = sellingNumberPlan.SellingNumberPlanId
            };
        }

        #endregion

        #region Private Methods

        Dictionary<int, SellingNumberPlan> GetCachedSellingNumberPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingNumberPlans",
               () =>
               {
                   ISellingNumberPlanDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
                   return dataManager.GetSellingNumberPlans().ToDictionary(x => x.SellingNumberPlanId, x => x);
               });

        }

        #endregion


       
    }
}
