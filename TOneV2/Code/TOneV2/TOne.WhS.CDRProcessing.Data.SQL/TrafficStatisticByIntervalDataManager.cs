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
    public class TrafficStatisticByIntervalDataManager : BaseSQLDataManager, ITrafficStatisticByIntervalDataManager
    {
        const string TRAFFICSTATISTIC_TABLENAME = "TrafficStats";
        public TrafficStatisticByIntervalDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
         
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(TrafficStatisticByInterval record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                                    record.StatisticItemId,
                                    record.CustomerId,
                                    record.SupplierId,
                                    record.Attempts,
                                    record.TotalDurationInSeconds);

       }
       public object FinishDBApplyStream(object dbApplyStream)
       {

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               TableName = "[TOneWhS_CDR].[TrafficStats]",
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

       public void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticByInterval> items)
       {
           Object dbApplyStream = InitialiazeStreamForDBApply();
           foreach (var itm in items)
               WriteRecordToStream(itm, dbApplyStream);
           Object preparedTrafficStats = FinishDBApplyStream(dbApplyStream);
           ApplyTrafficStatsToDB(preparedTrafficStats);
       }

       public void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticByInterval> items)
       {
           DataTable dtStatisticsToUpdate = GetTrafficStatsTable();

           dtStatisticsToUpdate.BeginLoadData();

           foreach (var item in items)
           {
                   DataRow dr = dtStatisticsToUpdate.NewRow();
                   FillStatisticRow(dr, item);
                   dtStatisticsToUpdate.Rows.Add(dr);

           }
           dtStatisticsToUpdate.EndLoadData();
           if (dtStatisticsToUpdate.Rows.Count > 0)
               ExecuteNonQuerySPCmd("[TOneWhS_CDR].[sp_TrafficStats_Update]",
                   (cmd) =>
                   {
                       var dtPrm = new System.Data.SqlClient.SqlParameter("@TrafficStats", SqlDbType.Structured);
                       dtPrm.Value = dtStatisticsToUpdate;
                       cmd.Parameters.Add(dtPrm);
                   });
       }
       void FillStatisticRow(DataRow dr, TrafficStatisticByInterval trafficStatistic)
       {
           dr["ID"] = trafficStatistic.StatisticItemId;
           dr["CustomerId"] = trafficStatistic.CustomerId;
           dr["SupplierId"] = trafficStatistic.SupplierId;
           dr["Attempts"] = trafficStatistic.Attempts;
           dr["TotalDurationInSeconds"] = trafficStatistic.TotalDurationInSeconds;
       }
       DataTable GetTrafficStatsTable()
       {
           DataTable dt = new DataTable(TRAFFICSTATISTIC_TABLENAME);
           dt.Columns.Add("ID", typeof(int));
           dt.Columns.Add("CustomerId", typeof(int));
           dt.Columns.Add("SupplierId", typeof(int));
           dt.Columns.Add("Attempts", typeof(DateTime));
           dt.Columns.Add("TotalDurationInSeconds", typeof(int));
           return dt;
       }
    }
}
