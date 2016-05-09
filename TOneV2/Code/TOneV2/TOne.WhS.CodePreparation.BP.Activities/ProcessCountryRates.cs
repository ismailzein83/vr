using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ProcessCountryRatesInput
    {
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

    }

    public class ProcessCountryRatesOutput
    {
        public IEnumerable<ChangedRate> ChangedRates { get; set; }

    }

    public sealed class ProcessCountryRates : BaseAsyncActivity<ProcessCountryRatesInput, ProcessCountryRatesOutput>
    {

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }


        protected override ProcessCountryRatesOutput DoWorkWithResult(ProcessCountryRatesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryRatesContext processCountryRateContext = new ProcessCountryRatesContext()
            {
                ExistingZones = existingZones
            };

            PriceListRateManager plCodeManager = new PriceListRateManager();
            plCodeManager.ProcessCountryRates(processCountryRateContext);


            return new ProcessCountryRatesOutput()
            {
                ChangedRates = processCountryRateContext.ChangedRates
            };
        }

        protected override ProcessCountryRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryRatesInput()
            {
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryRatesOutput result)
        {
            ChangedRates.Set(context, result.ChangedRates);
        }
    }
}
