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
    public class SupplierCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISupplierCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
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

        protected override bool ShouldSetCacheExpired()
        {
            return _dataManager.AreSupplierCodesUpdated(ref _updateHandle)
                        |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _settingCacheLastCheck);
        }

        public SupplierCode CacheAndGetCode(SupplierCode code)
        {
            return code;
            //Dictionary<long, SupplierCode> cachedCodesById = this.GetOrCreateObject("cachedCodesById", () => new Dictionary<long, SupplierCode>());
            //SupplierCode matchCode;
            //lock (cachedCodesById)
            //{
            //    matchCode = cachedCodesById.GetOrCreateItem(code.SupplierCodeId, () => code);
            //}
            //return matchCode;
        }
    }
}
