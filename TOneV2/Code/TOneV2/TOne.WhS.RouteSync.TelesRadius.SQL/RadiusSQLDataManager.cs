using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.TelesRadius.SQL
{
    public class RadiusSQLDataManager : BaseSQLDataManager, IRadiusDataManager
    {

        public RadiusSQLDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        string _connectionString;
        int _configId;
        public void ApplyRadiusRoutesForDB(object preparedRadiusRoutes)
        {
            InsertBulkToTable(preparedRadiusRoutes as BaseBulkInsertInfo);
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
            return base.InitializeStreamForBulkInsert();
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Route_Temp]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }
        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            RadiusConvertedRoute radiusRoute = record as RadiusConvertedRoute;
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", radiusRoute.CustomerId, radiusRoute.Code, radiusRoute.Options);
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
            query.AppendLine(query_CreateTempTables);

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
            ExecuteNonQueryText("sp_RadiusRoute_SwapTables", null);
        }
        public int ConfigId
        {
            get
            {
                return _configId;
            }
            set
            {
                _configId = value;
            }
        }

        #endregion

        #region constants

        readonly string[] columns = { "Customer", "Code", "Options" };

        const string query_SwapTables = @"	IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_Old]') AND s.type in (N'U'))
		                                        begin
			                                        DROP TABLE [dbo].Route_Old
		                                        end
	                                        EXEC sp_rename 'Route', 'Route_Old'
	                                        EXEC sp_rename 'Route_Temp', 'Route'";
        const string query_CreateTempTables = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_temp]') AND s.type in (N'U'))
                                                        begin
                                                            DROP TABLE [dbo].[Route_temp]
                                                        end

                                                CREATE TABLE [dbo].[Route_temp](
	                                                [Customer] [nvarchar](50) NULL,
	                                                [Code] [nvarchar](20) NULL,
	                                                [Options] [nvarchar](max) NULL
                                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";


        #endregion
    }
}
