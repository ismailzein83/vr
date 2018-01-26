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

    public class ApplyChangedSaleZoneServicesToDBInput
    {
        public IEnumerable<ChangedSaleZoneService> ChangedSaleZoneServices { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedSaleZoneServicesToDBOutput
    {

    }

    #endregion

    public class ApplyChangedSaleZoneServicesToDB : BaseAsyncActivity<ApplyChangedSaleZoneServicesToDBInput, ApplyChangedSaleZoneServicesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedSaleZoneService>> ChangedSaleZoneServices { get; set; }

        #endregion

        protected override ApplyChangedSaleZoneServicesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedSaleZoneServicesToDBInput()
            {
                ChangedSaleZoneServices = this.ChangedSaleZoneServices.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedSaleZoneServicesToDBOutput DoWorkWithResult(ApplyChangedSaleZoneServicesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ChangedSaleZoneService> changedSaleZoneServices = inputArgument.ChangedSaleZoneServices;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            var dataManager = SalesDataManagerFactory.GetDataManager<IChangedSaleZoneServiceDataManager>();
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyChangedSaleZoneServicesToDB(changedSaleZoneServices);

            return new ApplyChangedSaleZoneServicesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedSaleZoneServicesToDBOutput result)
        {

        }
    }
}
