using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        private ISupplierZoneServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
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
            return _dataManager.AreSupplierZoneServicesUpdated(ref _updateHandle);
        }
    }
}
