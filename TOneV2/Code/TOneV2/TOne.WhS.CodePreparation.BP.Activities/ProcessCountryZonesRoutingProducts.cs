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

    public class ProcessCountryZonesRoutingProductsInput
    {
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
        public IEnumerable<ExistingZoneRoutingProducts> ExistingZonesRoutingProducts { get; set; }
    }

    public class ProcessCountryZonesRoutingProductsOutput
    {
        public IEnumerable<ChangedZoneRoutingProducts> ChangedZonesRoutingProducts { get; set; }
    }

    public sealed class ProcessCountryZonesRoutingProducts : BaseAsyncActivity<ProcessCountryZonesRoutingProductsInput, ProcessCountryZonesRoutingProductsOutput>
    {

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZoneRoutingProducts>> ExistingZonesRoutingProducts { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZoneRoutingProducts>> ChangedZonesRoutingProducts { get; set; }


        protected override ProcessCountryZonesRoutingProductsOutput DoWorkWithResult(ProcessCountryZonesRoutingProductsInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryZonesRoutingProductsContext processCountryZonesServicesContext = new ProcessCountryZonesRoutingProductsContext()
            {
                ExistingZones = existingZones,
                ExistingZonesRoutingProducts = inputArgument.ExistingZonesRoutingProducts
            };

            PriceListZoneRoutingProductsManager plZonesServicesManager = new PriceListZoneRoutingProductsManager();
            plZonesServicesManager.ProcessCountryZonesRoutingProducts(processCountryZonesServicesContext);


            return new ProcessCountryZonesRoutingProductsOutput()
            {
                ChangedZonesRoutingProducts = processCountryZonesServicesContext.ChangedZonesRoutingProducts
            };
        }

        protected override ProcessCountryZonesRoutingProductsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesRoutingProductsInput()
            {
                ExistingZonesRoutingProducts = this.ExistingZonesRoutingProducts.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesRoutingProductsOutput result)
        {
            ChangedZonesRoutingProducts.Set(context, result.ChangedZonesRoutingProducts);
        }
    }
}
