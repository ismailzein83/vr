using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISupplierRateDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
        object _updateHandle;
        DateTime? _settingCacheLastCheck;

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
            return _dataManager.AreSupplierRatesUpdated(ref _updateHandle)
                        |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _settingCacheLastCheck);
        }
        
        public SupplierRate CacheAndGetRate(SupplierRate rate)
        {
            return rate;
            //Dictionary<long, SupplierRate> cachedRatesById = this.GetOrCreateObject("cachedRatesById", () => new Dictionary<long, SupplierRate>());
            //SupplierRate matchRate;
            //lock (cachedRatesById)
            //{
            //    matchRate = cachedRatesById.GetOrCreateItem(rate.SupplierRateId, () => rate);
            //}
            //return matchRate;
        }
    }
}
