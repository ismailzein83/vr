using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
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
            query.AppendLine(query_CodeSaleZoneTable);
            query.AppendLine(query_TableTypes);
            query.AppendLine(query_CustomerRouteTable);
            query.AppendLine(query_CustomerZoneDetailTable);
            ExecuteNonQueryText(query.ToString(), null);
        }

        private void CreateProductRoutingDatabaseSchema()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_SupplierZoneDetailsTable);
            query.AppendLine(query_CodeMatchTable);
            query.AppendLine(query_CodeSaleZoneTable);
            query.AppendLine(query_TableTypes);
            query.AppendLine(query_RPZonesTableType);
            query.AppendLine(query_RoutingProductTable);
            ExecuteNonQueryText(query.ToString(), null);
        }

        #region Constants


        const string query_SupplierZoneDetailsTable = @"CREATE TABLE [dbo].[SupplierZoneDetail](
	                                                    [SupplierId] [int] NOT NULL,
	                                                    [SupplierZoneId] [bigint] NOT NULL,
	                                                    [EffectiveRateValue] [decimal](20, 8) NOT NULL
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
                                                        [SaleEntityServiceIds] [nvarchar](max) NULL
                                                        ) ON [PRIMARY];
                                                        CREATE CLUSTERED INDEX [IX_CustomerZoneDetail_SaleZoneId] ON [dbo].[CustomerZoneDetail] 
                                                        (
	                                                        [SaleZoneId] ASC
                                                        );";



        const string query_CodeMatchTable = @"    CREATE TABLE [dbo].[CodeMatch](
	                                                    [CodePrefix] [varchar](20) NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [Content] [nvarchar](max) NOT NULL
                                                    ) ON [PRIMARY]

                                                ";

        const string query_CodeSaleZoneTable = @"CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]

                                                    ";

        const string query_CustomerRouteTable = @"       CREATE TABLE [dbo].[CustomerRoute](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [Rate] [decimal](20, 8) NULL,
	                                                    [IsBlocked] [bit] NOT NULL,
	                                                    [ExecutedRuleId] [int] NOT NULL,
	                                                    [RouteOptions] [varchar](max) NULL/*,
                                                    CONSTRAINT [PK_CustomerRoute] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [CustomerID] ASC,
	                                                    [Code] ASC
                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                    */) ON [PRIMARY]

                                ";


        const string query_TableTypes = @"CREATE TYPE [LongIDType] AS TABLE(
	                                                    [ID] [bigint] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [ID] ASC
                                                    )WITH (IGNORE_DUP_KEY = OFF)
                                                    )

                                            ";

        const string query_RPZonesTableType = @"CREATE TYPE [RPZonesType] AS TABLE(
                                                    [RoutingProductId] [int] NOT NULL,                                                    
                                                    [SaleZoneId] [bigint] NOT NULL)";


        const string query_RoutingProductTable = @"
                                                    Create Table ProductRoute(
                                                        RoutingProductId int Not Null,
                                                        SaleZoneId bigint Not Null,
                                                        ExecutedRuleId int Null,
                                                        OptionsDetailsBySupplier nvarchar(max) NULL,
                                                        OptionsByPolicy nvarchar(max) NULL,
                                                        IsBlocked bit NULL
                                                    )ON [PRIMARY]

                                                    ";

        #endregion
    }
}
