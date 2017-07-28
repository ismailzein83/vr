using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;
using Vanrise.Data.SQL;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.MVTSRadius.SQL
{
    public class RadiusSQLDataManager : BaseSQLDataManager, IRadiusDataManager
    {

        public RadiusSQLDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        RadiusConnectionString _connectionString;
        List<RadiusConnectionString> _redundantConnectionStrings;


        Dictionary<int, MVTSRadiusSQLDataManager> _mvtsRadiusSQLDataManagers;
        public Guid ConfigId { get { return new Guid("366AB5D3-5083-420D-B5CE-5313DA025106"); } }

        public void ApplyRadiusRoutesForDB(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            RadiusRouteBulkInsertInfo radiusRouteBulkInsertInfo = context.PreparedItemsForApply as RadiusRouteBulkInsertInfo;

            PrepareDataManagers();
            Dictionary<MVTSRadiusSQLDataManager, BulkInsertStreams> dataManagersStreams = new Dictionary<MVTSRadiusSQLDataManager, BulkInsertStreams>();
            foreach (var dataManagerEntry in _mvtsRadiusSQLDataManagers)
            {
                var dataManagerIndex = dataManagerEntry.Key;
                BaseBulkInsertInfo routeStreamInfo = dataManagerIndex == 0 ? radiusRouteBulkInsertInfo.RadiusRouteStreamBulkInsertInfo : radiusRouteBulkInsertInfo.RadiusRouteStreamBulkInsertInfo.CreateOutputCopy();
                BaseBulkInsertInfo routePercentageStreamInfo = dataManagerIndex == 0 ? radiusRouteBulkInsertInfo.RadiusRoutePercentageStreamBulkInsertInfo : radiusRouteBulkInsertInfo.RadiusRoutePercentageStreamBulkInsertInfo.CreateOutputCopy();
                dataManagersStreams.Add(dataManagerEntry.Value, new BulkInsertStreams
                {
                    RouteStream = routeStreamInfo,
                    RoutePercentageStream = routePercentageStreamInfo
                });
            }
            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((mvtsRadiusSQLDataManager, dataManagerIndex) =>
            {
                BulkInsertStreams streams = dataManagersStreams[mvtsRadiusSQLDataManager];
                mvtsRadiusSQLDataManager.ApplyRadiusRoutesForDB(streams.RouteStream, streams.RoutePercentageStream);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, false, null, out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        private class BulkInsertStreams
        {
            public BaseBulkInsertInfo RouteStream { get; set; }

            public BaseBulkInsertInfo RoutePercentageStream { get; set; }
        }

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

            if (hasPercentage)
            {
                foreach (MVTSRadiusOption option in radiusRoute.MVTSRadiusOptions)
                {
                    radiusRouteBulkInsert.RadiusRoutePercentageStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", radiusRoute.CustomerId, radiusRoute.Code, option.Priority, option.Option, option.Percentage.HasValue ? option.Percentage : 0, 0);
                }
            }
        }

        #endregion

        #region IRadiusDataManager

        public RadiusConnectionString ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public List<RadiusConnectionString> RedundantConnectionStrings
        {
            get { return _redundantConnectionStrings; }
            set { _redundantConnectionStrings = value; }
        }
        public void PrepareTables(ISwitchRouteSynchronizerInitializeContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((mvtsRadiusSQLDataManager, dataManagerIndex) =>
            {
                mvtsRadiusSQLDataManager.PrepareTables();
            }, context.SwitchName, context.SwitchId, null, context.WriteBusinessHandledException, true, "initializing", out switchSyncOutput);
            context.SwitchSyncOutput = switchSyncOutput;
        }

        public object PrepareDataForApply(List<ConvertedRoute> radiusRoutes)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (ConvertedRoute route in radiusRoutes)
                WriteRecordToStream(route, dbApplyStream);
            return FinishDBApplyStream(dbApplyStream);
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            ApplyRadiusRoutesForDB(context);
        }

        public void SwapTables(ISwapTableContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecMVTSRadiusSQLDataManagerAction((mvtsRadiusSQLDataManager, dataManagerIndex) =>
            {
                string[] args = new string[] { (dataManagerIndex + 1).ToString(), context.SwitchName };

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finalizing Database {0} for Switch '{1}'...", args);
                mvtsRadiusSQLDataManager.SwapTables(context.IndexesCommandTimeoutInSeconds);
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Database {0} for Switch '{1}' is finalized", args);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, true, "finalizing", out switchSyncOutput);
            context.SwitchSyncOutput = switchSyncOutput;
        }

        #endregion

        private void ExecMVTSRadiusSQLDataManagerAction(Action<MVTSRadiusSQLDataManager, int> action, string switchName, string switchId, SwitchSyncOutput previousSwitchSyncOutput,
            Action<Exception> writeBusinessHandledException, bool isBusinessException, string businessExceptionMessage, out SwitchSyncOutput switchSyncOutput)
        {
            PrepareDataManagers();
            HashSet<int> failedNodeIndexes = null;
            if (previousSwitchSyncOutput != null && previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList != null)
            {
                failedNodeIndexes = previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList.Select(itm => (itm as MVTSRadiusSWSyncOutput).ItemIndex).ToHashSet();
                if (failedNodeIndexes != null && failedNodeIndexes.Count == _mvtsRadiusSQLDataManagers.Count)
                {
                    switchSyncOutput = new SwitchSyncOutput()
                    {
                        SwitchId = switchId,
                        SwitchSyncResult = SwitchSyncResult.Failed
                    };
                    return;
                }
            }

            ConcurrentDictionary<int, SwitchRouteSynchroniserOutput> exceptions = new ConcurrentDictionary<int, SwitchRouteSynchroniserOutput>();

            Parallel.For(0, _mvtsRadiusSQLDataManagers.Count, (i) =>
            {
                if (failedNodeIndexes == null || !failedNodeIndexes.Contains(i))
                {
                    try
                    {
                        action(_mvtsRadiusSQLDataManagers[i], i);
                    }
                    catch (Exception ex)
                    {
                        string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                        string exceptionDetail = ex.ToString();
                        exceptions.TryAdd(i, new MVTSRadiusSWSyncOutput() { ItemIndex = i, ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail });
                        Exception exception = isBusinessException ? new VRBusinessException(string.Format("Error occured while {0} Database {1} for Switch '{2}'", businessExceptionMessage, i + 1, switchName), ex) : ex;
                        writeBusinessHandledException(exception);
                    }
                }
            });
            switchSyncOutput = exceptions.Count > 0 ? new SwitchSyncOutput()
            {
                SwitchId = switchId,
                SwitchRouteSynchroniserOutputList = exceptions.Values.ToList(),
                SwitchSyncResult = exceptions.Count == _mvtsRadiusSQLDataManagers.Count ? SwitchSyncResult.Failed : SwitchSyncResult.PartialFailed
            } : null;
        }

        private void PrepareDataManagers()
        {
            if (_mvtsRadiusSQLDataManagers == null)
            {
                int counter = 0;
                _mvtsRadiusSQLDataManagers = new Dictionary<int, MVTSRadiusSQLDataManager>();
                _mvtsRadiusSQLDataManagers.Add(counter, new MVTSRadiusSQLDataManager(_connectionString));
                counter++;

                if (_redundantConnectionStrings != null)
                {
                    foreach (RadiusConnectionString radiusConnectionString in _redundantConnectionStrings)
                    {
                        _mvtsRadiusSQLDataManagers.Add(counter, new MVTSRadiusSQLDataManager(radiusConnectionString));
                        counter++;
                    }
                }
            }
        }

        #region constants

        readonly string[] radiusRouteColumns = { "Customer", "Code", "RouteOption", "IsPercentage" };
        readonly string[] radiusRoutePercentageColumns = { "Customer", "Code", "Priority", "RouteOption", "Percentage", "Statistics" };


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

    public class MVTSRadiusSQLDataManager : BaseSQLDataManager
    {
        public string ConnectionString { get { return GetConnectionString(); } }

        RadiusConnectionString _radiusConnectionString;
        public MVTSRadiusSQLDataManager(RadiusConnectionString radiusConnectionString)
        {
            _radiusConnectionString = radiusConnectionString;
        }
        protected override string GetConnectionString()
        {
            return _radiusConnectionString.ConnectionString;
        }
        public void ApplyRadiusRoutesForDB(BaseBulkInsertInfo radiusRouteStreamBulkInsertInfo, BaseBulkInsertInfo radiusRoutePercentageStreamBulkInsertInfo)
        {
            Parallel.For(0, 2, (i) =>
            {
                switch (i)
                {
                    case 0: InsertBulkToTable(radiusRouteStreamBulkInsertInfo); ; break;
                    case 1: InsertBulkToTable(radiusRoutePercentageStreamBulkInsertInfo); break;
                }
            });
        }
        public void PrepareTables()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(query_CreateRadiusRouteTable);
            query.AppendLine(query_CreateRadiusRoutePercentageTable);
            query.AppendLine(query_CreateRadiusRouteTempTable);
            query.AppendLine(query_CreateRadiusRoutePercentageTempTable);
            ExecuteNonQueryText(query.ToString(), null);
        }
        public void SwapTables(int indexesCommandTimeoutInSeconds)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(string.Format(query_CreateIndexes, _radiusConnectionString.MaxDoP.HasValue ? string.Format(",MAXDOP={0}", _radiusConnectionString.MaxDoP.Value) : ""));
            query.AppendLine(query_SwapRadiusRouteTable);
            query.AppendLine(query_SwapRadiusRoutePercentageTable);

            ExecuteNonQueryText(query.ToString(), null, indexesCommandTimeoutInSeconds);
        }

        #region Constants

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

        const string query_CreateRadiusRoutePercentageTable = @"IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoutePercentage]') AND s.type in (N'U'))
                                                                  begin                                                                                              CREATE TABLE [dbo].[RadiusRoutePercentage](
	                                                                [Customer] [varchar](50) NOT NULL,
	                                                                [Code] [varchar](50) NOT NULL,
	                                                                [Priority] [int] NOT NULL,
	                                                                [RouteOption] [varchar](255) NOT NULL,
	                                                                [Percentage] [int] NOT NULL,
	                                                                [Statistics] [int] NOT NULL) 
                                                                    ON [PRIMARY]  
                                                                  end";
        const string query_CreateRadiusRouteTable = @"IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RadiusRoute]') AND s.type in (N'U'))
                                                          begin
                                                              CREATE TABLE [dbo].[RadiusRoute](
	                                                          [Customer] [varchar](50) NOT NULL,
	                                                          [Code] [varchar](50) NOT NULL,
	                                                          [RouteOption] [varchar](255) NOT NULL,
	                                                          [IsPercentage] [varchar](1) NOT NULL) 
                                                              ON [PRIMARY]
                                                          end";

        const string query_CreateIndexes = @"
                            CREATE CLUSTERED INDEX customercode ON dbo.RadiusRoute_Temp
	                            (
	                            Customer,
	                            Code
	                            ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON {0}) ON [PRIMARY]

                           CREATE CLUSTERED INDEX CustomerCode ON dbo.RadiusRoutePercentage_Temp
	                            (
	                            Customer,
	                            Code
	                            ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON {0}) ON [PRIMARY]";

        #endregion
    }
}