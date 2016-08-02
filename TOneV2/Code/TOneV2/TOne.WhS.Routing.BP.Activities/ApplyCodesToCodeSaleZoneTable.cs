using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class ApplyCodesToCodeSaleZoneTableInput
    {
        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }


    public sealed class ApplyCodesToCodeSaleZoneTable : DependentAsyncActivity<ApplyCodesToCodeSaleZoneTableInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyCodesToCodeSaleZoneTableInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeSaleZoneDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeSaleZoneDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        dataManager.ApplyCodeToCodeSaleZoneTable(preparedCodeMatch);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Codes To Code Sale Zone Table is done", null);
        }

        protected override ApplyCodesToCodeSaleZoneTableInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCodesToCodeSaleZoneTableInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
