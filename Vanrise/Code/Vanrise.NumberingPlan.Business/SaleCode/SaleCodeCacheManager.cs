using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class SaleCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleCodeDataManager _dataManager = NPDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
        object _updateHandle;

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
        {
            return GetOrCreateObject(cacheName, NumberingPlanCacheExpirationChecker.Instance, createObject);
        }

        protected override bool ShouldSetCacheExpired()
        {
            return _dataManager.AreSaleCodesUpdated(ref _updateHandle);
        }

        public SaleCode CacheAndGetCode(SaleCode code)
        {
            return code;
            //Dictionary<long, SaleCode> cachedCodesById = this.GetOrCreateObject("cachedCodesById", () => new Dictionary<long, SaleCode>());
            //SaleCode matchCode;
            //lock (cachedCodesById)
            //{
            //    matchCode = cachedCodesById.GetOrCreateItem(code.SaleCodeId, () => code);
            //}
            //return matchCode;
        }
    }
}
