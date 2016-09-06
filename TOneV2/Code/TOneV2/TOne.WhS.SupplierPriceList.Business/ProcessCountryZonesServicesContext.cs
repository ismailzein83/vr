using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ProcessCountryZonesServicesContext : IProcessCountryZonesServicesContext
    {

        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public IEnumerable<ExistingZoneService> ExistingZonesServices { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }
        public IEnumerable<NewZoneService> NewZonesServices { get; set; }

        public IEnumerable<ChangedZoneService> ChangedZonesServices { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }
    }
}
