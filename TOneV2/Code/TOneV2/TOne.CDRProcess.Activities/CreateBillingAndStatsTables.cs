//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Activities;
//using System.Collections.Concurrent;
//using System.Data;
//using System.Threading;
//using TABS;
//using TOne.Entities;
//using TOne.Caching;
//using Vanrise.Caching;
//using Vanrise.BusinessProcess;
//using TOne.Business;

//namespace TOne.CDRProcess.Activities
//{

//    #region Argument Classes
//    public class CreateBillingAndStatsTablesInput
//    {
        
//        public DateTime From { get; set; }

//        public DateTime To { get; set; }

//        public ConcurrentQueue<CDRBatch> QueueLoadedCDRs { get; set; }

//        public Guid CacheManagerId { get; set; }

//        public ConcurrentQueue<DataTable> QueueReadyTables { get; set; }
//    }

//    #endregion

//    public sealed class CreateBillingAndStatsTables : DependentAsyncActivity<CreateBillingAndStatsTablesInput>
//    {
//        #region Arguments

//        [RequiredArgument]
//        public InArgument<DateTime> From { get; set; }

//        [RequiredArgument]
//        public InArgument<DateTime> To { get; set; }

//        [RequiredArgument]
//        public InArgument<ConcurrentQueue<CDRBatch>> QueueLoadedCDRs { get; set; }

//        [RequiredArgument]
//        public InArgument<Guid> CacheManagerId { get; set; }

//        [RequiredArgument]
//        public InArgument<ConcurrentQueue<DataTable>> QueueReadyTables { get; set; }

        

//        #endregion

        
//        #region Private Methods

//        static object s_DataTableMappingsLock = new object();
//        protected static DataTable _TrafficStatsTable;


//        protected static DataTable TrafficStatsTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_TrafficStatsTable == null)
//                    {
//                        _TrafficStatsTable = new DataTable();
//                        _TrafficStatsTable.Columns.Add("SwitchId", typeof(int));
//                        _TrafficStatsTable.Columns.Add("Port_IN", typeof(string));
//                        _TrafficStatsTable.Columns.Add("Port_OUT", typeof(string));
//                        _TrafficStatsTable.Columns.Add("CustomerID", typeof(string));
//                        _TrafficStatsTable.Columns.Add("OurZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("OriginatingZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("SupplierID", typeof(string));
//                        _TrafficStatsTable.Columns.Add("SupplierZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("FirstCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTable.Columns.Add("LastCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTable.Columns.Add("Attempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DeliveredAttempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("SuccessfulAttempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DurationsInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("PDDInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("NumberOfCalls", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
//                        _TrafficStatsTable.Columns.Add("PGAD", typeof(int));
//                        _TrafficStatsTable.Columns.Add("CeiledDuration", typeof(int));
//                        _TrafficStatsTable.Columns.Add("ReleaseSourceAParty", typeof(int));
//                    }
//                }
//                return _TrafficStatsTable;
//            }
//        }

//        protected DataTable GetTrafficStatsTable(IEnumerable<TrafficStats> stats, string tableName)
//        {
//            //log.DebugFormat("Bulk Building Data table: {0}", tableName);
//            DataTable dt = TrafficStatsTable.Clone();
//            dt.TableName = tableName;

//            dt.BeginLoadData();
//            foreach (var group in stats)
//            {
//                DataRow row = dt.NewRow();
//                int index = -1;
//                index++; row[index] = group.Switch.SwitchID;
//                index++; row[index] = group.Port_IN;
//                index++; row[index] = group.Port_OUT;
//                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
//                index++; row[index] = group.OurZone == null ? DBNull.Value : (object)group.OurZone.ZoneID;
//                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
//                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
//                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
//                index++; row[index] = group.FirstCDRAttempt;
//                index++; row[index] = group.LastCDRAttempt;
//                index++; row[index] = group.Attempts;
//                index++; row[index] = group.DeliveredAttempts;
//                index++; row[index] = group.SuccessfulAttempts;
//                index++; row[index] = group.DurationsInSeconds;
//                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
//                index++; row[index] = group.MaxDurationInSeconds;
//                index++; row[index] = group.UtilizationInSeconds;
//                index++; row[index] = group.NumberOfCalls;
//                index++; row[index] = group.DeliveredNumberOfCalls;
//                index++; row[index] = group.PGAD;
//                index++; row[index] = group.CeiledDuration;
//                index++; row[index] = group.ReleaseSourceAParty;
//                dt.Rows.Add(row);

//            }
//            dt.EndLoadData();
//            return dt;
//        }

//        #endregion

//        protected override void DoWork(CreateBillingAndStatsTablesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
//        {
//            log4net.ILog logger = log4net.LogManager.GetLogger(TABS.Components.Engine.RepricingLoggerName);
//            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);

//            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);

//            ProtPricingGenerator generator;
//            using (NHibernate.ISession session = DataConfiguration.OpenSession())
//            {
//                generator = new ProtPricingGenerator(cacheManager, session);
//                session.Flush();
//                session.Close();
//            }

//            int sampleMinute = (int)(inputArgument.To - inputArgument.From).TotalMinutes;

//            Dictionary<string, TrafficStats> trafficStatistics = new Dictionary<string, TrafficStats>();
//            Dictionary<string, TrafficStats> dailyTrafficStatistics = cacheManager.GetOrCreateObject("DailyStatistics", CacheObjectType.TempObjects,
//                () =>
//                {
//                    return new Dictionary<string, TrafficStats>();
//                });
//            List<System.Threading.Tasks.Task> asyncTasks = new List<System.Threading.Tasks.Task>();



//            while (! previousActivityStatus.IsComplete || inputArgument.QueueLoadedCDRs.Count > 0)
//            {
//                CDRBatch cdrBatch;

//                while (inputArgument.QueueLoadedCDRs.TryDequeue(out cdrBatch))
//                {
//                    CDRBatchProcessor cdrBatchProcessor = new CDRBatchProcessor(inputArgument.QueueReadyTables, logger, codeMap, generator, sampleMinute, trafficStatistics, dailyTrafficStatistics, cdrBatch);
//                    var t = cdrBatchProcessor.ExecuteAsync();
//                    asyncTasks.Add(t);
//                }
//                Thread.Sleep(1000);
//            }

//            System.Threading.Tasks.Task.WaitAll(asyncTasks.ToArray());
//            if (trafficStatistics.Count > 0)
//            {
//                DataTable dtTrafficStatistics = GetTrafficStatsTable(trafficStatistics.Values, BulkManager.TRAFFICSTATS_TABLE_NAME);
//                inputArgument.QueueReadyTables.Enqueue(dtTrafficStatistics);
//            }
//        }

//        protected override CreateBillingAndStatsTablesInput GetInputArgument2(AsyncCodeActivityContext context)
//        {
//            return new CreateBillingAndStatsTablesInput
//            {
//                From = this.From.Get(context),
//                To = this.To.Get(context),
//                QueueLoadedCDRs = this.QueueLoadedCDRs.Get(context),
//                CacheManagerId = this.CacheManagerId.Get(context),
//                QueueReadyTables = this.QueueReadyTables.Get(context)
//            };
//        }
//    }
//}
