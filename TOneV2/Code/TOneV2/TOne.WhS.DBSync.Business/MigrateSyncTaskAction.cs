using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Runtime.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class MigrateSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            MigrateSyncTaskActionArgument migrateSyncTaskActionArgument = taskActionArgument as MigrateSyncTaskActionArgument;


            // Create Source Switch Reader and Pass Connection string to it.
            SourceSwitchMigratorReader sourceSwitchMigratorReader = new SourceSwitchMigratorReader();
            sourceSwitchMigratorReader.ConnectionString = migrateSyncTaskActionArgument.ConnectionString;

            // Create Source Switch Migrator and Migrate
            SourceSwitchMigrator sourceSwitchMigrator = new SourceSwitchMigrator(sourceSwitchMigratorReader);
            sourceSwitchMigrator.Migrate();




            // Create Source Currency Reader and Pass Connection string to it.
            SourceCurrencyMigratorReader sourceCurrencyMigratorReader = new SourceCurrencyMigratorReader();
            sourceCurrencyMigratorReader.ConnectionString = migrateSyncTaskActionArgument.ConnectionString;

            // Create Source Currency Migrator and Migrate
            SourceCurrencyMigrator sourceCurrencyMigrator = new SourceCurrencyMigrator(sourceCurrencyMigratorReader);
            sourceCurrencyMigrator.Migrate();





            // Create Source CurrencyExchangeRate Reader and Pass Connection string to it.
            SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader = new SourceCurrencyExchangeRateMigratorReader();
            sourceCurrencyExchangeRateMigratorReader.ConnectionString = migrateSyncTaskActionArgument.ConnectionString;

            // Create Source CurrencyExchangeRate Migrator and Migrate
            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator = new SourceCurrencyExchangeRateMigrator(sourceCurrencyExchangeRateMigratorReader);
            sourceCurrencyExchangeRateMigrator.Migrate();


            MigrationManager migrationManager = new MigrationManager();
            migrationManager.ExecuteMigration();
            

            Console.WriteLine("MigrationSyncTaskAction Executed");
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        



    }


}
