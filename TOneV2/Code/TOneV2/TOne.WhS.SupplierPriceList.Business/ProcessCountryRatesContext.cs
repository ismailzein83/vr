using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ProcessCountryRatesContext : IProcessCountryRatesContext
    {

        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public ExistingRateGroupByZoneName ExistingRatesGroupsByZoneName { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }
        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<ChangedRate> ChangedRates { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }
    }
}
