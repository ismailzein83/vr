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
    public class ApplyCustomerZoneDetailsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public int RoutingDatabaseId { get; set; }

    }
    public sealed class ApplyCustomerZoneDetailsToDB : DependentAsyncActivity<ApplyCustomerZoneDetailsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        protected override void DoWork(ApplyCustomerZoneDetailsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCustomerZoneDetail) =>
                    {
                        dataManager.ApplyCustomerZoneDetailsToDB(preparedCustomerZoneDetail);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyCustomerZoneDetailsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCustomerZoneDetailsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
