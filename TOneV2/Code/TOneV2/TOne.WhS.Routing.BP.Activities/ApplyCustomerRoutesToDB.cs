using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{

    public class ApplyCustomerRoutesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public sealed class ApplyCustomerRoutesToDB : DependentAsyncActivity<ApplyCustomerRoutesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyCustomerRoutesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCustomerRoute) =>
                    {
                        dataManager.ApplyCustomerRouteForDB(preparedCustomerRoute);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyCustomerRoutesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCustomerRoutesToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
