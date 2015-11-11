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
    public class ApplySupplierZoneDetailsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public int RoutingDatabaseId { get; set; }

    }
    public sealed class ApplySupplierZoneDetailsToDB : DependentAsyncActivity<ApplySupplierZoneDetailsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        protected override void DoWork(ApplySupplierZoneDetailsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedSupplierZoneDetail) =>
                    {
                        dataManager.ApplySupplierZoneDetailsForDB(preparedSupplierZoneDetail);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplySupplierZoneDetailsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplySupplierZoneDetailsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
