using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ProcessSaleZoneRoutingProductsContext : IProcessSaleZoneRoutingProductsContext
    {
        // Input Properties
        public IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; set; }

        public IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; set; }

        public IEnumerable<ExistingSaleZoneRoutingProduct> ExistingSaleZoneRoutingProducts { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        // Output Properties
        public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }

        public IEnumerable<ChangedSaleZoneRoutingProduct> ChangedSaleZoneRoutingProducts { get; set; }
    }
}
