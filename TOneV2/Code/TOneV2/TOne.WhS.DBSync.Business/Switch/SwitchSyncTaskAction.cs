using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            SwitchSyncTaskActionArgument switchSyncTaskActionArgument = taskActionArgument as SwitchSyncTaskActionArgument;
            SourceSwitchMigrator sourceSwitchMigrator = new SourceSwitchMigrator(switchSyncTaskActionArgument.SourceSwitchReader);
            sourceSwitchMigrator.Migrate();
            Console.WriteLine("SwitchSyncTaskAction Executed");
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
