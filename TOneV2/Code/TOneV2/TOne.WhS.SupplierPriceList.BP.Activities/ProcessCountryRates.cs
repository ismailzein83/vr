using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ProcessCountryRates : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);
            IEnumerable<ExistingZone> existingZones = null;

            if (existingZonesByZoneId != null)
                existingZones = existingZonesByZoneId.Select(item => item.Value);

            ProcessCountryRatesContext processCountryRateContext = new ProcessCountryRatesContext()
            {
                ImportedRates = this.ImportedRates.Get(context),
                ExistingRates = this.ExistingRates.Get(context),
                ExistingZones = existingZones,
                NewAndExistingZones = this.NewAndExistingZones.Get(context)
            };
        }

        private class ProcessCountryRatesContext : IProcessCountryRatesContext
        {

            public IEnumerable<ImportedRate> ImportedRates { get; set; }

            public IEnumerable<ExistingRate> ExistingRates { get; set; }

            public IEnumerable<ExistingZone> ExistingZones { get; set; }

            public ZonesByName NewAndExistingZones { get; set; }


            public IEnumerable<NewRate> NewRates { get; set; }

            public IEnumerable<ChangedRate> ChangedRates { get; set; }
        }
    }
}
