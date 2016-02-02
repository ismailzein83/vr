using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

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

        ISaleCodeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
        object _updateHandle;

        protected override bool ShouldSetCacheExpired()
        {
            return _dataManager.AreSaleCodesUpdated(ref _updateHandle);
        }
    }
}
