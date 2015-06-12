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
        const string TRAFFICSTATISTIC_TABLENAME = "TrafficStats";

        public void UpdateTrafficStatisticBatch(DateTime batchStart, DateTime batchEnd, TrafficStatisticsByKey trafficStatisticsByKey)
        {
            Dictionary<string, int> existingItemsIdByGroupKeys = GetTrafficStatisticsIdsByGroupKeys(batchStart, batchEnd);
            
            string filePathNewStatistics = GetFilePathForBulkInsert();
            DataTable dtStatisticsToUpdate = GetTrafficStatsTable();
            
            dtStatisticsToUpdate.BeginLoadData();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePathNewStatistics))
            {
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
                        AddTrafficStatisticToStream(wr, trafficStatistic);
                    }
                }
                wr.Close();
            }
            dtStatisticsToUpdate.EndLoadData();
            
            ExecuteNonQuerySPCmd("Update",
                (cmd) =>
                {
                });

            base.InsertBulkToTable(new Vanrise.Data.SQL.BulkInsertInfo
                {
                    DataFilePath = filePathNewStatistics,
                    TableName = TRAFFICSTATISTIC_TABLENAME,
                    FieldSeparator = '^'
                });
        }


        private void PrepareTrafficStatsBaseForDBApply(TOne.CDR.Entities.TrafficStatistic trafficStatistic, System.IO.StreamWriter wr)
        {
            wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}",
                        0,
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
                        Math.Round(trafficStatistic.PGAD, 5),
                        trafficStatistic.CeiledDuration,
                        trafficStatistic.ReleaseSourceAParty
                        ));
        }

        public Object PrepareTrafficStatsForDBApply(List<TOne.CDR.Entities.TrafficStatistic> trafficStatistics)
        {
            string filePath = GetFilePathForBulkInsert();

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (TOne.CDR.Entities.TrafficStatistic trafficStatistic in trafficStatistics)
                {
                    PrepareTrafficStatsBaseForDBApply(trafficStatistic, wr);
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "[dbo].[TrafficStats]",
                DataFilePath = filePath,
                TabLock = false,
                FieldSeparator = '^'
            };
        }

        private void AddTrafficStatisticToStream(System.IO.StreamWriter wr, TrafficStatistic trafficStatistic)
        {
            wr.WriteLine("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^",
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
                trafficStatistic.ReleaseSourceAParty);
        }

        void FillStatisticRow(DataRow dr, TrafficStatistic trafficStatistic)
        {
            dr["ID"] = trafficStatistic.ID;
            dr["SwitchId"] = (short)trafficStatistic.SwitchId;
            dr["Port_IN"] = trafficStatistic.Port_IN;
            dr["Port_OUT"] = trafficStatistic.Port_OUT;
            dr["CustomerID"] = trafficStatistic.CustomerId;
            dr["OurZoneID"] = trafficStatistic.OurZoneId;
            dr["OriginatingZoneID"] = trafficStatistic.OriginatingZoneId;
            dr["SupplierID"] = trafficStatistic.SupplierId;
            dr["SupplierZoneID"] = trafficStatistic.SupplierZoneId;
            dr["FirstCDRAttempt"] = trafficStatistic.FirstCDRAttempt;
            dr["LastCDRAttempt"] = trafficStatistic.LastCDRAttempt;
            dr["Attempts"] = trafficStatistic.Attempts;
            dr["DeliveredAttempts"] = trafficStatistic.DeliveredAttempts;
            dr["SuccessfulAttempts"] = trafficStatistic.SuccessfulAttempts;
            dr["DurationsInSeconds"] = trafficStatistic.DurationsInSeconds;
            dr["PDDInSeconds"] = trafficStatistic.PDDInSeconds;
            dr["MaxDurationInSeconds"] = trafficStatistic.MaxDurationInSeconds;
            dr["UtilizationInSeconds"] = trafficStatistic.UtilizationInSeconds;
            dr["NumberOfCalls"] = trafficStatistic.NumberOfCalls;
            dr["DeliveredNumberOfCalls"] = trafficStatistic.DeliveredNumberOfCalls;
            dr["PGAD"] = trafficStatistic.PGAD;
            dr["CeiledDuration"] = trafficStatistic.CeiledDuration;
            dr["ReleaseSourceAParty"] = trafficStatistic.ReleaseSourceAParty;
        }

        DataTable GetTrafficStatsTable()
        {
            DataTable dt = new DataTable(TRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("SwitchId", typeof(short));
            dt.Columns.Add("Port_IN", typeof(string));
            dt.Columns.Add("Port_OUT", typeof(string));
            dt.Columns.Add("CustomerID", typeof(string));
            dt.Columns.Add("OurZoneID", typeof(int));
            dt.Columns.Add("OriginatingZoneID", typeof(int));
            dt.Columns.Add("SupplierID", typeof(string));
            dt.Columns.Add("SupplierZoneID", typeof(int));
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
            dt.Columns.Add("PGAD", typeof(int));
            dt.Columns.Add("CeiledDuration", typeof(int));
            dt.Columns.Add("ReleaseSourceAParty", typeof(int));
            return dt;
        }

        private Dictionary<string, int> GetTrafficStatisticsIdsByGroupKeys(DateTime batchStart, DateTime batchEnd)
        {
            Dictionary<string, int> trafficStatistics = new Dictionary<string, int>();

            ExecuteReaderSP("[Analytics].[sp_TrafficStats_GetIdsByGroupedKeys]", (reader) =>
            {
                while (reader.Read())
                {
                    trafficStatistics.Add(
                    TrafficStatistic.GetGroupKey(GetReaderValue<int>(reader, "SwitchId"),
                        reader["Port_IN"] as string,
                        reader["Port_OUT"] as string,
                        reader["CustomerID"] as string,
                        GetReaderValue<int>(reader, "OurZoneID"),
                        GetReaderValue<int>(reader, "OriginatingZoneID"),
                        GetReaderValue<int>(reader, "SupplierZoneID")), (int)reader["ID"]);
                }
            }, batchStart, batchEnd);

            return trafficStatistics;
        }

        public void UpdateTrafficStatisticDailyBatch(DateTime batchStart, DateTime batchEnd, TrafficStatisticsDailyByKey trafficStatisticsByKey)
        {
            //Same as UpdateTrafficStatisticBatch for the TrafficStatsDaily
            throw new NotImplementedException();
        }
    }
}