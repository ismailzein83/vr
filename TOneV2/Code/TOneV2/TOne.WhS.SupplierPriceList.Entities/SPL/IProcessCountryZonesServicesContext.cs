using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IProcessCountryZonesServicesContext
    {
        IEnumerable<ImportedZone> ImportedZones { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        IEnumerable<ExistingZoneService> ExistingZonesServices { get; }
        ZonesByName NewAndExistingZones { get; }

        DateTime PriceListDate { get; set; }

        IEnumerable<NewZoneService> NewZonesServices { get; set; }

        IEnumerable<ChangedZoneService> ChangedZonesServices { get; set; }

        IEnumerable<NotImportedZone> NotImportedZones { get; }
    }
}
