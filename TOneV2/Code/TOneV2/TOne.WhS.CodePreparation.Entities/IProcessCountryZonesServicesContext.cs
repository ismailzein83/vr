using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
    public interface IProcessCountryZonesServicesContext
    {
        IEnumerable<ExistingZone> ExistingZones { get; }
        IEnumerable<ExistingZoneServices> ExistingZonesServices { get; }
        IEnumerable<ChangedZoneServices> ChangedZonesServices { set; }
    }
}
