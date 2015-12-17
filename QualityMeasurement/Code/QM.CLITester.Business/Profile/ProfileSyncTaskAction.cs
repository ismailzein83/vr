using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace QM.CLITester.Business
{
    public class ProfileSyncTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ProfileSyncTaskActionArgument profileSyncTaskActionArgument = taskActionArgument as ProfileSyncTaskActionArgument;
            SourceProfileSynchronizer sourceProfileSynchronizer = new SourceProfileSynchronizer(profileSyncTaskActionArgument.SourceProfileReader);
            sourceProfileSynchronizer.Synchronize();
            Console.WriteLine("ProfileSyncTaskAction Executed");
        }
    }
}
