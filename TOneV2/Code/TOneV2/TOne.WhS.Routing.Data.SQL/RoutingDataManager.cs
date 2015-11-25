using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RoutingDataManager : BaseTOneDataManager, IRoutingDataManager
    {
        public RoutingDataManager() :
            base(GetConnectionStringName("TOneWhS_Routing_DBConnStringKey", "TOneWhS_Routing_DBConnString"))
        {

        }

        int _databaseId;
        RoutingDatabaseType? _routingDatabaseType;
        /// <summary>
        /// If Routing Database id is less than or equal 0, returns the next available Id according to type and date.
        /// </summary>
        public int DatabaseId
        {
            get
            {
                if (_databaseId <= 0)
                {
                    RoutingDatabaseDataManager routingDatabaseManager = new RoutingDatabaseDataManager();
                    _databaseId = routingDatabaseManager.GetIDByType(_routingDatabaseType.Value, DateTime.Now);
                }
                return _databaseId;
            }
            set
            {
                _databaseId = value;
            }
        }

        public RoutingDatabaseType RoutingDatabaseType
        {
            set
            {
                _routingDatabaseType = value;
            }
        }
        /// <summary>
        /// Create Routing Database.
        /// </summary>
        internal void CreateDatabase()
        {
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.CreateDatabase(GetDatabaseName(), ConfigurationManager.AppSettings["RoutingDBDataFileDirectory"], ConfigurationManager.AppSettings["RoutingDBLogFileDirectory"]);
            CreateDatabaseSchema();
        }
        /// <summary>
        /// Drop Routing Database if database already exists.
        /// </summary>
        internal void DropDatabaseIfExists()
        {
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.DropDatabaseWithForceIfExists(GetDatabaseName());
        }
        /// <summary>
        ///  Get Routing connection string by type.
        /// </summary>
        /// <returns>Routing Connection String</returns>
        protected override string GetConnectionString()
        {
            if (_databaseId <= 0)
            {
                if (!_routingDatabaseType.HasValue)
                    throw new Exception("you need to set the DatabaseId or the RoutingDatabaseType property of the Data Manager before calling any operation on the Routing database");

                RoutingDatabaseDataManager routingDatabaseManager = new RoutingDatabaseDataManager();
                _databaseId = routingDatabaseManager.GetIDByType(_routingDatabaseType.Value, DateTime.Now);

                if (_databaseId <= 0)
                    throw new Exception(String.Format("Routing Database {0} not found", _routingDatabaseType));
            }
            return String.Format(base.GetConnectionString(), _databaseId);
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
        private void CreateDatabaseSchema()
        {
            ExecuteNonQueryText(query_CreateDatabaseSchema, null);
        }

        #region Constants

        const string query_CreateDatabaseSchema = @"
                                                    Create Table SupplierZoneDetail(
                                                        SupplierId int Not Null,
                                                        SupplierZoneId bigint Not Null,
                                                        EffectiveRateValue [decimal](10, 7) Not Null
                                                    )ON [PRIMARY]

                                                CREATE TABLE [dbo].[CustomerZoneDetail](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [RoutingProductId] [int] NULL,
	                                                    [RoutingProductSource] [tinyint] NULL,
	                                                    [SellingProductId] [int] NULL,
	                                                    [EffectiveRateValue] [decimal](10, 7) NULL,
	                                                    [RateSource] [tinyint] NULL
                                                    ) ON [PRIMARY]

                                                CREATE TABLE [dbo].[CodeMatch](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [Content] [nvarchar](max) NOT NULL
                                                    ) ON [PRIMARY]

                                               CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]

                                                CREATE TABLE [dbo].[CustomerRoute](
	                                                    [CustomerID] [int] NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneID] [bigint] NOT NULL,
	                                                    [Rate] [decimal](10, 7) NULL,
	                                                    [IsBlocked] [bit] NOT NULL,
	                                                    [ExecutedRuleId] [int] NOT NULL,
	                                                    [RouteOptions] [nvarchar](max) NULL,
                                                     CONSTRAINT [PK_CustomerRoute] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [CustomerID] ASC,
	                                                    [Code] ASC
                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]

                                        CREATE TABLE [dbo].[RoutingProductOptions](
	                                                                    [ProductID] [int] NOT NULL,
	                                                                    [SaleZoneID] [int] NOT NULL,
	                                                                    [RouteOptions] [varchar](max) NOT NULL,
                                                                     CONSTRAINT [PK_RoutingProductRoute] PRIMARY KEY CLUSTERED 
                                                                    (
	                                                                    [ProductID] ASC,
	                                                                    [SaleZoneID] ASC
                                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                                    ) ON [PRIMARY]

                                                CREATE TYPE [LongIDType] AS TABLE(
	                                                    [ID] [bigint] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [ID] ASC
                                                    )WITH (IGNORE_DUP_KEY = OFF))
                                                ";

        const string query_SupplierZoneDetailsTable = @"Create Table SupplierZoneDetail(
                                                        SupplierId int Not Null,
                                                        SupplierZoneId bigint Not Null,
                                                        EffectiveRateValue [decimal](10, 7) Not Null
                                                    )ON [PRIMARY]  ";

        const string query_CustomerZoneDetailTable = @" CREATE TABLE [dbo].[CustomerZoneDetail](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [RoutingProductId] [int] NULL,
	                                                    [RoutingProductSource] [tinyint] NULL,
	                                                    [SellingProductId] [int] NULL,
	                                                    [EffectiveRateValue] [decimal](10, 7) NULL,
	                                                    [RateSource] [tinyint] NULL
                                                    ) ON [PRIMARY]";

        const string query_CodeMatchTable = @"CREATE TABLE [dbo].[CodeMatch](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [Content] [nvarchar](max) NOT NULL
                                                    ) ON [PRIMARY]";

        const string query_CodeSaleZoneTable = @"CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]";

        const string query_CustomerRouteTable = @"       CREATE TABLE [dbo].[CustomerRoute](
	                                                    [CustomerID] [int] NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneID] [bigint] NOT NULL,
	                                                    [Rate] [decimal](10, 7) NULL,
	                                                    [IsBlocked] [bit] NOT NULL,
	                                                    [ExecutedRuleId] [int] NOT NULL,
	                                                    [RouteOptions] [nvarchar](max) NULL,
                                                     CONSTRAINT [PK_CustomerRoute] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [CustomerID] ASC,
	                                                    [Code] ASC
                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]";

        const string query_RoutingProductOptionsTable = @"CREATE TABLE [dbo].[RoutingProductOptions](
	                                                                    [ProductID] [int] NOT NULL,
	                                                                    [SaleZoneID] [int] NOT NULL,
	                                                                    [RouteOptions] [varchar](max) NOT NULL,
                                                                     CONSTRAINT [PK_RoutingProductRoute] PRIMARY KEY CLUSTERED 
                                                                    (
	                                                                    [ProductID] ASC,
	                                                                    [SaleZoneID] ASC
                                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                                    ) ON [PRIMARY]";

        const string query_TableTypes = @"CREATE TYPE [LongIDType] AS TABLE(
	                                                    [ID] [bigint] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [ID] ASC
                                                    )WITH (IGNORE_DUP_KEY = OFF)
                                                    )";

        #endregion
    }
}
