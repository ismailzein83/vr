using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Runtime.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {

            Console.WriteLine("Database Sync Task Action Started");

            MigrationManager migrationManager = new MigrationManager();
            migrationManager.PrepareBeforeApplyingRecords();

            DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;


            // Create Source Currency Reader and Pass Connection string to it.
            SourceCurrencyMigratorReader sourceCurrencyMigratorReader = new SourceCurrencyMigratorReader();
            sourceCurrencyMigratorReader.ConnectionString = dbSyncTaskActionArgument.ConnectionString;

            // Create Source Currency Migrator and Migrate
            SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(sourceCurrencyMigratorReader);
            sourceCurrencyMigrator.Migrate();


            // Create Source CurrencyExchangeRate Reader and Pass Connection string to it.
            SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader = new SourceCurrencyExchangeRateMigratorReader();
            sourceCurrencyExchangeRateMigratorReader.ConnectionString = dbSyncTaskActionArgument.ConnectionString;

            // Create Source CurrencyExchangeRate Migrator and Migrate
            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(sourceCurrencyExchangeRateMigratorReader);
            sourceCurrencyExchangeRateMigrator.Migrate();

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
