using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
    public interface IProcessCountryRatesContext
    {
        IEnumerable<ExistingZone> ExistingZones { get; }
        IEnumerable<ExistingRate> ExistingRates { get; }
        IEnumerable<ZoneToProcess> ZonesToProcess { get; }
        IEnumerable<ChangedRate> ChangedRates { set; }
        IEnumerable<AddedRate> NewRates { get; set; }
        DateTime EffectiveDate { get; set; }
        int SellingNumberPlanId { get; set; }
    }
}
