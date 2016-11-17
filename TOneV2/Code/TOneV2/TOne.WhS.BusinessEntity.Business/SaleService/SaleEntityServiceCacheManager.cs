using System;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        private ISaleEntityServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
        private object _updateHandle;

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
        {
            return GetOrCreateObject(cacheName, BECacheExpirationChecker.Instance, createObject);
        }

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleEntityServicesUpdated(ref _updateHandle);
        }
    }
}
