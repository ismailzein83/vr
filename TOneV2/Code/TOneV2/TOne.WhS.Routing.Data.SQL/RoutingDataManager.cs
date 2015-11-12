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
                                                        EffectiveRateValue numeric(18,4) Not Null
                                                    )ON [PRIMARY]                                        
                                                    
                                                    CREATE TABLE [dbo].[CodeMatch](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [Content] [nvarchar](max) NOT NULL
                                                    ) ON [PRIMARY]

                                                    CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]

                                                    CREATE TABLE [dbo].[RoutingCustomerInfo](
	                                                    [CustomerId] [int] NOT NULL
                                                    ) ON [PRIMARY]

                                                    CREATE TABLE [dbo].[RoutingSupplierInfo](
	                                                    [SupplierId] [int] NOT NULL
                                                    ) ON [PRIMARY]";

        #endregion
    }
}
