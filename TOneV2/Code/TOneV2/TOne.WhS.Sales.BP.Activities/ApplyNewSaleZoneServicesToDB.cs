using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ApplyNewSaleZoneServicesToDBInput
    {
        public IEnumerable<NewSaleZoneService> NewSaleZoneServices { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewSaleZoneServicesToDBOutput
    {

    }

    #endregion

    public class ApplyNewSaleZoneServicesToDB : BaseAsyncActivity<ApplyNewSaleZoneServicesToDBInput, ApplyNewSaleZoneServicesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneService>> NewSaleZoneServices { get; set; }

        #endregion

        protected override ApplyNewSaleZoneServicesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewSaleZoneServicesToDBInput()
            {
                NewSaleZoneServices = this.NewSaleZoneServices.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewSaleZoneServicesToDBOutput DoWorkWithResult(ApplyNewSaleZoneServicesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<NewSaleZoneService> newSaleZoneServices = inputArgument.NewSaleZoneServices;

            var dataManager = SalesDataManagerFactory.GetDataManager<INewSaleZoneServiceDataManager>();
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyNewSaleZoneServicesToDB(newSaleZoneServices);

            return new ApplyNewSaleZoneServicesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewSaleZoneServicesToDBOutput result)
        {

        }
    }
}
