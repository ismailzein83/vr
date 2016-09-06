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

    public class ProcessCountryZonesServicesInput
    {
        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

        public IEnumerable<ExistingZoneService> ExistingZonesServices { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public DateTime PriceListDate { get; set; }

        public IEnumerable<NotImportedZone> NotImportedZones { get; set; }

    }

    public class ProcessCountryZonesServicesOutput
    {
        public IEnumerable<NewZoneService> NewZonesServices { get; set; }

        public IEnumerable<ChangedZoneService> ChangedZonesServices { get; set; }

    }

    public sealed class ProcessCountryZonesServices : BaseAsyncActivity<ProcessCountryZonesServicesInput, ProcessCountryZonesServicesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZoneService>> ExistingZonesServices { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> PriceListDate { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NewZoneService>> NewZonesServices { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZoneService>> ChangedZonesServices { get; set; }


        protected override ProcessCountryZonesServicesOutput DoWorkWithResult(ProcessCountryZonesServicesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ExistingZone> existingZones = null;

            if (inputArgument.ExistingZonesByZoneId != null)
                existingZones = inputArgument.ExistingZonesByZoneId.Select(item => item.Value);

            ProcessCountryZonesServicesContext processCountryZonesServicesContext = new ProcessCountryZonesServicesContext()
            {
                ImportedZones = inputArgument.ImportedZones,
                ExistingZones = existingZones,
                NewAndExistingZones = inputArgument.NewAndExistingZones,
                PriceListDate = inputArgument.PriceListDate,
                NotImportedZones = inputArgument.NotImportedZones
            };

            PriceListZoneServiceManager manager = new PriceListZoneServiceManager();
            manager.ProcessCountryZonesServices(processCountryZonesServicesContext);

            return new ProcessCountryZonesServicesOutput()
            {
                ChangedZonesServices = processCountryZonesServicesContext.ChangedZonesServices,
                NewZonesServices = processCountryZonesServicesContext.NewZonesServices
            };
        }

        protected override ProcessCountryZonesServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryZonesServicesInput()
            {
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context),
                ImportedZones = this.ImportedZones.Get(context),
                NewAndExistingZones = this.NewAndExistingZones.Get(context),
                PriceListDate = this.PriceListDate.Get(context),
                NotImportedZones = this.NotImportedZones.Get(context),
                ExistingZonesServices = this.ExistingZonesServices.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryZonesServicesOutput result)
        {
            NewZonesServices.Set(context, result.NewZonesServices);
            ChangedZonesServices.Set(context, result.ChangedZonesServices);
        }
    }
}
