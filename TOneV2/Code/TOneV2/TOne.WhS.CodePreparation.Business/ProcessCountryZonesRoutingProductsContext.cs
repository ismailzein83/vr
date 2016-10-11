using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ProcessCountryZonesRoutingProductsContext : IProcessCountryZonesRoutingProductsContext
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<ExistingZoneRoutingProducts> ExistingZonesRoutingProducts { get; set; }
        public IEnumerable<ChangedZoneRoutingProducts> ChangedZonesRoutingProducts { get; set; }
    }
}
