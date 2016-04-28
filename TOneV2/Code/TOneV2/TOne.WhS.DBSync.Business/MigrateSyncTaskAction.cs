using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MigrateSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            MigrateSyncTaskActionArgument migrateSyncTaskActionArgument = taskActionArgument as MigrateSyncTaskActionArgument;
            SourceSwitchMigrator sourceSwitchMigrator = new SourceSwitchMigrator(migrateSyncTaskActionArgument.SourceMigrationReader);
            sourceSwitchMigrator.Migrate();
            Console.WriteLine("MigrationSyncTaskAction Executed");
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
