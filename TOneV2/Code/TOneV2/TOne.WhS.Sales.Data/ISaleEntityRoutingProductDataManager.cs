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
        bool InsertDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, NewDefaultRoutingProduct newDefaultRoutingProduct);

        bool UpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DefaultRoutingProductChange defaultRoutingProductChange);

        bool InsertZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<NewZoneRoutingProduct> newZoneRoutingProducts);

        bool UpdateZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneRoutingProductChange> zoneRoutingProductChanges);
    }
}
