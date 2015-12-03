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
    public class BillingStatisticDataManager : BaseSQLDataManager, IBillingStatisticDataManager
    {

        #region ctor/Local Variables
        public BillingStatisticDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        const string BILLINGTRAFFICSTATISTIC_TABLENAME = "TrafficStatsBilling";
        readonly string[] columns = { "ID", "CallDate", "CustomerID", "SupplierID", "SupplierZoneID", "SaleZoneID", "CostCurrency", "SaleCurrency", "NumberOfCalls", "FirstCallTime", "LastCallTime", "MinDuration", "MaxDuration", "AvgDuration", "CostNets", "CostExtraCharges", "SaleNets", "SaleExtraCharges", "SaleRate", "CostRate", "SaleDuration", "CostDuration", "SaleRateType", "CostRateType" };

        #endregion

        #region Public Methods
        public void InsertBillingStatisticItemsToDB(IEnumerable<Entities.BillingStatistic> items)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (var itm in items)
                WriteRecordToStream(itm, dbApplyStream);
            Object preparedBillingStats = FinishDBApplyStream(dbApplyStream);
            ApplyBillingStatsToDB(preparedBillingStats);
        }
        public void UpdateBillingStatisticItemsInDB(IEnumerable<Entities.BillingStatistic> items)
        {
            DataTable dtStatisticsToUpdate = GetBillingTrafficStatsDailyTable();

            dtStatisticsToUpdate.BeginLoadData();

            foreach (var item in items)
            {
                DataRow dr = dtStatisticsToUpdate.NewRow();
                FillStatisticRow(dr, item);
                dtStatisticsToUpdate.Rows.Add(dr);

            }
            dtStatisticsToUpdate.EndLoadData();
            if (dtStatisticsToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[TOneWhS_Analytic].[sp_BillingStats_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@BillingStats", SqlDbType.Structured);
                        dtPrm.Value = dtStatisticsToUpdate;
                        cmd.Parameters.Add(dtPrm);
                    });
        }
        public Dictionary<string, long> GetBillingStatisticItemsIdsByKeyFromDB(Entities.BillingStatisticBatch batch)
        {
            Dictionary<string, long> trafficStatistics = new Dictionary<string, long>();

            ExecuteReaderSP("[TOneWhS_Analytic].sp_BillingStats_GetIdsByGroupedKeys", (reader) =>
            {

                string key = null;
                while (reader.Read())
                {
                    key = BillingStatistic.GetStatisticItemKey(
                             GetReaderValue<int>(reader, "CustomerID"),
                             GetReaderValue<int>(reader, "SupplierID"),
                             GetReaderValue<long>(reader, "SaleZoneID"),
                             GetReaderValue<long>(reader, "SupplierZoneID"),
                             GetReaderValue<int>(reader, "CostCurrency"),
                             GetReaderValue<int>(reader, "SaleCurrency"),
                             GetReaderValue<DateTime>(reader, "CallDate"),
                             GetReaderValue<int>(reader, "SaleRateType"),
                             GetReaderValue<int>(reader, "CostRateType"));
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
        private void WriteRecordToStream(BillingStatistic record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}",
                                     record.StatisticItemId,
                                     record.CallDate,
                                     record.CustomerId,
                                     record.SupplierId,
                                     record.SupplierZoneId,
                                     record.SaleZoneId,
                                     record.CostCurrency,
                                     record.SaleCurrency,
                                     record.NumberOfCalls,
                                     record.FirstCallTime.ToShortTimeString(),
                                     record.LastCallTime.ToShortTimeString(),
                                     record.MinDuration,
                                     record.MaxDuration,
                                     record.AvgDuration,
                                     record.CostNets,
                                     record.CostExtraCharges,
                                     record.SaleNets,
                                     record.SaleExtraCharges,
                                     record.SaleRate,
                                     record.CostRate,
                                     record.SaleDuration,
                                     record.CostDuration,
                                     record.SaleRateType,
                                     record.CostRateType);

        }
        private void ApplyBillingStatsToDB(Object billingStatsBatch)
        {
            InsertBulkToTable(billingStatsBatch as StreamBulkInsertInfo);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = columns,
                TableName = "[TOneWhS_Analytic].[BillingStats]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private void FillStatisticRow(DataRow dr, BillingStatistic trafficStatistic)
        {
            dr["ID"] = trafficStatistic.StatisticItemId;
            dr["CallDate"] = trafficStatistic.CallDate;
            dr["CustomerID"] = trafficStatistic.CustomerId;
            dr["SupplierID"] = trafficStatistic.SupplierId;
            dr["SupplierZoneID"] = trafficStatistic.SupplierZoneId;
            dr["SaleZoneID"] = trafficStatistic.SaleZoneId;
            dr["CostCurrency"] = trafficStatistic.CostCurrency;
            dr["SaleCurrency"] = trafficStatistic.SaleCurrency;
            dr["NumberOfCalls"] = trafficStatistic.NumberOfCalls;
            dr["FirstCallTime"] = trafficStatistic.FirstCallTime.ToShortTimeString();
            dr["LastCallTime"] = trafficStatistic.LastCallTime.ToShortTimeString();
            dr["MinDuration"] = trafficStatistic.MinDuration;
            dr["MaxDuration"] = trafficStatistic.MaxDuration;
            dr["AvgDuration"] = trafficStatistic.AvgDuration;
            dr["CostNets"] = trafficStatistic.CostNets;
            dr["CostExtraCharges"] = trafficStatistic.CostExtraCharges;
            dr["SaleNets"] = trafficStatistic.SaleNets;
            dr["SaleExtraCharges"] = trafficStatistic.SaleExtraCharges;
            dr["SaleRate"] = trafficStatistic.SaleRate;
            dr["CostRate"] = trafficStatistic.CostRate;
            dr["SaleDuration"] = trafficStatistic.SaleDuration;
            dr["CostDuration"] = trafficStatistic.CostDuration;
        }
        private DataTable GetBillingTrafficStatsDailyTable()
        {
            DataTable dt = new DataTable(BILLINGTRAFFICSTATISTIC_TABLENAME);
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("CallDate", typeof(DateTime));
            dt.Columns.Add("CustomerID", typeof(int));
            dt.Columns.Add("SupplierID", typeof(int));
            dt.Columns.Add("SupplierZoneID", typeof(long));
            dt.Columns.Add("SaleZoneID", typeof(long));
            dt.Columns.Add("CostCurrency", typeof(int));
            dt.Columns.Add("SaleCurrency", typeof(int));
            dt.Columns.Add("NumberOfCalls", typeof(int));
            dt.Columns.Add("FirstCallTime", typeof(string));
            dt.Columns.Add("LastCallTime", typeof(string));
            dt.Columns.Add("MinDuration", typeof(int));
            dt.Columns.Add("MaxDuration", typeof(int));
            dt.Columns.Add("AvgDuration", typeof(decimal));
            dt.Columns.Add("CostNets", typeof(decimal));
            dt.Columns.Add("CostExtraCharges", typeof(decimal));
            dt.Columns.Add("SaleNets", typeof(decimal));
            dt.Columns.Add("SaleExtraCharges", typeof(decimal));
            dt.Columns.Add("SaleRate", typeof(decimal));
            dt.Columns.Add("CostRate", typeof(decimal));
            dt.Columns.Add("SaleDuration", typeof(int));
            dt.Columns.Add("CostDuration", typeof(int));
            return dt;
        }

        #endregion

    }
}