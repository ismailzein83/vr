using System;
using System.Collections.Generic;
using System.Configuration;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.Runtime.Entities;


namespace TOne.WhS.DBSync.Business
{

    public class DBSyncTaskAction : SchedulerTaskAction
    {
        MigrationManager _MigrationManager;
        MigrationManager.MigrationCredentials _MigrationCredentials;
        DBSyncLogger _Logger = new DBSyncLogger();

        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            try
            {
                _Logger.WriteInformation("Database Sync Task Action Started");

                DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;

                bool useTempTables = dbSyncTaskActionArgument.UseTempTables;

                List<DBTable> context = new List<DBTable>();
                context.Add(new DBTable { Name = Constants.Table_CurrencyExchangeRate, Schema = Constants.SCHEMA_Common, Database = Constants.DB_TOneConfiguration_Migration });
                context.Add(new DBTable { Name = Constants.Table_Currency, Schema = Constants.SCHEMA_Common, Database = Constants.DB_TOneConfiguration_Migration });
                context.Add(new DBTable { Name = Constants.Table_Switch, Schema = Constants.SCHEMA_TOneWhS_BE, Database = Constants.DB_TOneV2_Migration });
                context.Add(new DBTable { Name = Constants.Table_CarrierProfile, Schema = Constants.SCHEMA_TOneWhS_BE, Database = Constants.DB_TOneV2_Migration });
                context.Add(new DBTable { Name = Constants.Table_CarrierAccount, Schema = Constants.SCHEMA_TOneWhS_BE, Database = Constants.DB_TOneV2_Migration });

                if (useTempTables)
                {
                    _Logger.WriteInformation("Prepare Database Before Applying Records Started");
                    _MigrationCredentials = new MigrationManager.MigrationCredentials();
                    _MigrationCredentials.MigrationServer = ConfigurationManager.AppSettings["MigrationServer"];
                    _MigrationCredentials.MigrationServerUserID = ConfigurationManager.AppSettings["MigrationServerUserID"];
                    _MigrationCredentials.MigrationServerPassword = ConfigurationManager.AppSettings["MigrationServerPassword"];
                    _MigrationManager = new MigrationManager(_MigrationCredentials, context);
                    _MigrationManager.PrepareBeforeApplyingRecords();
                    _Logger.WriteInformation("Prepare Database Before Applying Records Ended");
                }

                TransferData(dbSyncTaskActionArgument, useTempTables, context, _Logger);

                if (useTempTables)
                {
                    _Logger.WriteInformation("FinalizeMigration Started");
                    _MigrationManager.FinalizeMigration();
                    _Logger.WriteInformation("FinalizeMigration Ended"); ;
                }

                _Logger.WriteInformation("Database Sync Task Action Executed");

            }

            catch (Exception ex)
            {
                _Logger.WriteException(ex);
            }



            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private void TransferData(DBSyncTaskActionArgument dbSyncTaskActionArgument, bool useTempTables, List<DBTable> context, DBSyncLogger logger)
        {
            SourceCurrencyMigrator sourceCurrencyMigrator =
                new SourceCurrencyMigrator(dbSyncTaskActionArgument.ConnectionString, useTempTables, logger);
            sourceCurrencyMigrator.Migrate(context);

            SourceCurrencyExchangeRateMigrator sourceCurrencyExchangeRateMigrator =
                new SourceCurrencyExchangeRateMigrator(dbSyncTaskActionArgument.ConnectionString, useTempTables, logger);
            sourceCurrencyExchangeRateMigrator.Migrate(context);

            SourceSwitchMigrator sourceSwitchMigrator =
                new SourceSwitchMigrator(dbSyncTaskActionArgument.ConnectionString, useTempTables, logger);
            sourceSwitchMigrator.Migrate(context);

            SourceCarrierProfileMigrator sourceCarrierProfileMigrator =
               new SourceCarrierProfileMigrator(dbSyncTaskActionArgument.ConnectionString, useTempTables, logger);
            sourceCarrierProfileMigrator.Migrate(context);

            SourceCarrierAccountMigrator sourceCarrierAccountMigrator =
               new SourceCarrierAccountMigrator(dbSyncTaskActionArgument.ConnectionString, useTempTables, logger);
            sourceCarrierAccountMigrator.Migrate(context);
        }
    }
}
