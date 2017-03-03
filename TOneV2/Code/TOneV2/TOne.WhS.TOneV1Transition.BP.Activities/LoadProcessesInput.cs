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

        [RequiredArgument]
        public OutArgument<int> RoutingMigrationOffsetInMinutes { get; set; }


        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            ConfigManager configManager = new ConfigManager();
            DBSyncTaskActionArgument dbSyncTaskActionArgument = configManager.GetDBSyncTaskActionArgument();
            RoutingProcessInput routingProcessInput = configManager.GetRoutingProcessInput();
            int routingMigrationOffsetInMinutes = configManager.GetRoutingMigrationOffsetInMinutes();

            this.DBSyncTaskActionArgument.Set(context.ActivityContext, dbSyncTaskActionArgument);
            this.RoutingProcessInput.Set(context.ActivityContext, routingProcessInput);
            this.RoutingMigrationOffsetInMinutes.Set(context.ActivityContext, routingMigrationOffsetInMinutes);
        }
    }
}