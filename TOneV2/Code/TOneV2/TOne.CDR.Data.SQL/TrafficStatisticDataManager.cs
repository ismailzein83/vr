using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class TrafficStatisticDataManager : BaseTOneDataManager, ITrafficStatisticDataManager
    {

        #region TRAFFIC STAT

        const string TRAFFICSTATISTIC_TABLENAME = "TrafficStats";
        
        public void UpdateTrafficStatisticBatch(DateTime batchStart, DateTime batchEnd, TrafficStatisticsByKey trafficStatisticsByKey)
        {
            Dictionary<string, int> existingItemsIdByGroupKeys = GetTrafficStatisticsIdsByGroupKeys(batchStart, batchEnd);

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            DataTable dtStatisticsToUpdate = GetTrafficStatsTable();

            dtStatisticsToUpdate.BeginLoadData();

            foreach (var item in trafficStatisticsByKey)
            {
                TrafficStatistic trafficStatistic = item.Value;
                int existingId;
                if (existingItemsIdByGroupKeys != null && existingItemsIdByGroupKeys.TryGetValue(item.Key, out existingId))
                {
                    trafficStatistic.ID = existingId;
                    DataRow dr = dtStatisticsToUpdate.NewRow();
                    FillStatisticRow(dr, trafficStatistic);
                    dtStatisticsToUpdate.Rows.Add(dr);
                }
                else
                {
                    AddTrafficStatisticToStream(stream, trafficStatistic);
                }
            }
            stream.Close();

            dtStatisticsToUpdate.EndLoadData();

            if (dtStatisticsToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[Analytics].[sp_TrafficStats_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@TrafficStats", SqlDbType.Structured);
                        dtPrm.Value = dtStatisticsToUpdate;
                        cmd.Parameters.Add(dtPrm);
                    });

            base.InsertBulkToTable(new Vanrise.Data.SQL.StreamBulkInsertInfo
            {
                Stream = stream,
                TableName = TRAFFICSTATISTIC_TABLENAME,
                TabLock = false,
                FieldSeparator = '^'
            });
        }

        private void AddTrafficStatisticToStream(StreamForBulkInsert stream, TrafficStatistic trafficStatistic)
        {
            stream.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}",
                trafficStatistic.SwitchId,
                trafficStatistic.Port_IN,
                trafficStatistic.Port_OUT,
                trafficStatistic.CustomerId,
                trafficStatistic.OurZoneId,
                trafficStatistic.OriginatingZoneId,
                trafficStatistic.SupplierId,
                trafficStatistic.SupplierZoneId,
                trafficStatistic.FirstCDRAttempt,
                trafficStatistic.LastCDRAttempt,
                trafficStatistic.Attempts,
                trafficStatistic.DeliveredAttempts,
                trafficStatistic.SuccessfulAttempts,
                trafficStatistic.DurationsInSeconds,
                trafficStatistic.PDDInSeconds,
                trafficStatistic.MaxDurationInSeconds,
                trafficStatistic.UtilizationInSeconds,
                trafficStatistic.NumberOfCalls,
                trafficStatistic.DeliveredNumberOfCalls,
                trafficStatistic.PGAD,
                trafficStatistic.CeiledDuration,
                trafficStatistic.ReleaseSourceAParty,
                trafficStatistic.ReleaseSourceS);
        }

        void FillStatisticRow(DataRow dr, TrafficStatistic trafficStatistic)
        {
            dr["ID"] = trafficStatistic.ID;
            dr["FirstCDRAttempt"] = trafficStatistic.FirstCDRAttempt;
            dr["LastCDRAttempt"] = trafficStatistic.LastCDRAttempt;
            dr["Attempts"] = trafficStatistic.Attempts;
            dr["DeliveredAttempts"] = trafficStatistic.DeliveredAttempts;
            dr["SuccessfulAttempts"] = trafficStatistic.SuccessfulAttempts;
            dr["DurationsInSeconds"] = trafficStatistic.DurationsInSeconds;

            if (trafficStatistic.PDDInSeconds.HasValue)
                dr["PDDInSeconds"] = trafficStatistic.PDDInSeconds.Value;
            else
                dr["PDDInSeconds"] = DBNull.Value;

            dr["MaxDurationInSeconds"] = trafficStatistic.MaxDurationInSeconds;
            dr["UtilizationInSeconds"] = trafficStatistic.UtilizationInSeconds;
            dr["NumberOfCalls"] = trafficStatistic.NumberOfCalls;
            dr["DeliveredNumberOfCalls"] = trafficStatistic.DeliveredNumberOfCalls;
            dr["PGAD"] = trafficStatistic.PGAD;
            dr["CeiledDuration"] = trafficStatistic.CeiledDuration;
            dr["ReleaseSourceAParty"] = trafficStatistic.ReleaseSourceAParty;
            dr["ReleaseSourceS"] = trafficStatistic.ReleaseSourceS;
        }

        DataTable GetTrafficStatsTable()
        {
            DataTable dt = new DataTable(TRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("FirstCDRAttempt", typeof(DateTime));
            dt.Columns.Add("LastCDRAttempt", typeof(DateTime));
            dt.Columns.Add("Attempts", typeof(int));
            dt.Columns.Add("DeliveredAttempts", typeof(int));
            dt.Columns.Add("SuccessfulAttempts", typeof(int));
            dt.Columns.Add("DurationsInSeconds", typeof(decimal));
            dt.Columns.Add("PDDInSeconds", typeof(decimal));
            dt.Columns.Add("MaxDurationInSeconds", typeof(decimal));
            dt.Columns.Add("UtilizationInSeconds", typeof(decimal));
            dt.Columns.Add("NumberOfCalls", typeof(int));
            dt.Columns.Add("DeliveredNumberOfCalls", typeof(int));
            dt.Columns.Add("PGAD", typeof(decimal));
            dt.Columns.Add("CeiledDuration", typeof(int));
            dt.Columns.Add("ReleaseSourceAParty", typeof(int));
            dt.Columns.Add("ReleaseSourceS", typeof(int));
            return dt;
        }

        private Dictionary<string, int> GetTrafficStatisticsIdsByGroupKeys(DateTime batchStart, DateTime batchEnd)
        {
            Dictionary<string, int> trafficStatistics = new Dictionary<string, int>();

            ExecuteReaderSP("Analytics.sp_TrafficStats_GetIdsByGroupedKeys", (reader) =>
            {
                while (reader.Read())
                {
                    trafficStatistics.Add(
                    TrafficStatistic.GetGroupKey(Int32.Parse(reader["SwitchId"].ToString().Trim()),
                        reader["Port_IN"] as string,
                        reader["Port_OUT"] as string,
                        reader["CustomerID"] as string,
                        (int)reader["OurZoneID"],
                        (int)reader["OriginatingZoneID"],
                        (int)reader["SupplierZoneID"]),  Int32.Parse(reader["ID"].ToString().Trim()));
                }
            }, batchStart, batchEnd);

            return trafficStatistics;
        }

        #endregion


        #region DAILY TRAFFIC STAT

        const string DAILYTRAFFICSTATISTIC_TABLENAME = "TrafficStatsDaily";

        public void UpdateTrafficStatisticDailyBatch(DateTime batchDate, TrafficStatisticsDailyByKey trafficStatisticsByKey)
        {
            Dictionary<string, int> existingItemsIdByGroupKeys = GetDailyTrafficStatisticsIdsByGroupKeys(batchDate);

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            DataTable dtStatisticsToUpdate = GetDailyTrafficStatsTable();

            dtStatisticsToUpdate.BeginLoadData();

            foreach (var item in trafficStatisticsByKey)
            {
                TrafficStatisticDaily trafficStatistic = item.Value;
                int existingId;
                if (existingItemsIdByGroupKeys != null && existingItemsIdByGroupKeys.TryGetValue(item.Key, out existingId))
                {
                    trafficStatistic.ID = existingId;
                    DataRow dr = dtStatisticsToUpdate.NewRow();
                    FillDailyStatisticRow(dr, trafficStatistic);
                    dtStatisticsToUpdate.Rows.Add(dr);
                }
                else
                {
                    AddDailyTrafficStatisticToStream(stream, trafficStatistic);
                }
            }
            stream.Close();

            dtStatisticsToUpdate.EndLoadData();

            if (dtStatisticsToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[Analytics].[sp_TrafficStatsDaily_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@TrafficStats", SqlDbType.Structured);
                        dtPrm.Value = dtStatisticsToUpdate;
                        cmd.Parameters.Add(dtPrm);
                    });

            base.InsertBulkToTable(new Vanrise.Data.SQL.StreamBulkInsertInfo
            {
                Stream = stream,
                TableName = DAILYTRAFFICSTATISTIC_TABLENAME,
                TabLock = false,
                FieldSeparator = '^'
            });
        }

        private void AddDailyTrafficStatisticToStream(StreamForBulkInsert stream, TrafficStatisticDaily trafficStatistic)
        {
            stream.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}",
                trafficStatistic.CallDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                trafficStatistic.SwitchId,
                trafficStatistic.CustomerId,
                trafficStatistic.OurZoneId,
                trafficStatistic.OriginatingZoneId,
                trafficStatistic.SupplierId,
                trafficStatistic.SupplierZoneId,
                trafficStatistic.Attempts,
                trafficStatistic.DeliveredAttempts,
                trafficStatistic.SuccessfulAttempts,
                trafficStatistic.DurationsInSeconds,
                trafficStatistic.PDDInSeconds,
                trafficStatistic.UtilizationInSeconds,
                trafficStatistic.NumberOfCalls,
                trafficStatistic.DeliveredNumberOfCalls,
                trafficStatistic.PGAD,
                trafficStatistic.CeiledDuration,
                trafficStatistic.MaxDurationInSeconds,
                trafficStatistic.ReleaseSourceAParty,
                trafficStatistic.ReleaseSourceS);
        }

        void FillDailyStatisticRow(DataRow dr, TrafficStatisticDaily trafficStatistic)
        {
            dr["ID"] = trafficStatistic.ID;
            dr["Attempts"] = trafficStatistic.Attempts;
            dr["DeliveredAttempts"] = trafficStatistic.DeliveredAttempts;
            dr["SuccessfulAttempts"] = trafficStatistic.SuccessfulAttempts;
            dr["DurationsInSeconds"] = trafficStatistic.DurationsInSeconds;

            if (trafficStatistic.PDDInSeconds.HasValue)
                dr["PDDInSeconds"] = trafficStatistic.PDDInSeconds.Value;
            else
                dr["PDDInSeconds"] = DBNull.Value;

            dr["MaxDurationInSeconds"] = trafficStatistic.MaxDurationInSeconds;
            dr["UtilizationInSeconds"] = trafficStatistic.UtilizationInSeconds;
            dr["NumberOfCalls"] = trafficStatistic.NumberOfCalls;
            dr["DeliveredNumberOfCalls"] = trafficStatistic.DeliveredNumberOfCalls;
            dr["PGAD"] = trafficStatistic.PGAD;
            dr["CeiledDuration"] = trafficStatistic.CeiledDuration;
            dr["ReleaseSourceAParty"] = trafficStatistic.ReleaseSourceAParty;
            dr["ReleaseSourceS"] = trafficStatistic.ReleaseSourceS;
        }

        DataTable GetDailyTrafficStatsTable()
        {
            DataTable dt = new DataTable(DAILYTRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Attempts", typeof(int));
            dt.Columns.Add("DeliveredAttempts", typeof(int));
            dt.Columns.Add("SuccessfulAttempts", typeof(int));
            dt.Columns.Add("DurationsInSeconds", typeof(decimal));
            dt.Columns.Add("PDDInSeconds", typeof(decimal));
            dt.Columns.Add("UtilizationInSeconds", typeof(decimal));
            dt.Columns.Add("NumberOfCalls", typeof(int));
            dt.Columns.Add("DeliveredNumberOfCalls", typeof(int));
            dt.Columns.Add("PGAD", typeof(decimal));
            dt.Columns.Add("CeiledDuration", typeof(int));
            dt.Columns.Add("MaxDurationInSeconds", typeof(decimal));
            dt.Columns.Add("ReleaseSourceAParty", typeof(int));
            dt.Columns.Add("ReleaseSourceS", typeof(int));
            return dt;
        }

        private Dictionary<string, int> GetDailyTrafficStatisticsIdsByGroupKeys(DateTime batchDate)
        {
            Dictionary<string, int> trafficStatistics = new Dictionary<string, int>();

            ExecuteReaderSP("Analytics.sp_TrafficStatsDaily_GetIdsByGroupedKeys", (reader) =>
            {
                while (reader.Read())
                {
                    trafficStatistics.Add(
                    TrafficStatisticDaily.GetGroupKey(Int32.Parse(reader["SwitchId"].ToString().Trim()),
                        reader["CustomerID"] as string,
                        (int)reader["OurZoneID"],
                        (int)reader["OriginatingZoneID"],
                        (int)reader["SupplierZoneID"]), Int32.Parse(reader["ID"].ToString().Trim()));
                }
            }, batchDate);

            return trafficStatistics;
        }

        #endregion



        public void ApplyTrafficStatsForDB(object preparedTrafficStats)
        {
            InsertBulkToTable(preparedTrafficStats as BaseBulkInsertInfo);
        }

        public void SaveTrafficStatsForDB(List<TrafficStatistic> trafficStatistics)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (TrafficStatistic trafficStatistic in trafficStatistics)
                WriteRecordToStream(trafficStatistic, dbApplyStream);
            Object preparedtrafficStatistics = FinishDBApplyStream(dbApplyStream);
            ApplyTrafficStatsForDB(preparedtrafficStatistics);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[TrafficStats]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(TrafficStatistic record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}",
                       0,
                       record.SwitchId,
                       record.Port_IN,
                       record.Port_OUT,
                       record.CustomerId,
                       record.OurZoneId,
                       record.OriginatingZoneId,
                       record.SupplierId,
                       record.SupplierZoneId,
                       record.FirstCDRAttempt,
                       record.LastCDRAttempt,
                       record.Attempts,
                       record.DeliveredAttempts,
                       record.SuccessfulAttempts,
                       record.DurationsInSeconds,
                       record.PDDInSeconds,
                       record.MaxDurationInSeconds,
                       record.UtilizationInSeconds,
                       record.NumberOfCalls,
                       record.DeliveredNumberOfCalls,
                       Math.Round(record.PGAD, 5),
                       record.CeiledDuration,
                       record.ReleaseSourceAParty,
                       record.ReleaseSourceS
                       );
        
        }
    }
}