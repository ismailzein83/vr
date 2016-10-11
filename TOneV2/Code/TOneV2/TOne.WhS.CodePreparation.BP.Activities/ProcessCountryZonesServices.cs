using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ProcessCountryZonesServicesInput
    {
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
        public IEnumerable<ExistingZoneServices> ExistingZonesServices { get; set; }
    }

    public class ProcessCountryZonesServicesOutput
    {
        public IEnumerable<ChangedZoneServices> ChangedZonesServices { get; set; }
    }

    public sealed class ProcessCountryZonesServices : BaseAsyncActivity<ProcessCountryZonesServicesInput, ProcessCountryZonesServicesOutput>
    {

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZoneServices>> ExistingZonesServices { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZoneServices>> ChangedZonesServices { get; set; }


        protected override ProcessCountryZonesServicesOutput DoWorkWithResult(ProcessCountryZonesServicesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryZonesServicesContext processCountryZonesServicesContext = new ProcessCountryZonesServicesContext()
            {
                ExistingZones = existingZones,
                ExistingZonesServices = inputArgument.ExistingZonesServices
            };

            PriceListZoneServicesManager plZoneServicesManager = new PriceListZoneServicesManager();
            plZoneServicesManager.ProcessCountryZonesServices(processCountryZonesServicesContext);


            return new ProcessCountryZonesServicesOutput()
            {
                ChangedZonesServices = processCountryZonesServicesContext.ChangedZonesServices
            };
        }

        protected override ProcessCountryZonesServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesServicesInput()
            {
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ExistingZonesServices = this.ExistingZonesServices.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesServicesOutput result)
        {
            ChangedZonesServices.Set(context, result.ChangedZonesServices);
        }
    }
}
