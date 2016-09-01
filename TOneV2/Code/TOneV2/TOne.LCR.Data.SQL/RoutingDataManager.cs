using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.LCR.Entities;
using TOne.LCR.Entities.Routing;
using Vanrise.Entities;

namespace TOne.LCR.Data.SQL
{
    public class RoutingDataManager : BaseTOneDataManager, IRoutingDataManager
    {
        public RoutingDataManager() :
            base("RoutingDBConnStringTemplateKey")
        {

        }

        int _databaseId;
        RoutingDatabaseType? _routingDatabaseType;
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

        internal void CreateDatabase()
        {
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.CreateDatabase(GetDatabaseName(), ConfigurationManager.AppSettings["RoutingDBDataFileDirectory"], ConfigurationManager.AppSettings["RoutingDBLogFileDirectory"]);
            CreateDatabaseSchema();
        }

        internal void DropDatabaseIfExists()
        {
            MasterDatabaseDataManager masterDataManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDataManager.DropDatabaseWithForceIfExists(GetDatabaseName());
        }

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




CREATE TABLE [dbo].[ZoneInfo](
	                                                    [ZoneID] [int] NOT NULL,
	                                                    [Name] [nvarchar](255) NOT NULL,
                                                     CONSTRAINT [PK_ZoneInfo] PRIMARY KEY CLUSTERED 
                                                     (
	                                                    [ZoneID] ASC
                                                     )
                                                    ) 
                                                    CREATE TABLE [dbo].[SupplierZoneRate](
                                                          [RateID] [bigint] NOT NULL,
	                                                    [PriceListID] [int] NOT NULL,
	                                                    [ZoneID] [int] NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [NormalRate] float NULL,
	                                                    [ServicesFlag] [smallint] NULL,
                                                        [BED] [DateTime] Not NULL,
                                                        [EED] [DateTime] NULL
                                                    ) 
                                                    CREATE TABLE [dbo].[CustomerZoneRate](
                                                        [RateID] [bigint] NOT NULL,
	                                                    [PriceListID] [int] NOT NULL,
	                                                    [ZoneID] [int] NULL,
	                                                    [CustomerID] [varchar](5) NOT NULL,
	                                                    [NormalRate] [float] NULL,
	                                                    [ServicesFlag] [smallint] NULL,
                                                        [BED] [DateTime] Not NULL,
                                                        [EED] [DateTime] NULL
                                                    ) 
                                                    CREATE TABLE [dbo].[ZoneMatch](
	                                                    [OurZoneID] [int] NOT NULL,
	                                                    [SupplierZoneID] [int] NOT NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL,
                                                        [IsCodeGroup] BIT NOT NULL
                                                    ) 
                                                    CREATE TABLE [dbo].[CodeMatch](
	                                                    [Code] [varchar](30) NOT NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [SupplierCode] [varchar](30) NOT NULL,
	                                                    [SupplierCodeID] [bigint] NOT NULL,
	                                                    [SupplierZoneID] [int] NOT NULL,
	                                                    [SupplierRate] float NULL,
	                                                    [ServicesFlag] [smallint] NULL,
	                                                    [PriceListID] [int] NULL
                                                    )
                                                    CREATE TABLE [dbo].[Route](
	                                                    [CustomerID] [varchar](5) NOT NULL,
	                                                    [Code] [varchar](30) NULL,
	                                                    [OurZoneID] [int] NULL,
	                                                    [OurActiveRate] [decimal](18,5) NULL,
	                                                    [OurServicesFlag] [smallint] NULL,
	                                                    [Options] varchar(1000),
                                                        [RuleId] int NULL,
                                                        [RuleType] tinyint NULL,
                                                        [timestamp] timestamp
                                                    )

                                                    CREATE TYPE [dbo].[SuppliersCodeInfoType] AS TABLE(
                                                      
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [HasUpdatedCodes] [bit] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [SupplierID] ASC
                                                        )
                                                    )

                                                    CREATE TYPE [dbo].[IntIDType] AS TABLE(
	                                                    [ID] [int] NOT NULL
	                                                    PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [ID] ASC
                                                        )
                                                    )
                                                    
                                                    CREATE TYPE [dbo].[CodeType] AS TABLE(
	                                                        [Code] [varchar](20) NOT NULL,
	                                                        [IncludeSubCodes] BIT
	                                                        PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [Code] ASC
                                                        )WITH (IGNORE_DUP_KEY = OFF)
                                                        )

                                                         CREATE TYPE [dbo].[RouteType] AS TABLE(
	                                                        [Code] [varchar](20) NOT NULL,
	                                                        [CustomerId] [varchar](5) NOT NULL,
                                                            [Rate] [decimal](18,5) NULL,
                                                            [Options] varchar(500)
	                                                        )

                                                    CREATE TYPE [StringIDType] AS TABLE(
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [Code] ASC
                                                    )WITH (IGNORE_DUP_KEY = OFF)
                                                    )

                                                     CREATE TABLE [dbo].[TempZoneMatch](
	                                                    [OurZoneId] [int] NOT NULL,
                                                        [SupplierZoneId] [int] NOT NULL,
                                                        [SupplierId] [varchar](5) NOT NULL,
                                                        [IsCodeGroup] BIT NOT NULL
                                                    ) 
                                                    ";

        #endregion

       

    }
}
