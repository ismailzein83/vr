using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
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


		public void FinalizeCustomerRouteDatabase(Action<string> trackStep)
		{
			CustomerRouteDataManager customerRouteDataManager = new CustomerRouteDataManager() { RoutingDatabase = this.RoutingDatabase };
			customerRouteDataManager.FinalizeCurstomerRoute(trackStep);
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
			query.AppendLine(query_CustomerZoneDetailTable);
			query.AppendLine(query_CustomerRouteTable);
			query.AppendLine(query_CodeMatchTable);
			//query.AppendLine(query_CodeSaleZoneTable);
			query.AppendLine(query_TableTypes);

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
			query.AppendLine(query_CodeSaleZoneMatchTable);
			query.AppendLine(query_CodeSupplierZoneMatchTable);
			ExecuteNonQueryText(query.ToString(), null);

			//CREATE FUNCTION must be the only statement in the batch
			ExecuteNonQueryText(query_ParseStringListFunction.ToString(), null);
		}

		#region Constants

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

		const string query_CodeMatchTable = @"    CREATE TABLE [dbo].[CodeMatch](
	                                                    [CodePrefix] [varchar](20) NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [Content] [nvarchar](max) NOT NULL
                                                    ) ON [PRIMARY]";

		const string query_CodeSaleZoneTable = @"CREATE TABLE [dbo].[CodeSaleZone](
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL
                                                    ) ON [PRIMARY]";

		const string query_CustomerRouteTable = @"CREATE TABLE [dbo].[CustomerRoute](
	                                                    [CustomerId] [int] NOT NULL,
	                                                    [Code] [varchar](20) NOT NULL,
	                                                    [SaleZoneId] [bigint] NOT NULL,
	                                                    [Rate] [decimal](20, 8) NULL,
                                                        [SaleZoneServiceIds] [varchar](max) NULL,
	                                                    [IsBlocked] [bit] NOT NULL,
	                                                    [ExecutedRuleId] [int] NOT NULL,
	                                                    [RouteOptions] [varchar](max) NULL/*,
                                                    CONSTRAINT [PK_CustomerRoute] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [CustomerID] ASC,
	                                                    [Code] ASC
                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                                    */) ON [PRIMARY] ";

		private const string query_CodeSaleZoneMatchTable = @"CREATE TABLE [dbo].[CodeSaleZoneMatch](
																[Code] [varchar](20) NOT NULL,
																[SellingNumberPlanID] [int] NOT NULL,
																[SaleZoneID] [bigint] NOT NULL
															) ON [PRIMARY]
															CREATE CLUSTERED INDEX [IX_CodeSaleZoneMatch_Code] ON [dbo].[CodeSaleZoneMatch]
															(
																[Code] ASC
															);";

		private const string query_CodeSupplierZoneMatchTable = @"CREATE TABLE [dbo].[CodeSupplierZoneMatch](
																	[Code] [varchar](20) NOT NULL,
																	[SupplierID] [int] NOT NULL,
																	[SupplierZoneID] [bigint] NOT NULL
																) ON [PRIMARY]
																CREATE CLUSTERED INDEX [IX_CodeSupplierZoneMatch_Code] ON [dbo].[CodeSupplierZoneMatch]
																(
																	[Code] ASC
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