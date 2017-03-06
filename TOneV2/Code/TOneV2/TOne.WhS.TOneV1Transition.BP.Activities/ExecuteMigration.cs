using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.DBSync.Business;
using Vanrise.Runtime.Entities;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.TOneV1Transition.BP.Activities
{
    public sealed class ExecuteMigration : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DBSyncTaskActionArgument> DBSyncTaskActionArgument { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Starting Data Migration From Current TONE Database");

            DBSyncTaskActionArgument dbSyncTaskActionArgument = this.DBSyncTaskActionArgument.Get(context.ActivityContext);

            BPMigrationContext bpMigrationContext = new BPMigrationContext(context.ActivityContext.GetSharedInstanceData());
            DBSyncTaskAction dbSyncTaskAction = new DBSyncTaskAction(bpMigrationContext);
            dbSyncTaskAction.Execute(null, dbSyncTaskActionArgument, null);

            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finished Data Migration From Current TONE Database");
        }
    }
}