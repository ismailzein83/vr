using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
    public interface IProcessCountryZonesRoutingProductsContext
    {
        IEnumerable<ExistingZone> ExistingZones { get; }
        IEnumerable<ExistingZoneRoutingProducts> ExistingZonesRoutingProducts { get; set; }
        IEnumerable<ChangedZoneRoutingProducts> ChangedZonesRoutingProducts { get; set; }
    }
}
