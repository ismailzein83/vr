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
    public class SupplierCodeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        ISupplierCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierCodeDataManager>();
        object _updateHandle;

        protected override bool ShouldSetCacheExpired()
        {
            return _dataManager.AreSupplierCodesUpdated(ref _updateHandle);
        }

        public SupplierCode CacheAndGetCode(SupplierCode code)
        {
            Dictionary<long, SupplierCode> cachedCodesById = this.GetOrCreateObject("cachedCodesById", () => new Dictionary<long, SupplierCode>());
            SupplierCode matchCode;
            lock (cachedCodesById)
            {
                matchCode = cachedCodesById.GetOrCreateItem(code.SupplierCodeId, () => code);
            }
            return matchCode;
        }
    }
}
