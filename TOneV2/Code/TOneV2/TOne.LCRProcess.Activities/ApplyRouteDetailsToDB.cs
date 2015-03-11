using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public class ApplyRouteDetailsToDBInput
    {
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyRouteDetailsToDB : DependentAsyncActivity<ApplyRouteDetailsToDBInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyRouteDetailsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRouteDetailDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteDetailDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                 {
                     bool hasItems = false;
                     do
                     {
                         hasItems = inputArgument.InputQueue.TryDequeue(
                             (preparedRouteDetails) =>
                             {
                                 dataManager.ApplyRouteDetailsToDB(preparedRouteDetails);
                             });
                     } while (!ShouldStop(handle) && hasItems);
                 }
                );
        }

        protected override ApplyRouteDetailsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyRouteDetailsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
