using System;
using System.Collections.Generic;
using System.Configuration;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Runtime.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            Console.WriteLine("Database Sync Task Action Started");

            List<DBTable> context = new List<DBTable>();
            context.Add(new DBTable { Name = "CurrencyExchangeRate", Schema = "Common", Database = "TOneConfiguration_Migration" });
            context.Add(new DBTable { Name = "Currency", Schema = "Common", Database = "TOneConfiguration_Migration" });
            context.Add(new DBTable { Name = "Switch", Schema = "TOneWhS_BE", Database = "TOneV2_Migration" });

            MigrationManager.MigrationCredentials migrationCredentials = new MigrationManager.MigrationCredentials();
            migrationCredentials.MigrationServer = ConfigurationManager.AppSettings["MigrationServer"];
            migrationCredentials.MigrationServerUserID = ConfigurationManager.AppSettings["MigrationServerUserID"];
            migrationCredentials.MigrationServerPassword = ConfigurationManager.AppSettings["MigrationServerPassword"];

            MigrationManager migrationManager = new MigrationManager(migrationCredentials, context);
            migrationManager.PrepareBeforeApplyingRecords();

            DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;

            SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(new SourceCurrencyMigratorReader(dbSyncTaskActionArgument.ConnectionString));
            sourceCurrencyMigrator.Migrate(context);

            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(new SourceCurrencyExchangeRateMigratorReader(dbSyncTaskActionArgument.ConnectionString));
            sourceCurrencyExchangeRateMigrator.Migrate(context);

            migrationManager.FinalizeMigration();

            Console.WriteLine("Database Sync Task Action Executed");
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }





    }


}
