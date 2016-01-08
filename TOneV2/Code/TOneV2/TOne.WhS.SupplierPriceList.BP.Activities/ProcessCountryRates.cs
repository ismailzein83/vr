using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class ProcessCountryRatesInput
    {
        public IEnumerable<ImportedRate> ImportedRates { get; set; }

        public IEnumerable<ExistingRate> ExistingRates { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

    }

    public class ProcessCountryRatesOutput
    {
        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<ChangedRate> ChangedRates { get; set; }

    }

    public sealed class ProcessCountryRates : BaseAsyncActivity<ProcessCountryRatesInput, ProcessCountryRatesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }


        protected override ProcessCountryRatesOutput DoWorkWithResult(ProcessCountryRatesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryRatesContext processCountryRateContext = new ProcessCountryRatesContext()
            {
                ImportedRates = inputArgument.ImportedRates,
                ExistingRates = inputArgument.ExistingRates,
                ExistingZones = existingZones,
                NewAndExistingZones = inputArgument.NewAndExistingZones
            };

            PriceListRateManager manager = new PriceListRateManager();
            manager.ProcessCountryRates(processCountryRateContext);

            return new ProcessCountryRatesOutput()
            {
                ChangedRates = processCountryRateContext.ChangedRates,
                NewRates = processCountryRateContext.NewRates
            };
        }

        protected override ProcessCountryRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryRatesInput()
            {
                ExistingRates = this.ExistingRates.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ImportedRates = this.ImportedRates.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryRatesOutput result)
        {
            NewRates.Set(context, result.NewRates);
            ChangedRates.Set(context, result.ChangedRates);
        }
    }
}
