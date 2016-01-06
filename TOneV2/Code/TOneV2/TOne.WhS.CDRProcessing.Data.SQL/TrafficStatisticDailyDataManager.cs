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
      
        #region ctor/Local Variables
        public TrafficStatisticDailyDataManager(): base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        const string TRAFFICSTATISTIC_TABLENAME = "TrafficStatsDaily";
        readonly string[] columns =  { "ID", "CustomerID", "SupplierID", "Attempts", "DurationInSeconds", "FirstCDRAttempt", "LastCDRAttempt", "SaleZoneID", "SupplierZoneID", "SumOfPDDInSeconds", "MaxDurationInSeconds", "NumberOfCalls", "PortOut", "PortIn", "DeliveredAttempts", "SuccessfulAttempts", "DeliveredNumberOfCalls", "CeiledDuration", "SwitchID", "SumOfPGAD" };
      
        #endregion

        #region Public Methods
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
                ExecuteNonQuerySPCmd("[TOneWhS_Analytic].[sp_TrafficStatsDaily_Update]",
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

            ExecuteReaderSP("[TOneWhS_Analytic].sp_TrafficStatsDaily_GetIdsByGroupedKeys", (reader) =>
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
                             GetReaderValue<string>(reader, "PortIn"),
                             GetReaderValue<int>(reader, "SwitchID"));
                    if (!trafficStatistics.ContainsKey(key))
                        trafficStatistics.Add(key, GetReaderValue<long>(reader, "ID"));
                }
            }, batch.BatchStart, batch.BatchEnd);

            return trafficStatistics;
        }

        #endregion

        #region Private Methods
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(TrafficStatisticDaily record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}",
                                     record.StatisticItemId,
                                     record.CustomerId,
                                     record.SupplierId,
                                     record.Attempts,
                                     record.DurationInSeconds,
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
                                    record.SuccessfulAttempts,
                                    record.DeliveredNumberOfCalls,
                                    record.CeiledDuration,
                                    record.SwitchID,
                                    record.PGAD);

        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = columns,
                TableName = "[TOneWhS_Analytic].[TrafficStatsDaily]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private void ApplyTrafficStatsToDB(Object trafficStatsBatch)
        {
            InsertBulkToTable(trafficStatsBatch as StreamBulkInsertInfo);
        }
        private void FillStatisticRow(DataRow dr, TrafficStatisticDaily trafficStatistic)
        {
            dr["ID"] = trafficStatistic.StatisticItemId;
            dr["CustomerId"] = trafficStatistic.CustomerId;
            dr["SupplierId"] = trafficStatistic.SupplierId;
            dr["Attempts"] = trafficStatistic.Attempts;
            dr["TotalDurationInSeconds"] = trafficStatistic.DurationInSeconds;
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
            dr["DeliveredNumberOfCalls"] = trafficStatistic.DeliveredNumberOfCalls;
            dr["CeiledDuration"] = trafficStatistic.CeiledDuration;
            dr["SumOfPGAD"] = trafficStatistic.PGAD;
            dr["UtilizationInSeconds"] = trafficStatistic.Utilization;
        }
        private DataTable GetTrafficStatsDailyTable()
        {
            DataTable dt = new DataTable(TRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("CustomerId", typeof(int));
            dt.Columns.Add("SupplierId", typeof(int));
            dt.Columns.Add("Attempts", typeof(int));
            dt.Columns.Add("TotalDurationInSeconds", typeof(Decimal));
            dt.Columns.Add("FirstCDRAttempt", typeof(DateTime));
            dt.Columns.Add("LastCDRAttempt", typeof(DateTime));
            dt.Columns.Add("SaleZoneID", typeof(long));
            dt.Columns.Add("SupplierZoneID", typeof(long));
            dt.Columns.Add("PDDInSeconds", typeof(Decimal));
            dt.Columns.Add("MaxDurationInSeconds", typeof(Decimal));
            dt.Columns.Add("NumberOfCalls", typeof(int));
            dt.Columns.Add("PortOut", typeof(string));
            dt.Columns.Add("PortIn", typeof(string));
            dt.Columns.Add("DeliveredAttempts", typeof(int));
            dt.Columns.Add("SuccessfulAttempts", typeof(int));
            dt.Columns.Add("DeliveredNumberOfCalls", typeof(int));
            dt.Columns.Add("CeiledDuration", typeof(long));
            dt.Columns.Add("SumOfPGAD", typeof(Decimal));
            dt.Columns.Add("UtilizationInSeconds", typeof(Decimal));
            return dt;
        }

        #endregion
      
    }
}
