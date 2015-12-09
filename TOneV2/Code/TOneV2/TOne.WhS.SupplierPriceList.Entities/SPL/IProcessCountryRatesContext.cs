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
        IEnumerable<ImportedRate> ImportedRates { get; }

        IEnumerable<ExistingRate> ExistingRates { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        ZonesByName NewAndExistingZones { get; }
    }
}
