using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessSaleZoneRoutingProductsContext
    {
        // Input Properties
        IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; }

        IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; }

        IEnumerable<ExistingSaleZoneRoutingProduct> ExistingSaleZoneRoutingProducts { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        // Output Properties
        IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { set; }

        IEnumerable<ChangedSaleZoneRoutingProduct> ChangedSaleZoneRoutingProducts { set; }
    }
}
