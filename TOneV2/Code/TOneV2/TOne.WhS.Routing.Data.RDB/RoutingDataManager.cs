using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RoutingDataManager : IRoutingDataManager
    {
        int _databaseId;
        string _databaseName;
        RoutingProcessType _routingProcessType;

        public RoutingDatabase RoutingDatabase { get; set; }

        #region Public/Internal

        internal string CreateDatabase(int databaseId, RoutingProcessType routingProcessType, IEnumerable<RoutingCustomerInfo> routingCustomerInfos)
        {
            _databaseId = databaseId;
            _routingProcessType = routingProcessType;

            RDBQueryContext queryContext = new RDBQueryContext(this.GetDataProvider());
            _databaseName = queryContext.GetDataBaseName(new GetDataBaseNameInput());

            CreateDatabaseInput createDatabaseInput = new CreateDatabaseInput()
            {
                DatabaseName = _databaseName,
                DataFileDirectory = ConfigurationManager.AppSettings["RoutingDBDataFileDirectory"],
                LogFileDirectory = ConfigurationManager.AppSettings["RoutingDBLogFileDirectory"]
            };
            var createDBQueryContext = new RDBQueryContext(RDBDataProviderFactory.CreateProvider("TOneWhS_Routing", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"));
            createDBQueryContext.CreateDatabase(createDatabaseInput);

            switch (routingProcessType)
            {
                case RoutingProcessType.RoutingProductRoute:
                    CreateProductRoutingDatabaseSchema(routingCustomerInfos);
                    break;

                case RoutingProcessType.CustomerRoute:
                    CreateCustomerRoutingDatabaseSchema();
                    break;
            }

            return _databaseName;
        }

        public void StoreCarrierAccounts(List<CarrierAccountInfo> carrierAccounts)
        {
            CarrierAccountDataManager carrierAccountDataManager = new CarrierAccountDataManager();
            carrierAccountDataManager.RoutingDatabase = this.RoutingDatabase;
            var dbApplyStream = carrierAccountDataManager.InitialiazeStreamForDBApply();

            foreach (var carrierAccount in carrierAccounts)
                carrierAccountDataManager.WriteRecordToStream(carrierAccount, dbApplyStream);

            var streamReadyToApply = carrierAccountDataManager.FinishDBApplyStream(dbApplyStream);
            carrierAccountDataManager.ApplyCarrierAccountsToTable(streamReadyToApply);
        }

        public void StoreSaleZones(List<SaleZone> saleZones)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            saleZoneDataManager.RoutingDatabase = this.RoutingDatabase;
            var dbApplyStream = saleZoneDataManager.InitialiazeStreamForDBApply();

            foreach (var saleZone in saleZones)
                saleZoneDataManager.WriteRecordToStream(saleZone, dbApplyStream);

            var streamReadyToApply = saleZoneDataManager.FinishDBApplyStream(dbApplyStream);
            saleZoneDataManager.ApplySaleZonesToTable(streamReadyToApply);
        }

        public void FinalizeCustomerRouteDatabase(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            CustomerRouteDataManager customerRouteDataManager = new CustomerRouteDataManager() { RoutingDatabase = this.RoutingDatabase };
            customerRouteDataManager.FinalizeCurstomerRoute(trackStep, commandTimeoutInSeconds, maxDOP);
        }

        public void FinalizeRoutingProcess(IFinalizeRouteContext context, Action<string> trackStep)
        {
        }

        internal void DropDatabaseIfExists()
        {
            RDBQueryContext queryContext = new RDBQueryContext(this.GetDataProvider());
            DropDatabaseInput dropDatabaseInput = new DropDatabaseInput() { DatabaseName = RoutingDatabase.Settings.DatabaseName };
            queryContext.DropDatabase(dropDatabaseInput);
        }

        #endregion

        #region Private/Protected

        private void CreateCustomerRoutingDatabaseSchema()
        {
            var queryContext = new RDBQueryContext(this.GetDataProvider());

            CreateSaleZoneTable(queryContext);
            CreateCarrierAccountTable(queryContext);
            CreateCodeSaleZoneMatchTable(queryContext);
            CreateCodeSupplierZoneMatchTable(queryContext);
            CreateRoutingEntityDetailsTable(queryContext);
            CreateSwitchSyncDataTable(queryContext);

            CreateCustomerZoneDetailTable(queryContext);
            CreateSupplierZoneDetailTable(queryContext);
            CreateCustomerQualityConfigurationDataTable(queryContext);
            CreateCustomerRouteTable(queryContext);
            CreateModifiedCustomerRouteTable(queryContext);

            queryContext.ExecuteNonQuery();
        }

        private void CreateProductRoutingDatabaseSchema(IEnumerable<RoutingCustomerInfo> routingCustomerInfos)
        {
            var queryContext = new RDBQueryContext(this.GetDataProvider());

            CreateSaleZoneTable(queryContext);
            CreateCodeSaleZoneTable(queryContext);
            CreateZoneCodeGroupTable(queryContext);
            CreateCodeSaleZoneMatchTable(queryContext);
            CreateCodeSupplierZoneMatchTable(queryContext);

            CreateCustomerZoneDetailTable(queryContext);
            CreateSupplierZoneDetailTable(queryContext);
            CreateCustomerQualityConfigurationDataTable(queryContext);
            CreateRPQualityConfigurationDataTable(queryContext);
            CreateProductRouteTable(queryContext);

            if (routingCustomerInfos != null && routingCustomerInfos.Count() > 0)
            {
                foreach (var customer in routingCustomerInfos)
                    CreateProductRouteByCustomerTable(queryContext, customer.CustomerId);
            }

            queryContext.ExecuteNonQuery();
        }

        private void CreateCarrierAccountTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CarrierAccountDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CarrierAccountDataManager.s_CarrierAccountColumnDefinitions);
        }

        private void CreateSaleZoneTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(SaleZoneDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, SaleZoneDataManager.s_SaleZoneColumnDefinitions);
        }

        private void CreateCodeSaleZoneTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CodeSaleZoneDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CodeSaleZoneDataManager.s_CodeSaleZoneColumnDefinitions);
        }

        private void CreateZoneCodeGroupTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(RPZoneCodeGroupDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, RPZoneCodeGroupDataManager.s_ZoneCodeGroupColumnDefinitions);
        }

        private void CreateCodeSaleZoneMatchTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CodeSaleZoneMatchDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CodeSaleZoneMatchDataManager.s_CodeSaleZoneMatchColumnDefinitions);

            var createCodeClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCodeClusteredIndexQuery.DBTableName(CodeSaleZoneMatchDataManager.DBTABLE_NAME);
            createCodeClusteredIndexQuery.IndexName("IX_CodeSaleZoneMatch_Code");
            createCodeClusteredIndexQuery.IndexType(RDBCreateIndexType.Clustered);
            createCodeClusteredIndexQuery.AddColumn(CodeSaleZoneMatchDataManager.COL_Code);
        }

        private void CreateCodeSupplierZoneMatchTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CodeSupplierZoneMatchDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CodeSupplierZoneMatchDataManager.s_CodeSupplierZoneMatchColumnDefinitions);

            var createCodeClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCodeClusteredIndexQuery.DBTableName(CodeSupplierZoneMatchDataManager.DBTABLE_NAME);
            createCodeClusteredIndexQuery.IndexName("IX_CodeSupplierZoneMatch_Code");
            createCodeClusteredIndexQuery.IndexType(RDBCreateIndexType.Clustered);
            createCodeClusteredIndexQuery.AddColumn(CodeSupplierZoneMatchDataManager.COL_Code);

            var createSupplierIdNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSupplierIdNonClusteredIndexQuery.DBTableName(CodeSupplierZoneMatchDataManager.DBTABLE_NAME);
            createSupplierIdNonClusteredIndexQuery.IndexName("IX_CodeSupplierZoneMatch_SupplierID");
            createSupplierIdNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSupplierIdNonClusteredIndexQuery.AddColumn(CodeSupplierZoneMatchDataManager.COL_SupplierID);

            //private const string query_CodeSupplierZoneMatchTable = @"CREATE TABLE [dbo].[CodeSupplierZoneMatch](
            //	                                                        [Code] [varchar](20) NOT NULL,
            //	                                                        [SupplierID] [int] NOT NULL,
            //	                                                        [SupplierZoneID] [bigint] NOT NULL,
            //                                                          [CodeMatch] [varchar](20) NOT NULL
            //                                                          ) ON [PRIMARY]
            //                                                          CREATE CLUSTERED INDEX [IX_CodeSupplierZoneMatch_Code] ON [dbo].[CodeSupplierZoneMatch]
            //                                                          (
            //                                                          	[Code] ASC
            //                                                          );
            //                                                          CREATE NONCLUSTERED INDEX [IX_CodeSupplierZoneMatch_SupplierID] ON dbo.CodeSupplierZoneMatch
            //                                                          (
            //                                                              [SupplierID] ASC
            //                                                          );";      
        }

        private void CreateRoutingEntityDetailsTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(RoutingEntityDetailsDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, RoutingEntityDetailsDataManager.s_RoutingEntityDetailsColumnDefinitions);
        }

        private void CreateSwitchSyncDataTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(SwitchSyncDataDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, SwitchSyncDataDataManager.s_SwitchSyncDataColumnDefinitions);
        }

        private void CreateCustomerZoneDetailTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CustomerZoneDetailsDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CustomerZoneDetailsDataManager.s_CustomerZoneDetailsColumnDefinitions);

            var createSaleZoneIdClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSaleZoneIdClusteredIndexQuery.DBTableName(CustomerZoneDetailsDataManager.DBTABLE_NAME);
            createSaleZoneIdClusteredIndexQuery.IndexName("IX_CustomerZoneDetail_SaleZoneId");
            createSaleZoneIdClusteredIndexQuery.IndexType(RDBCreateIndexType.Clustered);
            createSaleZoneIdClusteredIndexQuery.AddColumn(CustomerZoneDetailsDataManager.COL_SaleZoneId);

            var createVersionNumberNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createVersionNumberNonClusteredIndexQuery.DBTableName(CustomerZoneDetailsDataManager.DBTABLE_NAME);
            createVersionNumberNonClusteredIndexQuery.IndexName("IX_CustomerZoneDetail_VersionNumber");
            createVersionNumberNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createVersionNumberNonClusteredIndexQuery.AddColumn(CustomerZoneDetailsDataManager.COL_VersionNumber, RDBCreateIndexDirection.DESC);
        }

        private void CreateSupplierZoneDetailTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(SupplierZoneDetailsDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, SupplierZoneDetailsDataManager.s_SupplierZoneDetailColumnDefinitions);

            var createSupplierZoneIdClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSupplierZoneIdClusteredIndexQuery.DBTableName(SupplierZoneDetailsDataManager.DBTABLE_NAME);
            createSupplierZoneIdClusteredIndexQuery.IndexName("IX_SupplierZoneDetail_SupplierZoneId");
            createSupplierZoneIdClusteredIndexQuery.IndexType(RDBCreateIndexType.Clustered);
            createSupplierZoneIdClusteredIndexQuery.AddColumn(SupplierZoneDetailsDataManager.COL_SupplierZoneId);

            var createVersionNumberNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createVersionNumberNonClusteredIndexQuery.DBTableName(SupplierZoneDetailsDataManager.DBTABLE_NAME);
            createVersionNumberNonClusteredIndexQuery.IndexName("IX_SupplierZoneDetail_VersionNumber");
            createVersionNumberNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createVersionNumberNonClusteredIndexQuery.AddColumn(SupplierZoneDetailsDataManager.COL_VersionNumber, RDBCreateIndexDirection.DESC);
        }

        private void CreateCustomerQualityConfigurationDataTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CustomerQualityConfigurationDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CustomerQualityConfigurationDataManager.s_CustomerQualityConfigurationDataColumnDefinitions);

            var createVersionNumberNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createVersionNumberNonClusteredIndexQuery.DBTableName(CustomerQualityConfigurationDataManager.DBTABLE_NAME);
            createVersionNumberNonClusteredIndexQuery.IndexName("IX_CustomerQualityConfigurationData_VersionNumber");
            createVersionNumberNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createVersionNumberNonClusteredIndexQuery.AddColumn(CustomerQualityConfigurationDataManager.COL_VersionNumber, RDBCreateIndexDirection.DESC);
        }

        private void CreateRPQualityConfigurationDataTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(RPQualityConfigurationDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, RPQualityConfigurationDataManager.s_RPQualityConfigurationDataColumnDefinitions);

            var createVersionNumberNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createVersionNumberNonClusteredIndexQuery.DBTableName(RPQualityConfigurationDataManager.DBTABLE_NAME);
            createVersionNumberNonClusteredIndexQuery.IndexName("IX_RPQualityConfigurationData_VersionNumber");
            createVersionNumberNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createVersionNumberNonClusteredIndexQuery.AddColumn(RPQualityConfigurationDataManager.COL_VersionNumber, RDBCreateIndexDirection.DESC);
        }

        private void CreateCustomerRouteTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(CustomerRouteDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, CustomerRouteDataManager.s_CustomerRouteColumnDefinitions);
        }

        private void CreateProductRouteTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(RPRouteDataManager.RP_DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, RPRouteDataManager.s_ProductRouteRoutingTableColumnDefinitions);
        }

        private void CreateProductRouteByCustomerTable(RDBQueryContext queryContext, int customerId)
        {
            RPRouteDataManager.RegisterProductRouteByCustomerTableDefinition(customerId);

            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(RPRouteDataManager.RPByCustomer_DBTABLE_NAME.Replace("#CustomerId#", customerId.ToString()));
            Helper.AddRoutingTableColumns(createTableQuery, RPRouteDataManager.s_ProductRouteByCustomerRoutingTableColumnDefinitions);
        }

        private void CreateModifiedCustomerRouteTable(RDBQueryContext queryContext)
        {
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(ModifiedCustomerRouteDataManager.DBTABLE_NAME);
            Helper.AddRoutingTableColumns(createTableQuery, ModifiedCustomerRouteDataManager.s_ModifiedCustomerRouteColumnDefinitions);

            var createVersionNumberNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createVersionNumberNonClusteredIndexQuery.DBTableName(ModifiedCustomerRouteDataManager.DBTABLE_NAME);
            createVersionNumberNonClusteredIndexQuery.IndexName("IX_ModifiedCustomerRoute_CustomerId_Code");
            createVersionNumberNonClusteredIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createVersionNumberNonClusteredIndexQuery.AddColumn(ModifiedCustomerRouteDataManager.COL_CustomerId);
            createVersionNumberNonClusteredIndexQuery.AddColumn(ModifiedCustomerRouteDataManager.COL_Code);
        }

        protected BaseRDBDataProvider GetDataProvider()
        {
            BaseRDBDataProvider tempBaseRDBDataProvider = RDBDataProviderFactory.CreateProvider("WhS_Routing", "TOneWhS_Routing_DBConnStringKey", "TOneWhS_Routing_DBConnString");
            RDBQueryContext queryContext = new RDBQueryContext(tempBaseRDBDataProvider);

            string connectionString;
            if (RoutingDatabase != null)
                connectionString = queryContext.GetOverridenConnectionString(new GetOverridenConnectionStringInput() { OverridingDatabaseName = RoutingDatabase.Settings.DatabaseName });
            else
                connectionString = queryContext.GetConnectionString().Replace("#WorkflowType#", _routingProcessType.ToString()).Replace("#DatabaseId#", _databaseId.ToString());

            return RDBDataProviderFactory.CreateProviderFromConnString("WhS_Routing", connectionString);
        }

        #endregion
    }
}