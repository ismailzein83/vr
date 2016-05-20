using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Runtime.Entities;


namespace TOne.WhS.DBSync.Business
{

    public class DBSyncTaskAction : SchedulerTaskAction
    {


        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            MigrationContext context = new MigrationContext();
            try
            {
                DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;
                MigrationManager migrationManager;

                context.WriteInformation("Database Sync Task Action Started");
                context.UseTempTables = dbSyncTaskActionArgument.UseTempTables;
                context.ConnectionString = dbSyncTaskActionArgument.ConnectionString;
                context.DefaultSellingNumberPlanId = dbSyncTaskActionArgument.DefaultSellingNumberPlanId;

                Dictionary<DBTableName, DBTable> dtTables = new Dictionary<DBTableName, DBTable>();

                foreach (DBTableName table in Enum.GetValues(typeof(DBTableName)))
                {


                    switch (table)
                    {
                        case DBTableName.CarrierAccount:
                            CarrierAccountDBSyncDataManager carrierAccountDBSyncDataManager = new CarrierAccountDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, carrierAccountDBSyncDataManager.GetConnection(), carrierAccountDBSyncDataManager.GetSchema());
                            break;


                        case DBTableName.CarrierProfile:
                            CarrierProfileDBSyncDataManager carrierProfileDBSyncDataManager = new CarrierProfileDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, carrierProfileDBSyncDataManager.GetConnection(), carrierProfileDBSyncDataManager.GetSchema());
                            break;


                        case DBTableName.Currency:
                            CurrencyDBSyncDataManager currencyDBSyncDataManager = new CurrencyDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, currencyDBSyncDataManager.GetConnection(), currencyDBSyncDataManager.GetSchema());
                            break;


                        case DBTableName.CurrencyExchangeRate:
                            CurrencyExchangeRateDBSyncDataManager currencyExchangeRateDBSyncDataManager = new CurrencyExchangeRateDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, currencyExchangeRateDBSyncDataManager.GetConnection(), currencyExchangeRateDBSyncDataManager.GetSchema());
                            break;


                        case DBTableName.Switch:
                            SwitchDBSyncDataManager switchDBSyncDataManager = new SwitchDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, switchDBSyncDataManager.GetConnection(), switchDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.Country:
                            CountryDBSyncDataManager countryDBSyncDataManager = new CountryDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, countryDBSyncDataManager.GetConnection(), countryDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.CodeGroup:
                            CodeGroupDBSyncDataManager codeGroupDBSyncDataManager = new CodeGroupDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, codeGroupDBSyncDataManager.GetConnection(), codeGroupDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SupplierCode:
                            SupplierCodeDBSyncDataManager supplierCodeDBSyncDataManager = new SupplierCodeDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, supplierCodeDBSyncDataManager.GetConnection(), supplierCodeDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SupplierPriceList:
                            SupplierPriceListDBSyncDataManager supplierPriceListDBSyncDataManager = new SupplierPriceListDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, supplierPriceListDBSyncDataManager.GetConnection(), supplierPriceListDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SupplierRate:
                            SupplierRateDBSyncDataManager supplierRateDBSyncDataManager = new SupplierRateDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, supplierRateDBSyncDataManager.GetConnection(), supplierRateDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SupplierZone:
                            SupplierZoneDBSyncDataManager supplierZoneDBSyncDataManager = new SupplierZoneDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, supplierZoneDBSyncDataManager.GetConnection(), supplierZoneDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SaleCode:
                            SaleCodeDBSyncDataManager saleCodeDBSyncDataManager = new SaleCodeDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, saleCodeDBSyncDataManager.GetConnection(), saleCodeDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SalePriceList:
                            SalePriceListDBSyncDataManager salePriceListDBSyncDataManager = new SalePriceListDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, salePriceListDBSyncDataManager.GetConnection(), salePriceListDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SaleRate:
                            SaleRateDBSyncDataManager saleRateDBSyncDataManager = new SaleRateDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, saleRateDBSyncDataManager.GetConnection(), saleRateDBSyncDataManager.GetSchema());
                            break;

                        case DBTableName.SaleZone:
                            SaleZoneDBSyncDataManager saleZoneDBSyncDataManager = new SaleZoneDBSyncDataManager(context.UseTempTables);
                            AddDBTable(dtTables, table, saleZoneDBSyncDataManager.GetConnection(), saleZoneDBSyncDataManager.GetSchema());
                            break;

                    }

                }


                context.DBTables = dtTables;

                CurrencyDBSyncDataManager sampleDBSyncDataManager = new CurrencyDBSyncDataManager(context.UseTempTables);
                context.MigrationCredentials = GetMigrationCredential(sampleDBSyncDataManager.GetConnection());
                migrationManager = new MigrationManager(context);

                PrepareBeforeApplyingRecords(context, migrationManager);
                TransferData(context);
                FinalizeMigration(context, migrationManager);

                context.WriteInformation("Database Sync Task Action Executed");

            }

            catch (Exception ex)
            {
                context.WriteException(ex);
            }

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private void AddDBTable(Dictionary<DBTableName, DBTable> dtTables, DBTableName table, string connectionString, string schema)
        {
            dtTables.Add(table, new DBTable() { Name = Vanrise.Common.Utilities.GetEnumDescription(table), Schema = schema, Database = GetDatabaseName(connectionString) });
        }

        private string GetDatabaseName(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string database = builder.InitialCatalog;
            return database;
        }

        private MigrationCredentials GetMigrationCredential(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            MigrationCredentials migrationCredential = new MigrationCredentials
            {
                MigrationServer = builder.DataSource,
                MigrationServerUserID = builder.UserID,
                MigrationServerPassword = builder.Password
            };
            return migrationCredential;
        }


        private void FinalizeMigration(MigrationContext context, MigrationManager migrationManager)
        {
            if (context.UseTempTables)
            {
                context.WriteInformation("Finalizing Migration Started");
                migrationManager.FinalizeMigration();
                context.WriteInformation("Finalizing Migration Ended"); ;
            }
        }

        private void PrepareBeforeApplyingRecords(MigrationContext context, MigrationManager migrationManager)
        {
            if (context.UseTempTables)
            {
                context.WriteInformation("Prepare Database Before Applying Records Started");
                migrationManager.PrepareBeforeApplyingRecords();
                context.WriteInformation("Prepare Database Before Applying Records Ended");
            }
        }

        private void TransferData(MigrationContext context)
        {
            SwitchMigrator switchMigrator = new SwitchMigrator(context);
            switchMigrator.Migrate();

            CurrencyMigrator currencyMigrator = new CurrencyMigrator(context);
            currencyMigrator.Migrate();

            CurrencyExchangeRateMigrator currencyExchangeRateMigrator = new CurrencyExchangeRateMigrator(context);
            currencyExchangeRateMigrator.Migrate();

            CountryMigrator countryMigrator = new CountryMigrator(context);
            countryMigrator.Migrate();

            CodeGroupMigrator codeGroupMigrator = new CodeGroupMigrator(context);
            codeGroupMigrator.Migrate();

            CarrierProfileMigrator carrierProfileMigrator = new CarrierProfileMigrator(context);
            carrierProfileMigrator.Migrate();

            CarrierAccountMigrator carrierAccountMigrator = new CarrierAccountMigrator(context);
            carrierAccountMigrator.Migrate();

            SaleZoneMigrator saleZoneMigrator = new SaleZoneMigrator(context);
            saleZoneMigrator.Migrate();

            SupplierZoneMigrator supplierZoneMigrator = new SupplierZoneMigrator(context);
            supplierZoneMigrator.Migrate();

            SaleCodeMigrator saleCodeMigrator = new SaleCodeMigrator(context);
            saleCodeMigrator.Migrate();

            SupplierCodeMigrator supplierCodeMigrator = new SupplierCodeMigrator(context);
            supplierCodeMigrator.Migrate();

            SalePriceListMigrator salePriceListMigrator = new SalePriceListMigrator(context);
            salePriceListMigrator.Migrate();

            SupplierPriceListMigrator supplierPriceListMigrator = new SupplierPriceListMigrator(context);
            supplierPriceListMigrator.Migrate();

            //SupplierRateMigrator supplierRateMigrator = new SupplierRateMigrator(context);
            //supplierRateMigrator.Migrate();
            
            //SaleRateMigrator saleRateMigrator = new SaleRateMigrator(context);
            //saleRateMigrator.Migrate();

        }
    }
}
