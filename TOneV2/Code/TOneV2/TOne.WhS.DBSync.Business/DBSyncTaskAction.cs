using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TOne.WhS.BusinessEntity.Data.SQL;
using TOne.WhS.BusinessEntity.MainExtensions;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskAction : SchedulerTaskAction
    {
        List<IDManagerEntity> _idManagerEntities = new List<IDManagerEntity>();

        MigrationContext _context;

        public DBSyncTaskAction()
        {
        }

        public DBSyncTaskAction(MigrationContext migrationContext)
        {
            _context = migrationContext;
        }

        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            if (_context == null)
                _context = new MigrationContext();

            //Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(task.OwnerId);
            _context.WriteInformation("Database Sync Task Action Started");
            DBSyncTaskActionArgument dbSyncTaskActionArgument = taskActionArgument as DBSyncTaskActionArgument;
            if (dbSyncTaskActionArgument.DefaultRate <= 0)
                throw new ArgumentException("Default Rate should be greater than zero");

            _context.DefaultRate = dbSyncTaskActionArgument.DefaultRate;
            _context.UseTempTables = dbSyncTaskActionArgument.UseTempTables;
            _context.ConnectionString = dbSyncTaskActionArgument.ConnectionString;
            _context.DefaultSellingNumberPlanId = dbSyncTaskActionArgument.DefaultSellingNumberPlanId;
            _context.SellingProductId = dbSyncTaskActionArgument.SellingProductId;
            _context.OffPeakRateTypeId = dbSyncTaskActionArgument.OffPeakRateTypeId;
            _context.WeekendRateTypeId = dbSyncTaskActionArgument.WeekendRateTypeId;
            _context.HolidayRateTypeId = dbSyncTaskActionArgument.HolidayRateTypeId;
            _context.MigratePriceListData = dbSyncTaskActionArgument.MigratePriceListData;
            _context.OnlyEffective = dbSyncTaskActionArgument.OnlyEffective;
            _context.MigrationRequestedTables = dbSyncTaskActionArgument.MigrationRequestedTables;
            _context.IsCustomerCommissionNegative = dbSyncTaskActionArgument.IsCustomerCommissionNegative;
            _context.EffectiveAfterDate = dbSyncTaskActionArgument.EffectiveAfter;
            _context.ParameterDefinitions = dbSyncTaskActionArgument.ParameterDefinitions == null ? new Dictionary<string, ParameterValue>() : dbSyncTaskActionArgument.ParameterDefinitions;
            _context.DBTables = FillDBTables(_context);

            MigrationManager migrationManager = ConstructMigrationManager(_context);
            PrepareBeforeApplyingRecords(_context, migrationManager);

            ApplyPreData();
            TransferData(_context);
            FinalizeMigration(_context, migrationManager);
            ApplyPostData();

            _context.WriteInformation("Database Sync Task Action Executed");
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private void ApplyPreData()
        {
            _context.WriteInformation("Start Applying Pre Data");
            List<long> fileIds = new List<long>();

            SalePriceListTemplateDataManager salePriceListTemplateDataManager = new SalePriceListTemplateDataManager();

            foreach (var priceListTemplate in salePriceListTemplateDataManager.GetAll())
            {
                var settings = priceListTemplate.Settings as BasicSalePriceListTemplateSettings;
                if (settings != null)
                    fileIds.Add(settings.TemplateFileId);
            }

            SettingManager settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType("VR_Common_CompanySettings");
            CompanySettings companySettingData = (CompanySettings)setting.Data;

            if (companySettingData != null)
            {
                foreach (var companySetting in companySettingData.Settings)
                {
                    fileIds.Add(companySetting.CompanyLogo);
                }
            }

            PreDataDBSyncDataManager dataManager = new PreDataDBSyncDataManager();
            List<VRFile> vrFiles = dataManager.GetExistingFiles(fileIds);

            FileDBSyncDataManager fileManager = new FileDBSyncDataManager(_context.UseTempTables);
            fileManager.InsertFiles(vrFiles);
            _context.WriteInformation("Finish Applying Pre Data");
        }

        private void ApplyPostData()
        {
            _context.WriteInformation("Start Applying Post Data");

            SettingManager settingManager = new SettingManager();
            Setting systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            CurrencySettingData currencySettingData = (CurrencySettingData)systemCurrencySetting.Data;

            if (_context.DBTables != null && _context.DBTables.ContainsKey(DBTableName.Rule))
            {
                RuleDBSyncDataManager ruleDBSyncDataManager = new RuleDBSyncDataManager();
                ruleDBSyncDataManager.DeleteRuleChangedTables();
            }

            PostDataDBSyncDataManager postDataDBSyncDataManager = new PostDataDBSyncDataManager();

            postDataDBSyncDataManager.FixSellingProductRates(_context.DefaultSellingNumberPlanId, _context.SellingProductId, currencySettingData.CurrencyId, _context.DefaultRate);

            _context.WriteInformation("Finish Applying Post Data");
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

                    case DBTableName.CarrierAccountStatusHistory:
                        iDBSyncDataManager = new CarrierAccountStatusHistoryDBSyncDataManager(context.UseTempTables);
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
                    case DBTableName.SaleEntityRoutingProduct:
                        iDBSyncDataManager = new SaleEntityRoutingProductDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.SaleZone:
                        iDBSyncDataManager = new SaleZoneDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.File:
                        iDBSyncDataManager = new FileDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.CustomerCountry:
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
                    case DBTableName.FinancialAccount:
                        iDBSyncDataManager = new FinancialAccountDBSyncDataManager(context.UseTempTables);
                        break;
                    case DBTableName.SwitchReleaseCause:
                        iDBSyncDataManager = new SwitchReleaseCauseDBSyncDataManager(context.UseTempTables);
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
            IMigrator migrator = null;
            switch ((DBTableName)System.Enum.Parse(typeof(DBTableName), table.Name))
            {
                case DBTableName.Currency:
                    migrator = new CurrencyMigrator(context);
                    break;

                case DBTableName.CurrencyExchangeRate:
                    migrator = new CurrencyExchangeRateMigrator(context);
                    break;

                case DBTableName.Country:
                    migrator = new CountryMigrator(context);
                    break;

                case DBTableName.CodeGroup:
                    migrator = new CodeGroupMigrator(context);
                    break;

                case DBTableName.CarrierProfile:
                    migrator = new CarrierProfileMigrator(context);
                    break;

                case DBTableName.CarrierAccount:
                    migrator = new CarrierAccountMigrator(context);
                    break;

                case DBTableName.CarrierAccountStatusHistory:
                    migrator = new CarrierAccountStatusHistoryMigrator(context);
                    break;

                case DBTableName.Switch:
                    migrator = new SwitchMigrator(context);
                    break;

                case DBTableName.SaleZone:
                    migrator = new SaleZoneMigrator(context);
                    break;

                case DBTableName.SupplierZone:
                    migrator = new SupplierZoneMigrator(context);
                    break;

                case DBTableName.SaleCode:
                    migrator = new SaleCodeMigrator(context);
                    break;

                case DBTableName.SupplierCode:
                    migrator = new SupplierCodeMigrator(context);
                    break;

                case DBTableName.SalePriceList:
                    migrator = new SalePriceListMigrator(context);
                    break;

                case DBTableName.SupplierPriceList:
                    migrator = new SupplierPriceListMigrator(context);
                    break;

                case DBTableName.SaleRate:
                    migrator = new SaleRateMigrator(context);
                    break;

                case DBTableName.SaleEntityRoutingProduct:
                    migrator = new SaleEntityRoutingProductMigrator(context);
                    break;

                case DBTableName.SupplierRate:
                    migrator = new SupplierRateMigrator(context);
                    break;

                case DBTableName.CustomerCountry:
                    migrator = new CustomerZoneMigrator(context);
                    break;

                case DBTableName.SwitchConnectivity:
                    migrator = new SwitchConnectivityMigrator(context);
                    break;

                case DBTableName.Rule:
                    migrator = new RuleMigrator(context);
                    break;
                case DBTableName.ZoneServiceConfig:
                    migrator = new FlaggedServiceMigrator(context);
                    break;

                case DBTableName.SupplierZoneService:
                    migrator = new SupplierZoneServicesMigrator(context);
                    break;

                case DBTableName.SaleEntityService:
                    migrator = new SaleZoneServicesMigrator(context);
                    break;

                case DBTableName.VRTimeZone:
                    migrator = new VRTimeZoneMigrator(context);
                    break;

                case DBTableName.FinancialAccount:
                    migrator = new FinancialAccountMigrator(context);
                    break;

                case DBTableName.SwitchReleaseCause:
                    migrator = new SwitchReleaseCauseMigrator(context);
                    break;

                //Default Case for Table names that do not require migrator
                default:
                    migrator = null;
                    break;
            }

            if (migrator != null)
            {
                if (table.MigrationRequested)
                {
                    migrator.Migrate(migrationInfoContext);
                    migrator.FillTableInfo(context.UseTempTables);
                }
                else
                {
                    migrator.FillTableInfo(false);
                }
            }
        }

        private void TransferData(MigrationContext context)
        {
            foreach (var dbTableNameValue in Enum.GetValues(typeof(DBTableName)))
            {
                MigrationInfoContext migrationContext = new MigrationInfoContext();

                DBTable dbTable = GetDBTableFromName(context, (DBTableName)dbTableNameValue);

                CallMigrator(context, dbTable, migrationContext);
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