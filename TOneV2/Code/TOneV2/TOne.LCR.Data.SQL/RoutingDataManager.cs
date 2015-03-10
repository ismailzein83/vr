using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.LCR.Entities;

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
            masterDataManager.CreateDatabase(GetDatabaseName());
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

        const string query_CreateDatabaseSchema = @"CREATE TABLE [dbo].[ZoneInfo](
	                                                    [ZoneID] [int] NOT NULL,
	                                                    [Name] [nvarchar](255) NOT NULL,
                                                     CONSTRAINT [PK_ZoneInfo] PRIMARY KEY CLUSTERED 
                                                     (
	                                                    [ZoneID] ASC
                                                     )
                                                    ) 
                                                    CREATE TABLE [SupplierZoneRate](
	                                                    [RateID] [bigint] NOT NULL,
	                                                    [PriceListID] [int] NOT NULL,
	                                                    [ZoneID] [int] NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [NormalRate] float NULL,
	                                                    [ServicesFlag] [smallint] NULL
                                                    ) 
                                                    CREATE TABLE [CustomerZoneRate](
	                                                    [RateID] [bigint] NOT NULL,
	                                                    [PriceListID] [int] NOT NULL,
	                                                    [ZoneID] [int] NULL,
	                                                    [CustomerID] [varchar](5) NOT NULL,
	                                                    [NormalRate] [float] NULL,
	                                                    [ServicesFlag] [smallint] NULL
                                                    ) 
                                                    CREATE TABLE [ZoneMatch](
	                                                    [OurZoneID] [int] NOT NULL,
	                                                    [SupplierZoneID] [int] NOT NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL
                                                    ) 
                                                    CREATE TABLE [CodeMatch](
	                                                    [Code] [varchar](30) NOT NULL,
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [SupplierCode] [varchar](30) NOT NULL,
	                                                    [SupplierCodeID] [bigint] NOT NULL,
	                                                    [SupplierZoneID] [int] NOT NULL
                                                    )
                                                    CREATE TABLE [Route](
	                                                    [RouteID] [int] IDENTITY(1,1) NOT NULL ,
	                                                    [CustomerID] [varchar](5) NOT NULL,
	                                                    [Code] [varchar](15) NULL,
	                                                    [OurZoneID] [int] NULL,
	                                                    [OurActiveRate] [real] NULL,
	                                                    [OurServicesFlag] [smallint] NULL,
	                                                    [Options] varchar(max)
                                                    )

                                                    CREATE TYPE [SuppliersCodeInfoType] AS TABLE(
	                                                    [SupplierID] [varchar](5) NOT NULL,
	                                                    [HasUpdatedCodes] [bit] NOT NULL,
	                                                    PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [SupplierID] ASC
                                                        )
                                                    )

                                                    CREATE TYPE [IntIDType] AS TABLE(
	                                                    [ID] [int] NOT NULL
	                                                    PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [ID] ASC
                                                        )
                                                    )
                                                    ";

        #endregion
    }
}
