using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    public class UpdateRouteBatchesInput
    {
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<RouteDetailBatch> RouteBatches { get; set; }
    }


    public sealed class UpdateRouteBatches : DependentAsyncActivity<UpdateRouteBatchesInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteDetailBatch>> RouteBatches { get; set; }

        protected override void DoWork(UpdateRouteBatchesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<RouteDetail> routeDetails = new List<RouteDetail>();
            IRouteDetailDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteDetailDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.RouteBatches.TryDequeue(
                        (routeBatches) =>
                        {
                            routeDetails.AddRange(routeBatches.Routes);
                            if (routeDetails.Count > 1000)
                            {
                                dataManager.UpdateRoutes(routeDetails);
                                routeDetails = new List<RouteDetail>();
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
                if (routeDetails.Count > 0)
                    dataManager.UpdateRoutes(routeDetails);
            });
        }

        protected override UpdateRouteBatchesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateRouteBatchesInput()
            {
                RouteBatches = this.RouteBatches.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
