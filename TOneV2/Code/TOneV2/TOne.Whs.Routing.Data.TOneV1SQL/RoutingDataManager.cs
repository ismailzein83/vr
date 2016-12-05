using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class RoutingDataManager : BaseTOneDataManager, IRoutingDataManager
    {
        public RoutingDataManager() :
            base(GetConnectionStringName("TOneV1RoutingDBConnStringKey", "TOneV1RoutingDBConnString"))
        {

        }

        public RoutingDatabase RoutingDatabase { get; set; }

        string _databaseName;

        /// <summary>
        /// Create Routing Database.
        /// </summary>
        internal string CreateDatabase(int databaseId, RoutingProcessType routingProcessType)
        {
            _databaseName = GetDatabaseName();
            switch (routingProcessType)
            {
                case RoutingProcessType.RoutingProductRoute:
                    throw new NotSupportedException("Product Cost Generation is not supported.");
                case RoutingProcessType.CustomerRoute:
                    DropCustomerRoutingTablesIfExists();
                    CreateCustomerRoutingDatabaseSchema();
                    break;
            }
            return _databaseName;
        }

        public void FinalizeCustomerRouteDatabase(Action<string> trackStep)
        {
            CustomerRouteDataManager customerRouteDataManager = new CustomerRouteDataManager();
            customerRouteDataManager.FinalizeCurstomerRoute(trackStep);
        }

        /// <summary>
        /// Drop Routing Database if database already exists.
        /// </summary>
        internal void DropDatabaseIfExists()
        {

        }
        protected Dictionary<string, int> _allZoneServiceConfigIds = new Dictionary<string, int>();
        protected int GetServiceFlag(HashSet<int> serviceIds, Dictionary<int, ZoneServiceConfig> allZoneServiceConfigs)
        {
            int serviceflag = 0;
            if (serviceIds != null)
            {
                HashSet<int> orderedServiceIds = serviceIds.OrderBy(itm => itm).ToHashSet();
                string serviceIdsAsString = string.Join<int>(",", orderedServiceIds);

                if (!_allZoneServiceConfigIds.TryGetValue(serviceIdsAsString, out serviceflag))
                {
                    foreach (int serviceId in serviceIds)
                    {
                        ZoneServiceConfig zoneServiceConfig = allZoneServiceConfigs.GetRecord(serviceId);
                        serviceflag |= int.Parse(zoneServiceConfig.SourceId);
                    }
                    _allZoneServiceConfigIds.Add(serviceIdsAsString, serviceflag);
                }
            }
            return serviceflag;
        }

        /// <summary>
        /// Get Name of Routing Database
        /// </summary>
        /// <returns>Database name</returns>
        private string GetDatabaseName()
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(base.GetConnectionString());
            return connStringBuilder.InitialCatalog;
        }
        private void CreateCustomerRoutingDatabaseSchema()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_SupplierZoneDetailsTable);
            query.AppendLine(query_CodeSaleZoneTable);
            query.AppendLine(query_TableTypes);
            query.AppendLine(query_CustomerRouteTempTable);
            query.AppendLine(query_CustomerZoneDetailTable);
            query.AppendLine(query_ZoneRateTempTable);
            query.AppendLine(query_CodeMatchTempTable);
            ExecuteNonQueryText(query.ToString(), null);
        }

        private void DropCustomerRoutingTablesIfExists()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_DropSupplierZoneDetailsTable);
            query.AppendLine(query_DropCodeSaleZoneTable);
            query.AppendLine(query_DropCustomerRouteTempTable);
            query.AppendLine(query_DropCustomerZoneDetailTable);
            query.AppendLine(query_DropZoneRateTempTable);
            query.AppendLine(query_DropCodeMatchTempTable);
            ExecuteNonQueryText(query.ToString(), null);
        }


        #region Constants
        protected const string query_DropSupplierZoneDetailsTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SupplierZoneDetail' AND TABLE_SCHEMA = 'dbo')
                                                                    drop table dbo.SupplierZoneDetail;";

        protected const string query_DropCodeSaleZoneTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CodeSaleZone' AND TABLE_SCHEMA = 'dbo')
                                                                drop table dbo.CodeSaleZone;";

        protected const string query_DropCustomerRouteTempTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Route_Temp' AND TABLE_SCHEMA = 'dbo')
                                                                  drop table dbo.Route_Temp;
                                                                  if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteOption_Temp' AND TABLE_SCHEMA = 'dbo')
                                                                  drop table dbo.RouteOption_Temp;";

        protected const string query_DropZoneRateTempTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ZoneRates_Temp' AND TABLE_SCHEMA = 'dbo')
                                                         drop table dbo.ZoneRates_Temp;";

        protected const string query_DropCodeMatchTempTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CodeMatch_Temp' AND TABLE_SCHEMA = 'dbo')
                                                         drop table dbo.CodeMatch_Temp;";

        

        protected const string query_DropCustomerZoneDetailTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CustomerZoneDetail' AND TABLE_SCHEMA = 'dbo')
                                                                    drop table dbo.CustomerZoneDetail;";

        const string query_CodeMatchTempTable = @"CREATE TABLE [dbo].[CodeMatch_Temp](
	                                              [Code] [varchar](30) NOT NULL,
	                                              [SupplierCodeID] [bigint] NOT NULL,
	                                              [SupplierZoneID] [int] NOT NULL,
	                                              [SupplierID] [varchar](5) NULL
                                                  ) ON [PRIMARY];";




        const string query_SupplierZoneDetailsTable = @"CREATE TABLE [dbo].[SupplierZoneDetail](
	                                                    [SupplierId] [int] NOT NULL,
	                                                    [SupplierZoneId] [bigint] NOT NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NOT NULL,
                                                        [SupplierServiceIds] [nvarchar](max) NULL,
                                                        [ExactSupplierServiceIds] [nvarchar](max) NULL,
                                                        [SupplierServiceWeight] int NOT NULL
                                                        ) ON [PRIMARY];
                                                        
                                                        CREATE CLUSTERED INDEX [IX_SupplierZoneDetail_SupplierZoneId] ON [dbo].[SupplierZoneDetail] 
                                                        (
	                                                        [SupplierZoneId] ASC
                                                        );";


        const string query_CustomerZoneDetailTable = @" CREATE TABLE [dbo].[CustomerZoneDetail](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [RoutingProductId] [int] NULL,
	                                                    [RoutingProductSource] [tinyint] NULL,
	                                                    [SellingProductId] [int] NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NULL,
	                                                    [RateSource] [tinyint] NULL,
                                                        [SaleZoneServiceIds] [nvarchar](max) NULL
                                                        ) ON [PRIMARY];
                                                        CREATE CLUSTERED INDEX [IX_CustomerZoneDetail_SaleZoneId] ON [dbo].[CustomerZoneDetail] 
                                                        (
	                                                        [SaleZoneId] ASC
                                                        );";


        const string query_CodeSaleZoneTable = @"CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]";

        const string query_ZoneRateTempTable = @"CREATE TABLE [dbo].[ZoneRates_Temp](
	                                            [ZoneID] [int] NOT NULL,
	                                            [SupplierID] [varchar](5) NOT NULL,
	                                            [CustomerID] [varchar](5) NOT NULL,
	                                            [ServicesFlag] [smallint] NULL,
	                                            [ProfileId] [int] NULL,
	                                            [ActiveRate] [real] NULL,
	                                            [IsTOD] [bit] NULL,
	                                            [IsBlock] [bit] NULL,
	                                            [CodeGroup] [varchar](20) NULL
                                            ) ON [PRIMARY]";

        const string query_CustomerRouteTempTable = @"  CREATE TABLE [dbo].[Route_Temp](
	                                                [RouteID] [int] NOT NULL,
	                                                [CustomerID] [varchar](5) NOT NULL,
	                                                [ProfileID] [int] NULL,
	                                                [Code] [varchar](15) NULL,
	                                                [OurZoneID] [int] NULL,
	                                                [OurActiveRate] [real] NULL,
	                                                [OurServicesFlag] [smallint] NULL,
	                                                [State] [tinyint] NOT NULL,
	                                                [Updated] [datetime] NULL,
	                                                [IsToDAffected] [bit] NOT NULL,
	                                                [IsSpecialRequestAffected] [bit] NOT NULL,
	                                                [IsOverrideAffected] [bit] NOT NULL,
	                                                [IsBlockAffected] [bit] NOT NULL,
	                                                [IsOptionBlock] [bit] NOT NULL,
	                                                [BatchID] [int] NOT NULL
                                                ) ON [PRIMARY];

                                                CREATE TABLE [dbo].[RouteOption_Temp](
	                                                [RouteID] [int] NOT NULL,
	                                                [SupplierID] [varchar](5) NOT NULL,
	                                                [SupplierZoneID] [int] NULL,
	                                                [SupplierActiveRate] [real] NULL,
	                                                [SupplierServicesFlag] [smallint] NULL,
	                                                [Priority] [tinyint] NOT NULL,
	                                                [NumberOfTries] [tinyint] NULL,
	                                                [State] [tinyint] NOT NULL,
	                                                [Updated] [datetime] NULL,
	                                                [Percentage] [tinyint] NULL
                                                ) ON [PRIMARY];";


        const string query_TableTypes = @"IF not EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'LongIDType')
                                          CREATE TYPE [LongIDType] AS TABLE(
	                                     [ID] [bigint] NOT NULL,
	                                     PRIMARY KEY CLUSTERED 
                                         (
                                             [ID] ASC
                                         )WITH (IGNORE_DUP_KEY = OFF)
                                         )";
        #endregion
    }
}