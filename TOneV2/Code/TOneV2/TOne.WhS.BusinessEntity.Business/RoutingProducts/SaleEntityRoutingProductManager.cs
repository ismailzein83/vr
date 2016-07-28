using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductManager
    {
        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            return dataManager.GetDefaultRoutingProductsEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            return dataManager.GetSaleZoneRoutingProductsEffectiveAfter(ownerType, ownerId, minimumDate);
        }
    }
}
