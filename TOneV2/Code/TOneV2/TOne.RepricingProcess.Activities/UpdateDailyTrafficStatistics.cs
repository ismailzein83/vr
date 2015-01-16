using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Data;
using System.Data;
using TABS;
using TOne.Entities;
using TOne.Business;
using TOne.Caching;
using Vanrise.Caching;

namespace TOne.RepricingProcess.Activities
{

    public sealed class UpdateDailyTrafficStatistics : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> CacheManagerID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> Day { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteDailyTrafficStats(this.Day.Get(context));
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(this.CacheManagerID.Get(context));
            Dictionary<string, TrafficStats> dailyTrafficStatistics = cacheManager.GetOrCreateObject("DailyStatistics", CacheObjectType.TempObjects,
               () =>
               {
                   return new Dictionary<string, TrafficStats>();
               });
            if (dailyTrafficStatistics.Count == 0)
                return;
            DataTable dtDailyTrafficStatistics = GetDailyTrafficStatsTable(dailyTrafficStatistics.Values, BulkManager.DAILYTRAFFICSTATS_TABLE_NAME);
            BulkManager.Instance.Write(dtDailyTrafficStatistics.TableName, dtDailyTrafficStatistics);

            Console.WriteLine("{0}: Finished Rebuilding Daily Traffic Stats", DateTime.Now);
        }

        DataTable GetDailyTrafficStatsTable(IEnumerable<TrafficStats> stats, string tableName)
        {
            DataTable DailystatsTable = TrafficStatsDailyTable.Clone();
            DailystatsTable.TableName = tableName;
            DailystatsTable.BeginLoadData();
            foreach (var group in stats)//.GroupBy(s=>s.CallDate.ToString("yyyy-MM-dd")
            {
                DataRow row = DailystatsTable.NewRow();
                int index = -1;
                index++; row[index] = group.CallDate;
                index++; row[index] = group.Switch.SwitchID;
                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
                index++; row[index] = group.OurZone == null ? DBNull.Value : (object)group.OurZone.ZoneID;
                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
                index++; row[index] = group.Attempts;
                index++; row[index] = group.DeliveredAttempts;
                index++; row[index] = group.SuccessfulAttempts;
                index++; row[index] = group.DurationsInSeconds;
                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
                index++; row[index] = group.UtilizationInSeconds;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.DeliveredNumberOfCalls;
                index++; row[index] = group.PGAD;
                index++; row[index] = group.CeiledDuration;
                index++; row[index] = group.MaxDurationInSeconds;
                index++; row[index] = group.ReleaseSourceAParty;
                DailystatsTable.Rows.Add(row);
            }
            DailystatsTable.EndLoadData();
            return DailystatsTable;
        }

        static object s_DataTableMappingsLock = new object();

        static DataTable _TrafficStatsDailyTable;
        static DataTable TrafficStatsDailyTable
        {
            get
            {
                lock (s_DataTableMappingsLock)
                {
                    if (_TrafficStatsDailyTable == null)
                    {
                        _TrafficStatsDailyTable = new DataTable();
                        //_TrafficStatsDailyTable.Columns.Add("ID", typeof(long));
                        _TrafficStatsDailyTable.Columns.Add("Calldate", typeof(DateTime));
                        _TrafficStatsDailyTable.Columns.Add("SwitchId", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("CustomerID", typeof(string));
                        _TrafficStatsDailyTable.Columns.Add("OurZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("OriginatingZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("SupplierID", typeof(string));
                        _TrafficStatsDailyTable.Columns.Add("SupplierZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("Attempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DeliveredAttempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("SuccessfulAttempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DurationsInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("PDDInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("NumberOfCalls", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("PGAD", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("CeiledDuration", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("ReleaseSourceAParty", typeof(int));
                    }
                }
                return _TrafficStatsDailyTable;
            }
            set { _TrafficStatsDailyTable = value; }
        }
    }
}
