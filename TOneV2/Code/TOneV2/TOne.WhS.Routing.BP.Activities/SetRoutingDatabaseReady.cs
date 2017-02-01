using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class SetRoutingDatabaseReadyInput
    {
        public int RoutingDatabaseId { get; set; }
    }


    public sealed class SetRoutingDatabaseReady : DependentAsyncActivity<SetRoutingDatabaseReadyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(SetRoutingDatabaseReadyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                
            });

            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            routingDatabaseManager.SetReady(inputArgument.RoutingDatabaseId);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Routing Database is set to ready", null);
        }

        protected override SetRoutingDatabaseReadyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SetRoutingDatabaseReadyInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}