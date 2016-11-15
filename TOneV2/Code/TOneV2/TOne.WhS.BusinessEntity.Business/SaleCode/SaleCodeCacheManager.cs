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
    public class SaleCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
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
            return _dataManager.AreSaleCodesUpdated(ref _updateHandle)
                        |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _settingCacheLastCheck);
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
