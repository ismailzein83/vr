using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ProcessSaleZoneServicesInput
    {
        public IEnumerable<SaleZoneServiceToAdd> SaleZoneServicesToAdd { get; set; }

        public IEnumerable<SaleZoneServiceToClose> SaleZoneServicesToClose { get; set; }

        public IEnumerable<ExistingSaleZoneService> ExistingSaleZoneServices { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }
    }

    public class ProcessSaleZoneServicesOutput
    {
        public IEnumerable<NewSaleZoneService> NewSaleZoneServices { get; set; }

        public IEnumerable<ChangedSaleZoneService> ChangedSaleZoneServices { get; set; }
    }

    #endregion

    public class ProcessSaleZoneServices : BaseAsyncActivity<ProcessSaleZoneServicesInput, ProcessSaleZoneServicesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingSaleZoneService>> ExistingSaleZoneServices { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewSaleZoneService>> NewSaleZoneServices { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedSaleZoneService>> ChangedSaleZoneServices { get; set; }

        #endregion

        protected override ProcessSaleZoneServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessSaleZoneServicesInput()
            {
                SaleZoneServicesToAdd = this.SaleZoneServicesToAdd.Get(context),
                SaleZoneServicesToClose = this.SaleZoneServicesToClose.Get(context),
                ExistingSaleZoneServices = this.ExistingSaleZoneServices.Get(context),
                ExistingZones = this.ExistingZones.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.NewSaleZoneServices.Get(context) == null)
                this.NewSaleZoneServices.Set(context, new List<NewSaleZoneService>());

            if (this.ChangedSaleZoneServices.Get(context) == null)
                this.ChangedSaleZoneServices.Set(context, new List<ChangedSaleZoneService>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessSaleZoneServicesOutput DoWorkWithResult(ProcessSaleZoneServicesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd = inputArgument.SaleZoneServicesToAdd;
            IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose = inputArgument.SaleZoneServicesToClose;
            IEnumerable<ExistingSaleZoneService> existingSaleZoneServices = inputArgument.ExistingSaleZoneServices;
            IEnumerable<ExistingZone> existingZones = inputArgument.ExistingZones;

            var processSaleZoneServicesContext = new ProcessSaleZoneServicesContext()
            {
                SaleZoneServicesToAdd = saleZoneServicesToAdd,
                SaleZoneServicesToClose = saleZoneServicesToClose,
                ExistingSaleZoneServices = existingSaleZoneServices,
                ExistingZones = existingZones
            };

            var priceListSaleZoneServiceManager = new PriceListSaleZoneServiceManager();
            priceListSaleZoneServiceManager.ProcessSaleZoneServices(processSaleZoneServicesContext);

            return new ProcessSaleZoneServicesOutput()
            {
                NewSaleZoneServices = processSaleZoneServicesContext.NewSaleZoneServices,
                ChangedSaleZoneServices = processSaleZoneServicesContext.ChangedSaleZoneServices
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessSaleZoneServicesOutput result)
        {
            this.NewSaleZoneServices.Set(context, result.NewSaleZoneServices);
            this.ChangedSaleZoneServices.Set(context, result.ChangedSaleZoneServices);
        }
    }
}
