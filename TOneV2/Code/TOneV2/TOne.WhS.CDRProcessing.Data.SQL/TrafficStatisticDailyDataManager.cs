using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
    public class TrafficStatisticDailyDataManager:BaseSQLDataManager,ITrafficStatisticDailyDataManager
    { 
        const string TRAFFICSTATISTIC_TABLENAME = "TrafficStatsDaily";
        public TrafficStatisticDailyDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(TrafficStatisticDaily record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}",
                                     record.StatisticItemId,
                                     record.CustomerId,
                                     record.SupplierId,
                                     record.Attempts,
                                     record.TotalDurationInSeconds,
                                    record.FirstCDRAttempt,
                                    record.LastCDRAttempt,
                                     record.SaleZoneId,
                                    record.SupplierZoneId,
                                    record.PDDInSeconds,
                                    record.MaxDurationInSeconds,
                                    record.NumberOfCalls,
                                    record.PortOut,
                                    record.PortIn,
                                    record.DeliveredAttempts,
                                    record.SuccessfulAttempts);

        }
        public object FinishDBApplyStream(object dbApplyStream)
        {

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_CDR].[TrafficStatsDaily]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplyTrafficStatsToDB(Object trafficStatsBatch)
        {
            InsertBulkToTable(trafficStatsBatch as StreamBulkInsertInfo);
        }
        void FillStatisticRow(DataRow dr, TrafficStatisticDaily trafficStatistic)
        {
            dr["ID"] = trafficStatistic.StatisticItemId;
            dr["CustomerId"] = trafficStatistic.CustomerId;
            dr["SupplierId"] = trafficStatistic.SupplierId;
            dr["Attempts"] = trafficStatistic.Attempts;
            dr["TotalDurationInSeconds"] = trafficStatistic.TotalDurationInSeconds;
            dr["FirstCDRAttempt"] = trafficStatistic.FirstCDRAttempt;
            dr["LastCDRAttempt"] = trafficStatistic.LastCDRAttempt;
            dr["SaleZoneID"] = trafficStatistic.SaleZoneId;
            dr["SupplierZoneID"] = trafficStatistic.SupplierZoneId;
            dr["PDDInSeconds"] = trafficStatistic.PDDInSeconds;
            dr["MaxDurationInSeconds"] = trafficStatistic.MaxDurationInSeconds;
            dr["NumberOfCalls"] = trafficStatistic.NumberOfCalls;
            dr["PortOut"] = trafficStatistic.PortOut;
            dr["PortIn"] = trafficStatistic.PortIn;
            dr["DeliveredAttempts"] = trafficStatistic.DeliveredAttempts;
            dr["SuccessfulAttempts"] = trafficStatistic.SuccessfulAttempts;
        }
        DataTable GetTrafficStatsDailyTable()
        {
            DataTable dt = new DataTable(TRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("CustomerId", typeof(int));
            dt.Columns.Add("SupplierId", typeof(int));
            dt.Columns.Add("Attempts", typeof(int));
            dt.Columns.Add("TotalDurationInSeconds", typeof(int));
            dt.Columns.Add("FirstCDRAttempt", typeof(DateTime));
            dt.Columns.Add("LastCDRAttempt", typeof(DateTime));
            dt.Columns.Add("SaleZoneID", typeof(long));
            dt.Columns.Add("SupplierZoneID", typeof(long));
            dt.Columns.Add("PDDInSeconds", typeof(int));
            dt.Columns.Add("MaxDurationInSeconds", typeof(int));
            dt.Columns.Add("NumberOfCalls", typeof(int));
            dt.Columns.Add("PortOut", typeof(string));
            dt.Columns.Add("PortIn", typeof(string));
            dt.Columns.Add("DeliveredAttempts", typeof(int));
            dt.Columns.Add("SuccessfulAttempts", typeof(int));
            return dt;
        }




        public void InsertStatisticItemsToDB(IEnumerable<Entities.TrafficStatisticDaily> items)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (var itm in items)
                WriteRecordToStream(itm, dbApplyStream);
            Object preparedTrafficStats = FinishDBApplyStream(dbApplyStream);
            ApplyTrafficStatsToDB(preparedTrafficStats);
        }

        public void UpdateStatisticItemsInDB(IEnumerable<Entities.TrafficStatisticDaily> items)
        {
            DataTable dtStatisticsToUpdate = GetTrafficStatsDailyTable();

            dtStatisticsToUpdate.BeginLoadData();

            foreach (var item in items)
            {
                DataRow dr = dtStatisticsToUpdate.NewRow();
                FillStatisticRow(dr, item);
                dtStatisticsToUpdate.Rows.Add(dr);

            }
            dtStatisticsToUpdate.EndLoadData();
            if (dtStatisticsToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[TOneWhS_CDR].[sp_TrafficStatsDaily_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@TrafficStats", SqlDbType.Structured);
                        dtPrm.Value = dtStatisticsToUpdate;
                        cmd.Parameters.Add(dtPrm);
                    });
        }

        public Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(Entities.TrafficStatisticDailyBatch batch)
        {
            Dictionary<string, long> trafficStatistics = new Dictionary<string, long>();

            ExecuteReaderSP("TOneWhS_CDR.sp_TrafficStatsDaily_GetIdsByGroupedKeys", (reader) =>
            {
                string key = null;
                while (reader.Read())
                {
                    key = TrafficStatisticDaily.GetStatisticItemKey(
                             GetReaderValue<int>(reader, "CustomerID"),
                             GetReaderValue<int>(reader, "SupplierID"),
                             GetReaderValue<long>(reader, "SaleZoneID"),
                             GetReaderValue<long>(reader, "SupplierZoneID"),
                             GetReaderValue<string>(reader, "PortOut"),
                             GetReaderValue<string>(reader, "PortIn"));
                    if (!trafficStatistics.ContainsKey(key))
                        trafficStatistics.Add(key, GetReaderValue<long>(reader, "ID"));
                }
            }, batch.BatchStart, batch.BatchEnd);

            return trafficStatistics;
        }
    }
}
