using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace QM.CLITester.Business
{
    public class ProfileSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ProfileSyncTaskActionArgument profileSyncTaskActionArgument = taskActionArgument as ProfileSyncTaskActionArgument;
            SourceProfileSynchronizer sourceProfileSynchronizer = new SourceProfileSynchronizer(profileSyncTaskActionArgument.SourceProfileReader);
            sourceProfileSynchronizer.Synchronize();
            Console.WriteLine("ProfileSyncTaskAction Executed");

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
