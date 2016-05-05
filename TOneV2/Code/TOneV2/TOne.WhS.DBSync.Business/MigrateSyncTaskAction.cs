using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MigrateSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            MigrateSyncTaskActionArgument migrateSyncTaskActionArgument = taskActionArgument as MigrateSyncTaskActionArgument;

            MigratorTOneV1Reader migratorTOneV1Reader = new MigratorTOneV1Reader();
            migratorTOneV1Reader.ConnectionString = migrateSyncTaskActionArgument.ConnectionString;

            SourceSwitchMigrator sourceSwitchMigrator = new SourceSwitchMigrator(migratorTOneV1Reader);
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
