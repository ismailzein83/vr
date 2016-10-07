using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Common;


namespace TOne.WhS.DBSync.Business
{

    public class DBSyncTaskAction : SchedulerTaskAction
    {
        List<IDManagerEntity> _idManagerEntities = new List<IDManagerEntity>();

        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            MigrationContext context = new MigrationContext();
            try
            {
                context.WriteInformation("Database Sync Task Action Started");
                DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;
                MigrationManager migrationManager;
                context.UseTempTables = dbSyncTaskActionArgument.UseTempTables;
                context.ConnectionString = dbSyncTaskActionArgument.ConnectionString;
                context.DefaultSellingNumberPlanId = dbSyncTaskActionArgument.DefaultSellingNumberPlanId;
                context.SellingProductId = dbSyncTaskActionArgument.SellingProductId;
                context.OffPeakRateTypeId = dbSyncTaskActionArgument.OffPeakRateTypeId;
                context.WeekendRateTypeId = dbSyncTaskActionArgument.WeekendRateTypeId;
                context.MigratePriceListData = dbSyncTaskActionArgument.MigratePriceListData;
                context.OnlyEffective = dbSyncTaskActionArgument.OnlyEffective;
                context.MigrationRequestedTables = dbSyncTaskActionArgument.MigrationRequestedTables;
                context.DBTables = FillDBTables(context);
                migrationManager = ConstructMigrationManager(context);
                PrepareBeforeApplyingRecords(context, migrationManager);
                //TruncateTables(context, migrationManager);
                TransferData(context);
                //CreateForeignKeys(context, migrationManager);
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

        private MigrationManager ConstructMigrationManager(MigrationContext context)
        {
            MigrationManager migrationManager;
            CurrencyDBSyncDataManager sampleDBSyncDataManager = new CurrencyDBSyncDataManager(context.UseTempTables);
            context.MigrationCredentials = GetMigrationCredential(sampleDBSyncDataManager.GetConnection());
            migrationManager = new MigrationManager(context);
            return migrationManager;
        }

        private Dictionary<DBTableName, DBTable> FillDBTables(MigrationContext context)
        {
            Dictionary<DBTableName, DBTable> dtTables = new Dictionary<DBTableName, DBTable>();

            IDBSyncDataManager iDBSyncDataManager = null;
            bool migrationRequested = false;
            foreach (DBTableName table in Enum.GetValues(typeof(DBTableName)))
            {
                migrationRequested = context.MigrationRequestedTables.Contains(table);
                switch (table)
                {
                    case DBTableName.CarrierAccount:
                        iDBSyncDataManager = new CarrierAccountDBSyncDataManager(context.UseTempTables);
                        break;


                    case DBTableName.CarrierProfile:
                        iDBSyncDataManager = new CarrierProfileDBSyncDataManager(context.UseTempTables);
                        break;


                    case DBTableName.Currency:
                        iDBSyncDataManager = new CurrencyDBSyncDataManager(context.UseTempTables);
                        break;


                    case DBTableName.CurrencyExchangeRate:
                        iDBSyncDataManager = new CurrencyExchangeRateDBSyncDataManager(context.UseTempTables);
                        break;


                    case DBTableName.Switch:
                        iDBSyncDataManager = new SwitchDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.Country:
                        iDBSyncDataManager = new CountryDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.CodeGroup:
                        iDBSyncDataManager = new CodeGroupDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SupplierCode:
                        iDBSyncDataManager = new SupplierCodeDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SupplierPriceList:
                        iDBSyncDataManager = new SupplierPriceListDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SupplierRate:
                        iDBSyncDataManager = new SupplierRateDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SupplierZone:
                        iDBSyncDataManager = new SupplierZoneDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SaleCode:
                        iDBSyncDataManager = new SaleCodeDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SalePriceList:
                        iDBSyncDataManager = new SalePriceListDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SaleRate:
                        iDBSyncDataManager = new SaleRateDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.SaleZone:
                        iDBSyncDataManager = new SaleZoneDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.File:
                        iDBSyncDataManager = new FileDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.CustomerZone:
                        iDBSyncDataManager = new CustomerZoneDBSyncDataManager(context.UseTempTables, context.SellingProductId);
                        break;

                    case DBTableName.SwitchConnectivity:
                        iDBSyncDataManager = new SwitchConnectivityDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.Rule:
                        iDBSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.ZoneServiceConfig:
                        iDBSyncDataManager = new ZoneServiceConfigDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.SupplierZoneService:
                        iDBSyncDataManager = new SupplierZoneServicesDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.SaleEntityService:
                        iDBSyncDataManager = new SaleZoneServicesDBSyncDataManager(context.UseTempTables);
                        break;

                    case DBTableName.VRTimeZone:
                        iDBSyncDataManager = new VRTimeZoneDBSyncDataManager(context.UseTempTables);
                        break;

                }
                AddDBTable(dtTables, table, iDBSyncDataManager.GetConnection(), iDBSyncDataManager.GetSchema(), migrationRequested);

            }
            return dtTables;
        }

        private void AddDBTable(Dictionary<DBTableName, DBTable> dtTables, DBTableName table, string connectionString, string schema, bool migrationRequested)
        {
            dtTables.Add(table, new DBTable() { Name = Vanrise.Common.Utilities.GetEnumDescription(table), Schema = schema, Database = GetDatabaseName(connectionString), MigrationRequested = migrationRequested });
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



        private void TruncateTables(MigrationContext context, MigrationManager migrationManager)
        {
            if (!context.UseTempTables)
            {
                migrationManager.TruncateTables();
            }
        }

        private void CreateForeignKeys(MigrationContext context, MigrationManager migrationManager)
        {
            if (!context.UseTempTables)
            {
                migrationManager.CreateForeignKeys();
            }
        }


        private void FinalizeMigration(MigrationContext context, MigrationManager migrationManager)
        {
            context.WriteInformation("Finalizing Migration Started");
            migrationManager.FinalizeMigration(context.UseTempTables, _idManagerEntities);
            context.WriteInformation("Finalizing Migration Ended");
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

        private void CallMigrator(MigrationContext context, DBTable table, MigrationInfoContext migrationInfoContext)
        {
            IMigrator imgrator = null;
            switch ((DBTableName)System.Enum.Parse(typeof(DBTableName), table.Name))
            {
                case DBTableName.Currency:
                    imgrator = new CurrencyMigrator(context);
                    break;

                case DBTableName.CurrencyExchangeRate:
                    imgrator = new CurrencyExchangeRateMigrator(context);
                    break;

                case DBTableName.Country:
                    imgrator = new CountryMigrator(context);
                    break;

                case DBTableName.CodeGroup:
                    imgrator = new CodeGroupMigrator(context);
                    break;

                case DBTableName.CarrierProfile:
                    imgrator = new CarrierProfileMigrator(context);
                    break;

                case DBTableName.CarrierAccount:
                    imgrator = new CarrierAccountMigrator(context);
                    break;

                case DBTableName.Switch:
                    imgrator = new SwitchMigrator(context);
                    break;

                case DBTableName.SaleZone:
                    imgrator = new SaleZoneMigrator(context);
                    break;

                case DBTableName.SupplierZone:
                    imgrator = new SupplierZoneMigrator(context);
                    break;

                case DBTableName.SaleCode:
                    imgrator = new SaleCodeMigrator(context);
                    break;

                case DBTableName.SupplierCode:
                    imgrator = new SupplierCodeMigrator(context);
                    break;

                case DBTableName.SalePriceList:
                    imgrator = new SalePriceListMigrator(context);
                    break;

                case DBTableName.SupplierPriceList:
                    imgrator = new SupplierPriceListMigrator(context);
                    break;

                case DBTableName.SaleRate:
                    imgrator = new SaleRateMigrator(context);
                    break;

                case DBTableName.SupplierRate:
                    imgrator = new SupplierRateMigrator(context);
                    break;

                case DBTableName.CustomerZone:
                    imgrator = new CustomerZoneMigrator(context);
                    break;

                case DBTableName.SwitchConnectivity:
                    imgrator = new SwitchConnectivityMigrator(context);
                    break;
                case DBTableName.Rule:
                    imgrator = new RuleMigrator(context);
                    break;
                case DBTableName.ZoneServiceConfig:
                    imgrator = new FlaggedServiceMigrator(context);
                    break;
                case DBTableName.SupplierZoneService:
                    imgrator = new  SupplierZoneServicesMigrator(context);
                    break;

                case DBTableName.SaleEntityService:
                    imgrator = new SaleZoneServicesMigrator(context);
                    break;

                case DBTableName.VRTimeZone:
                    imgrator = new VRTimeZoneMigrator(context);
                    break;
            }

            if (imgrator != null)
            {
                if (table.MigrationRequested)
                {
                    imgrator.Migrate(migrationInfoContext);
                    imgrator.FillTableInfo(context.UseTempTables);
                }
                else
                {
                    imgrator.FillTableInfo(false);
                }
            }
        }

        private void TransferData(MigrationContext context)
        {
            foreach (var dbTableNameValue in Enum.GetValues(typeof(DBTableName)))
            {
                MigrationInfoContext migrationContext = new MigrationInfoContext();
                CallMigrator(context, GetDBTableFromName(context, (DBTableName)dbTableNameValue), migrationContext);
                if (migrationContext.GeneratedIdsInfoContext != null)
                {
                    _idManagerEntities.Add(new IDManagerEntity() { LastTakenId = migrationContext.GeneratedIdsInfoContext.LastTakenId, TypeId = migrationContext.GeneratedIdsInfoContext.TypeId });
                }
            }
        }

        private static DBTable GetDBTableFromName(MigrationContext context, DBTableName dbTableName)
        {
            string tableName = Vanrise.Common.Utilities.GetEnumDescription(dbTableName);

            return context.DBTables.Values.FindRecord(item => item.Name.Equals(tableName));
        }
    }
}
