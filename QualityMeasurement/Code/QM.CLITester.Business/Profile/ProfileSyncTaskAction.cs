using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace QM.CLITester.Business
{
    public class ProfileSyncTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ProfileSyncTaskActionArgument ProfileSyncTaskActionArgument = taskActionArgument as ProfileSyncTaskActionArgument;
            SourceProfileSynchronizer sourceProfileSynchronizer = new SourceProfileSynchronizer(ProfileSyncTaskActionArgument.SourceProfileReader);
            sourceProfileSynchronizer.Synchronize();
            Console.WriteLine("ProfileSyncTaskAction Executed");
            //throw new NotImplementedException();
        }
    }
}
