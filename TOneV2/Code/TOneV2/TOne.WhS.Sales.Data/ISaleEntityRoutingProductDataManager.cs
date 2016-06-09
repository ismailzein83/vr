using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface ISaleEntityRoutingProductDataManager : IDataManager
    {
        bool InsertDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DraftNewDefaultRoutingProduct newDefaultRoutingProduct);

        bool UpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DraftChangedDefaultRoutingProduct defaultRoutingProductChange);

        bool InsertZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<DraftNewSaleZoneRoutingProduct> newZoneRoutingProducts);

        bool UpdateZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<DraftChangedSaleZoneRoutingProduct> zoneRoutingProductChanges);
    }
}
