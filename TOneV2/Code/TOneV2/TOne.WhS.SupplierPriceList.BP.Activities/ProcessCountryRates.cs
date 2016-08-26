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
        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public Dictionary<string, ExistingRateGroup> ExistingRatesGroupsByZoneName { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }

        public IEnumerable<int> ImportedRateTypeIds { get; set; }

    }

    public class ProcessCountryRatesOutput
    {
        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<ChangedRate> ChangedRates { get; set; }

    }

    public sealed class ProcessCountryRates : BaseAsyncActivity<ProcessCountryRatesInput, ProcessCountryRatesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, ExistingRateGroup>> ExistingRatesGroupsByZoneName { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> PriceListDate { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<int>> ImportedRateTypeIds { get; set; }

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
                ImportedZones = inputArgument.ImportedZones,
                ExistingRatesGroupsByZoneName = inputArgument.ExistingRatesGroupsByZoneName,
                ExistingZones = existingZones,
                NewAndExistingZones = inputArgument.NewAndExistingZones,
                PriceListDate = inputArgument.PriceListDate
            };

            PriceListRateManager manager = new PriceListRateManager();
            manager.ProcessCountryRates(processCountryRateContext, inputArgument.ImportedRateTypeIds);

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
                ExistingRatesGroupsByZoneName = this.ExistingRatesGroupsByZoneName.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ImportedZones = this.ImportedZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                ImportedRateTypeIds = this.ImportedRateTypeIds.Get(context),
                PriceListDate = this.PriceListDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryRatesOutput result)
        {
            NewRates.Set(context, result.NewRates);
            ChangedRates.Set(context, result.ChangedRates);
        }
    }
}
