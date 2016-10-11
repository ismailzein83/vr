using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ProcessCountryZonesServicesContext : IProcessCountryZonesServicesContext
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<ExistingZoneServices> ExistingZonesServices { get; set; }
        public IEnumerable<ChangedZoneServices> ChangedZonesServices { get; set; }
       
    }
}
