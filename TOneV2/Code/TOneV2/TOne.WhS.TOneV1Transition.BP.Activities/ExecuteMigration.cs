using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.DBSync.Business;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.TOneV1Transition.BP.Activities
{
    public sealed class ExecuteMigration : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DBSyncTaskActionArgument> DBSyncTaskActionArgument { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            DBSyncTaskActionArgument dbSyncTaskActionArgument = this.DBSyncTaskActionArgument.Get(context.ActivityContext);

            DBSyncTaskAction dbSyncTaskAction = new DBSyncTaskAction();
            dbSyncTaskAction.Execute(new SchedulerTask(), dbSyncTaskActionArgument, null);
        }
    }
}