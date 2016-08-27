using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IProcessCountryRatesContext
    {
        IEnumerable<ImportedZone> ImportedZones { get; }

        Dictionary<string, ExistingRateGroup> ExistingRatesGroupsByZoneName { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        ZonesByName NewAndExistingZones { get; }

        DateTime PriceListDate { get; set; }

        IEnumerable<NewRate> NewRates { set; }

        IEnumerable<ChangedRate> ChangedRates { set; }

        IEnumerable<NotImportedZone> NotImportedZones { get; }
    }
}
