using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
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


        ISaleCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
        object _updateHandle;

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
