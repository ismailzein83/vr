using System;
using System.Collections.Generic;
using System.Text;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.MVTSRadius.SQL
{
    public class RadiusSQLDataManager : BaseSQLDataManager, IRadiusDataManager
    {

        public RadiusSQLDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        string _connectionString;

        Guid _configId;
        public Guid ConfigId { get { return _configId; } set { _configId = new Guid("366AB5D3-5083-420D-B5CE-5313DA025106"); } }

        public void ApplyRadiusRoutesForDB(object preparedRadiusRoutes)
        {
            RadiusRouteBulkInsertInfo radiusRouteBulkInsertInfo = preparedRadiusRoutes as RadiusRouteBulkInsertInfo;
            InsertBulkToTable(radiusRouteBulkInsertInfo.RadiusRouteStreamBulkInsertInfo);
            InsertBulkToTable(radiusRouteBulkInsertInfo.RadiusRoutePercentageStreamBulkInsertInfo);
        }

        #region BaseSQLDataManager Overriden Methods
        protected override string GetConnectionString()
        {
            return _connectionString;
        }

        #endregion

        #region IBulkApplyDataManager
        public object InitialiazeStreamForDBApply()
        {
            RadiusRouteBulkInsert radiusRouteBulkInsert = new RadiusRouteBulkInsert();
            radiusRouteBulkInsert.RadiusRouteStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            radiusRouteBulkInsert.RadiusRoutePercentageStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            return radiusRouteBulkInsert;
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            RadiusRouteBulkInsert radiusRouteBulkInsert = dbApplyStream as RadiusRouteBulkInsert;

            radiusRouteBulkInsert.RadiusRouteStreamForBulkInsert.Close();
            radiusRouteBulkInsert.RadiusRoutePercentageStreamForBulkInsert.Close();

            RadiusRouteBulkInsertInfo radiusRouteBulkInsertInfo = new RadiusRouteBulkInsertInfo();

            radiusRouteBulkInsertInfo.RadiusRouteStreamBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[RadiusRoute_Temp]",
                Stream = radiusRouteBulkInsert.RadiusRouteStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = radiusRouteColumns
            };

            radiusRouteBulkInsertInfo.RadiusRoutePercentageStreamBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[RadiusRoutePercentage_Temp]",
                Stream = radiusRouteBulkInsert.RadiusRoutePercentageStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = radiusRoutePercentageColumns
            };

            return radiusRouteBulkInsertInfo;
        }
        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            MVTSRadiusConvertedRoute radiusRoute = record as MVTSRadiusConvertedRoute;
            bool hasPercentage = radiusRoute.HasPercentage;

            RadiusRouteBulkInsert radiusRouteBulkInsert = dbApplyStream as RadiusRouteBulkInsert;
            radiusRouteBulkInsert.RadiusRouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", radiusRoute.CustomerId, radiusRoute.Code, radiusRoute.Options, hasPercentage ? "Y" : "N");

            if(hasPercentage)
            {
                foreach(MVTSRadiusOption option in radiusRoute.MVTSRadiusOptions)
                {
                    radiusRouteBulkInsert.RadiusRoutePercentageStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", radiusRoute.CustomerId, radiusRoute.Code, option.Priority, option.Option, option.Percentage, 0);
                }
            }
        }

        #endregion

        #region IRadiusDataManager

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public void PrepareTables()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_CreateRadiusRouteTempTable);
            query.AppendLine(query_CreateRadiusRoutePercentageTempTable);
            ExecuteNonQueryText(query.ToString(), null);
        }
        public void InsertRoutes(List<ConvertedRoute> radiusRoutes)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (ConvertedRoute route in radiusRoutes)
                WriteRecordToStream(route, dbApplyStream);
            Object preparedRadiusRoutes = FinishDBApplyStream(dbApplyStream);
            ApplyRadiusRoutesForDB(preparedRadiusRoutes);
        }
        public void SwapTables()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_SwapRadiusRouteTable);
            query.AppendLine(query_SwapRadiusRoutePercentageTable);
            ExecuteNonQueryText(query.ToString(), null);
        }


        #endregion

        #region constants

        readonly string[] radiusRouteColumns = { "Customer", "Code", "RouteOption", "IsPercentage" };
        readonly string[] radiusRoutePercentageColumns = { "Customer", "Code", "Priority", "RouteOption", "Percentage", "Statistics" };

        const string query_SwapRadiusRouteTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoute_Old]') AND s.type in (N'U'))
		                                            begin
			                                            DROP TABLE [dbo].[RadiusRoute_Old]
		                                            end

                                                    IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoute]') AND s.type in (N'U'))
		                                            begin
	                                                    EXEC sp_rename 'RadiusRoute', 'RadiusRoute_Old'
		                                            end

	                                                EXEC sp_rename 'RadiusRoute_Temp', 'RadiusRoute'";

        const string query_CreateRadiusRouteTempTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoute_Temp]') AND s.type in (N'U'))
                                                          begin
                                                              DROP TABLE [dbo].[RadiusRoute_Temp]
                                                          end
                                                          
                                                          CREATE TABLE [dbo].[RadiusRoute_Temp](
	                                                      [Customer] [varchar](50) NOT NULL,
	                                                      [Code] [varchar](50) NOT NULL,
	                                                      [RouteOption] [varchar](255) NOT NULL,
	                                                      [IsPercentage] [varchar](1) NOT NULL) 
                                                          ON [PRIMARY]";

        const string query_SwapRadiusRoutePercentageTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoutePercentage_Old]') AND s.type in (N'U'))
		                                                      begin
			                                                      DROP TABLE [dbo].[RadiusRoutePercentage_Old]
		                                                      end
                                                              
                                                              IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoutePercentage]') AND s.type in (N'U'))
		                                                      begin
	                                                              EXEC sp_rename 'RadiusRoutePercentage', 'RadiusRoutePercentage_Old'
		                                                      end
                                                              
	                                                          EXEC sp_rename 'RadiusRoutePercentage_Temp', 'RadiusRoutePercentage'";

        const string query_CreateRadiusRoutePercentageTempTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoutePercentage_Temp]') AND s.type in (N'U'))
                                                                    begin
                                                                        DROP TABLE [dbo].[RadiusRoutePercentage_Temp]
                                                                    end
                                                          
                                                                    CREATE TABLE [dbo].[RadiusRoutePercentage_Temp](
	                                                                [Customer] [varchar](50) NOT NULL,
	                                                                [Code] [varchar](50) NOT NULL,
	                                                                [Priority] [int] NOT NULL,
	                                                                [RouteOption] [varchar](255) NOT NULL,
	                                                                [Percentage] [int] NOT NULL,
	                                                                [Statistics] [int] NOT NULL) 
                                                                    ON [PRIMARY]";
        #endregion

        private class RadiusRouteBulkInsert
        {
            public StreamForBulkInsert RadiusRouteStreamForBulkInsert { get; set; }
            public StreamForBulkInsert RadiusRoutePercentageStreamForBulkInsert { get; set; }
        }

        private class RadiusRouteBulkInsertInfo
        {
            public StreamBulkInsertInfo RadiusRouteStreamBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo RadiusRoutePercentageStreamBulkInsertInfo { get; set; }
        }
    }
}
