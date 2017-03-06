using System.Activities;
using TOne.WhS.DBSync.Business;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.TOneV1Transition.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.TOneV1Transition.BP.Activities
{
    public sealed class LoadProcessesInput : BaseCodeActivity
    {
        [RequiredArgument]
        public OutArgument<DBSyncTaskActionArgument> DBSyncTaskActionArgument { get; set; }

        [RequiredArgument]
        public OutArgument<RoutingProcessInput> RoutingProcessInput { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Process Input");

            ConfigManager configManager = new ConfigManager();
            DBSyncTaskActionArgument dbSyncTaskActionArgument = configManager.GetDBSyncTaskActionArgument();
            RoutingProcessInput routingProcessInput = configManager.GetRoutingProcessInput();

            this.DBSyncTaskActionArgument.Set(context.ActivityContext, dbSyncTaskActionArgument);
            this.RoutingProcessInput.Set(context.ActivityContext, routingProcessInput);

            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Process Input");
        }
    }
}