using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ProcessCountryRatesContext : IProcessCountryRatesContext
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<ExistingRate> ExistingRates { get; set; }
        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }
        public IEnumerable<ChangedRate> ChangedRates { get; set; }
        public IEnumerable<AddedRate> NewRates { get; set; }
        public DateTime EffectiveDate { get; set; }
        public SalePriceListsByOwner SalePriceListsByOwner { get; set; }
        public int SellingNumberPlanId { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }
    }
}
