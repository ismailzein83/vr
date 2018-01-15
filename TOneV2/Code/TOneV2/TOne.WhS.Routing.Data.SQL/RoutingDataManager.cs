using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RoutingDataManager : BaseTOneDataManager, IRoutingDataManager
    {
        public RoutingDataManager() :
            base(GetConnectionStringName("TOneWhS_Routing_DBConnStringKey", "TOneWhS_Routing_DBConnString"))
        {

        }

        public RoutingDatabase RoutingDatabase { get; set; }

        string _databaseName;

        RoutingProcessType _routingProcessType;

        int _databaseId;

        /// <summary>
        /// Create Routing Database.
        /// </summary>
        internal string CreateDatabase(int databaseId, RoutingProcessType routingProcessType)
        {
            _databaseId = databaseId;
            _routingProcessType = routingProcessType;
            _databaseName = GetDatabaseName();
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.CreateDatabase(_databaseName, ConfigurationManager.AppSettings["RoutingDBDataFileDirectory"], ConfigurationManager.AppSettings["RoutingDBLogFileDirectory"]);
            switch (routingProcessType)
            {
                case RoutingProcessType.RoutingProductRoute:
                    CreateProductRoutingDatabaseSchema();
                    break;
                case RoutingProcessType.CustomerRoute:
                    CreateCustomerRoutingDatabaseSchema();
                    break;
            }
            return _databaseName;

        }

        public void FinalizeCustomerRouteDatabase(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            CustomerRouteDataManager customerRouteDataManager = new CustomerRouteDataManager() { RoutingDatabase = this.RoutingDatabase };
            customerRouteDataManager.FinalizeCurstomerRoute(trackStep, commandTimeoutInSeconds, maxDOP);
        }

        public void FinalizeRoutingProcess(IFinalizeRouteContext context, Action<string> trackStep)
        {

        }

        public void StoreCarrierAccounts(List<CarrierAccountInfo> carrierAccounts)
        {
            CarrierAccountDataManager carrierAccountDataManager = new CarrierAccountDataManager();
            carrierAccountDataManager.RoutingDatabase = this.RoutingDatabase;
            var dbApplyStream = carrierAccountDataManager.InitialiazeStreamForDBApply();
            foreach (var carrierAccount in carrierAccounts)
            {
                carrierAccountDataManager.WriteRecordToStream(carrierAccount, dbApplyStream);
            }
            var streamReadyToApply = carrierAccountDataManager.FinishDBApplyStream(dbApplyStream);
            carrierAccountDataManager.ApplyCarrierAccountsToTable(streamReadyToApply);
        }

        public void StoreSaleZones(List<SaleZone> saleZones)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            saleZoneDataManager.RoutingDatabase = this.RoutingDatabase;
            var dbApplyStream = saleZoneDataManager.InitialiazeStreamForDBApply();
            foreach (var saleZone in saleZones)
            {
                saleZoneDataManager.WriteRecordToStream(saleZone, dbApplyStream);
            }
            var streamReadyToApply = saleZoneDataManager.FinishDBApplyStream(dbApplyStream);
            saleZoneDataManager.ApplySaleZonesToTable(streamReadyToApply);
        }

        /// <summary>
        /// Drop Routing Database if database already exists.
        /// </summary>
        internal void DropDatabaseIfExists()
        {
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.DropDatabaseWithForceIfExists(RoutingDatabase.Settings.DatabaseName);
        }

        /// <summary>
        ///  Get Routing connection string by type.
        /// </summary>
        /// <returns>Routing Connection String</returns>
        protected override string GetConnectionString()
        {
            if (RoutingDatabase != null)
            {
                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(base.GetConnectionString());
                connectionBuilder.InitialCatalog = RoutingDatabase.Settings.DatabaseName;
                return connectionBuilder.ToString();
            }
            else
            {
                return base.GetConnectionString().Replace("#WorkflowType#", _routingProcessType.ToString()).Replace("#DatabaseId#", _databaseId.ToString());
            }
        }

        /// <summary>
        /// Get Name of Routing Database
        /// </summary>
        /// <returns>Database name</returns>
        private string GetDatabaseName()
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(this.GetConnectionString());
            return connStringBuilder.InitialCatalog;
        }

        private void CreateCustomerRoutingDatabaseSchema()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_SupplierZoneDetailsTable);
            query.AppendLine(query_CustomerQualityConfigurationTable);
            query.AppendLine(query_CustomerZoneDetailTable);
            query.AppendLine(query_CustomerRouteTable);
            query.AppendLine(query_TableTypes);
            query.AppendLine(query_CustomerZoneType);
            query.AppendLine(query_CustomerZoneDetailType);
            query.AppendLine(query_SupplierZoneDetailType);
            query.AppendLine(query_CodeType);
            query.AppendLine(query_CustomerRouteType);
            query.AppendLine(query_SaleZoneTable);
            query.AppendLine(query_CarrierAccountTable);
            query.AppendLine(query_CodeSaleZoneMatchTable);
            query.AppendLine(query_CodeSupplierZoneMatchTable);
            query.AppendLine(query_RoutingEntityDetailsTable);
            query.AppendLine(query_SwitchSyncDataTable);
            ExecuteNonQueryText(query.ToString(), null);
        }

        private void CreateProductRoutingDatabaseSchema()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(query_SupplierZoneDetailsTable);
            query.AppendLine(query_RPQualityConfigurationTable);
            query.AppendLine(query_CustomerZoneDetailTable);
            query.AppendLine(query_CodeSaleZoneTable);
            query.AppendLine(query_TableTypes);
            query.AppendLine(query_RPZonesTableType);
            query.AppendLine(query_RoutingProductTable);
            query.AppendLine(query_CodeSaleZoneMatchTable);
            query.AppendLine(query_CodeSupplierZoneMatchTable);
            query.AppendLine(query_SaleZoneTable);
            ExecuteNonQueryText(query.ToString(), null);

            //CREATE FUNCTION must be the only statement in the batch
            ExecuteNonQueryText(query_ParseStringListFunction.ToString(), null);
        }

        #region Constants

        const string query_CustomerQualityConfigurationTable = @"CREATE TABLE [dbo].[QualityConfigurationData](
                                                        [QualityConfigurationId] [uniqueidentifier] NOT NULL,
                                                        [SupplierZoneId] [bigint] NOT NULL,
                                                        [Quality] [decimal](20, 8) NOT NULL
                                                        )ON [PRIMARY]";

        const string query_RPQualityConfigurationTable = @"CREATE TABLE [dbo].[QualityConfigurationData](
                                                        [QualityConfigurationId] [uniqueidentifier] NOT NULL,
                                                        [SaleZoneId] [bigint] NOT NULL,
                                                        [SupplierId] [int] NOT NULL,
                                                        [Quality] [decimal](20, 8) NOT NULL
                                                        )ON [PRIMARY]";

        const string query_SaleZoneTable = @"CREATE TABLE [dbo].[SaleZone](
	                                         [ID] [int] NOT NULL,
	                                         [Name] [varchar](max) NOT NULL,
                                             CONSTRAINT [PK_SaleZone] PRIMARY KEY CLUSTERED 
                                                  (
	                                                  [ID] ASC
                                                  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                             ) ON [PRIMARY]";

        const string query_CarrierAccountTable = @"CREATE TABLE [dbo].[CarrierAccount](
	                                               [ID] [int] NOT NULL,
	                                               [Name] [varchar](max) NOT NULL,
                                                   CONSTRAINT [PK_CarrierAccount] PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [ID] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                   ) ON [PRIMARY]";

        const string query_SupplierZoneDetailsTable = @"CREATE TABLE [dbo].[SupplierZoneDetail](
	                                                    [SupplierId] [int] NOT NULL,
	                                                    [SupplierZoneId] [bigint] NOT NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NOT NULL,
                                                        [SupplierServiceIds] [nvarchar](max) NULL,
                                                        [ExactSupplierServiceIds] [nvarchar](max) NULL,
                                                        [SupplierServiceWeight] int NOT NULL,
                                                        [SupplierRateId] bigint NOT NULL,
                                                        [SupplierRateEED] datetime NULL,
                                                        [VersionNumber] [int] NOT NULL
                                                        ) ON [PRIMARY];
                                                        
                                                        CREATE CLUSTERED INDEX [IX_SupplierZoneDetail_SupplierZoneId] ON [dbo].[SupplierZoneDetail] 
                                                        (
	                                                        [SupplierZoneId] ASC
                                                        );

                                                        CREATE NONCLUSTERED INDEX [IX_SupplierZoneDetail_VersionNumber] ON [dbo].[SupplierZoneDetail] 
                                                        (
	                                                        [VersionNumber] DESC
                                                        );";

        const string query_CustomerZoneDetailTable = @" CREATE TABLE [dbo].[CustomerZoneDetail](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [RoutingProductId] [int] NULL,
	                                                    [RoutingProductSource] [tinyint] NULL,
	                                                    [SellingProductId] [int] NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NULL,
	                                                    [RateSource] [tinyint] NULL,
                                                        [SaleZoneServiceIds] [nvarchar](max) NULL,
                                                        [VersionNumber] [int] NOT NULL
                                                        ) ON [PRIMARY];
                                                        CREATE CLUSTERED INDEX [IX_CustomerZoneDetail_SaleZoneId] ON [dbo].[CustomerZoneDetail] 
                                                        (
	                                                        [SaleZoneId] ASC
                                                        );
                                                        CREATE NONCLUSTERED INDEX [IX_CustomerZoneDetail_VersionNumber] ON [dbo].[CustomerZoneDetail] 
                                                        (
	                                                        [VersionNumber] DESC
                                                        );";

        const string query_CodeSaleZoneTable = @"CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]";

        const string query_CustomerRouteTable = @"CREATE TABLE [dbo].[CustomerRoute](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [IsBlocked] [bit] NOT NULL,
	                                                    [ExecutedRuleId] [int] NULL,
	                                                    [RouteOptions] [varchar](max) NULL,
                                                        [VersionNumber] [int] NOT NULL
                                                ) ON [PRIMARY] ";

        private const string query_CodeSaleZoneMatchTable = @"CREATE TABLE [dbo].[CodeSaleZoneMatch](
																[Code] [varchar](20) NOT NULL,
																[SellingNumberPlanID] [int] NOT NULL,
																[SaleZoneID] [bigint] NOT NULL,
                                                                [CodeMatch] [varchar](20) NOT NULL
															) ON [PRIMARY]
															CREATE CLUSTERED INDEX [IX_CodeSaleZoneMatch_Code] ON [dbo].[CodeSaleZoneMatch]
															(
																[Code] ASC
															);";

        private const string query_RoutingEntityDetailsTable = @"CREATE TABLE [dbo].[RoutingEntityDetails](
                                                                    [Type] [int] NOT NULL,
															    	[Info] [nvarchar](MAX) NOT NULL
															    ) ON [PRIMARY];";

        private const string query_SwitchSyncDataTable = @"CREATE TABLE [dbo].[SwitchSyncData](
																[SwitchId] [nvarchar](MAX) NOT NULL,
                                                                [LastVersionNumber] int NOT NULL
															) ON [PRIMARY];";


        private const string query_CodeSupplierZoneMatchTable = @"CREATE TABLE [dbo].[CodeSupplierZoneMatch](
																	[Code] [varchar](20) NOT NULL,
																	[SupplierID] [int] NOT NULL,
																	[SupplierZoneID] [bigint] NOT NULL,
                                                                    [CodeMatch] [varchar](20) NOT NULL
																) ON [PRIMARY]
																CREATE CLUSTERED INDEX [IX_CodeSupplierZoneMatch_Code] ON [dbo].[CodeSupplierZoneMatch]
																(
																	[Code] ASC
																);
                                                                CREATE NONCLUSTERED INDEX [IX_CodeSupplierZoneMatch_SupplierID] ON dbo.CodeSupplierZoneMatch
                                                                (
                                                                    [SupplierID] ASC
                                                                );";

        private const string query_ParseStringListFunction = @"CREATE Function [dbo].[ParseStringList] (@StringArray nvarchar(max))
																Returns @tbl_string Table  (ParsedString nvarchar(max))  As  

																BEGIN 

																DECLARE @end Int,
																		@start Int

																SET @stringArray =  @StringArray + ',' 
																SET @start=1
																SET @end=1

																WHILE @end<Len(@StringArray)
																	BEGIN
																		SET @end = CharIndex(',', @StringArray, @end)
																		INSERT INTO @tbl_string 
																			SELECT
																				Substring(@StringArray, @start, @end-@start)
																		SET @start=@end+1
																		SET @end = @end+1
																	END
																RETURN
																END";

        const string query_TableTypes = @"CREATE TYPE [LongIDType] AS TABLE(
	                                                    [ID] [bigint] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [ID] ASC
                                                    )WITH (IGNORE_DUP_KEY = OFF)
                                                    )";

        const string query_CustomerZoneType = @"CREATE TYPE [CustomerZoneType] AS TABLE(
	                                                    [CustomerId] [int] NOT NULL,
                                                        [SaleZoneId] [bigint] NOT NULL)";

        const string query_CodeType = @"CREATE TYPE [CodeType] AS TABLE(
	                                           [Code] [varchar](20) NOT NULL)";

        const string query_CustomerRouteType = @"CREATE TYPE [CustomerRouteType] AS TABLE(
	                                            [CustomerId] [int] NOT NULL,
	                                            [Code] [varchar](20) NOT NULL,
	                                            [SaleZoneId] [bigint] NOT NULL,
	                                            [IsBlocked] [bit] NOT NULL,
	                                            [ExecutedRuleId] [int] NULL,
	                                            [RouteOptions] [varchar](max) NULL,
	                                            [VersionNumber] [int] NULL)";


        const string query_CustomerZoneDetailType = @"CREATE TYPE [CustomerZoneDetailType] AS TABLE(
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [RoutingProductId] [int] NULL,
	                                                    [RoutingProductSource] [tinyint] NULL,
	                                                    [SellingProductId] [int] NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NULL,
	                                                    [RateSource] [tinyint] NULL,
                                                        [SaleZoneServiceIds] [nvarchar](max) NULL,
                                                        [VersionNumber] [int] NOT NULL)";

        const string query_SupplierZoneDetailType = @"CREATE TYPE [SupplierZoneDetailType] AS TABLE(
                                                        [SupplierId] [int] NOT NULL,
	                                                    [SupplierZoneId] [bigint] NOT NULL,
                                                        [EffectiveRateValue] [decimal](20, 8) NOT NULL,
                                                        [SupplierServiceIds] [nvarchar](max) NULL,
                                                        [ExactSupplierServiceIds] [nvarchar](max) NULL,
                                                        [SupplierServiceWeight] int NOT NULL,
                                                        [SupplierRateId] bigint NOT NULL,
                                                        [SupplierRateEED] datetime NULL,
                                                        [VersionNumber] [int] NOT NULL)";



        const string query_RPZonesTableType = @"CREATE TYPE [RPZonesType] AS TABLE(
                                                    [RoutingProductId] [int] NOT NULL,                                                    
                                                    [SaleZoneId] [bigint] NOT NULL)";

        const string query_RoutingProductTable = @"
                                                    Create Table ProductRoute(
                                                        RoutingProductId int Not Null,
                                                        SaleZoneId bigint Not Null,
                                                        SaleZoneServices varchar(max) NULL,
                                                        ExecutedRuleId int Null,
                                                        OptionsDetailsBySupplier nvarchar(max) NULL,
                                                        OptionsByPolicy nvarchar(max) NULL,
                                                        IsBlocked bit NULL
                                                    )ON [PRIMARY]";
        #endregion
    }
}