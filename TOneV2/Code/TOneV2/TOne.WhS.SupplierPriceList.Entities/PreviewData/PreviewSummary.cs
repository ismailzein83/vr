using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class PreviewSummary
    {
        public int NumberOfNewRates { get; set; }
        public int NumberOfIncreasedRates { get; set; }
        public int NumberOfDecreasedRates { get; set; }

        public int NumberOfNewOtherRates { get; set; }
        public int NumberOfIncreasedOtherRates { get; set; }
        public int NumberOfDecreasedOtherRates { get; set; }

        public int NumberOfNewZones { get; set; }

        public int NumberOfClosedZones { get; set; }

        public int NumberOfRenamedZones { get; set; }

        public int NumberOfNewCodes { get; set; }

        public int NumberOfClosedCodes { get; set; }

        public int NumberOfMovedCodes { get; set; }

        public int NumberOfZonesWithChangedServices { get; set; }

        public int NumberOfExcludedCountries { get; set; }
    }
}
