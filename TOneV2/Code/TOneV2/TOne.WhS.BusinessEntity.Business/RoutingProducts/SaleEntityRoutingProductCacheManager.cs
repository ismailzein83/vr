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
    public class SaleEntityRoutingProductCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleEntityRoutingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
        object _updateHandle;

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleEntityRoutingProductUpdated(ref _updateHandle);
        }
        
        public SaleZoneRoutingProduct CacheAndGetSaleZoneRP(SaleZoneRoutingProduct saleZoneRP)
        {
            Dictionary<long, SaleZoneRoutingProduct> cachedSaleZoneRPsById = this.GetOrCreateObject("cachedSaleZoneRPsById", () => new Dictionary<long, SaleZoneRoutingProduct>());
            SaleZoneRoutingProduct matchSaleZoneRP;
            lock (cachedSaleZoneRPsById)
            {
                matchSaleZoneRP = cachedSaleZoneRPsById.GetOrCreateItem(saleZoneRP.SaleEntityRoutingProductId, () => saleZoneRP);
            }
            return matchSaleZoneRP;
        }

        public DefaultRoutingProduct CacheAndGetDefaultRP(DefaultRoutingProduct defaultRP)
        {
            Dictionary<long, DefaultRoutingProduct> cachedDefaultRPsById = this.GetOrCreateObject("cachedDefaultRPsById", () => new Dictionary<long, DefaultRoutingProduct>());
            DefaultRoutingProduct matchDefaultRP;
            lock (cachedDefaultRPsById)
            {
                matchDefaultRP = cachedDefaultRPsById.GetOrCreateItem(defaultRP.SaleEntityRoutingProductId, () => defaultRP);
            }
            return matchDefaultRP;
        }
    }
}
