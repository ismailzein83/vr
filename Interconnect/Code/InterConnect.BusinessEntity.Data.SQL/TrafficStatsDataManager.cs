using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace InterConnect.BusinessEntity.Data.SQL
{
    public class TrafficStatsDataManager : BaseSQLDataManager, ITrafficStatsDataManager
    {
        public TrafficStatsDataManager()
            : base(GetConnectionStringName("Interconnect_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public IEnumerable<TrafficStats> GetTrafficStats(DateTime batchStart)
        {
            string query = @"SELECT  [ID]
      ,[BatchStart]
    ,CDPN
      ,[OperatorId]
      ,[TotalDuration]
      ,[Attempts]
  FROM [dbo].[TrafficStats] WHERE BatchStart = @BatchStart";
            return GetItemsText(query,
               (reader) =>
               {
                   return new Entities.TrafficStats
                   {
                       SummaryItemId = (long)reader["ID"],
                       Attempts = GetReaderValue<int>(reader, "Attempts"),
                       BatchStart = GetReaderValue<DateTime>(reader, "BatchStart"),
                       CDPN = reader["CDPN"] as string,
                       OperatorId = GetReaderValue<int>(reader, "OperatorId"),
                       TotalDuration = GetReaderValue<decimal>(reader, "TotalDuration")
                   };
               },
               (cmd) =>
               {
                   cmd.Parameters.Add(new SqlParameter("@BatchStart", batchStart));
               });
        }

        public void Insert(List<Entities.TrafficStats> itemsToAdd)
        {
            var stream = InitializeStreamForBulkInsert();
            foreach(var itm in itemsToAdd)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", itm.SummaryItemId, itm.BatchStart, itm.CDPN, itm.OperatorId, itm.TotalDuration, itm.Attempts);
            }
            stream.Close();
            var dbApplyStream = new Vanrise.Data.SQL.StreamBulkInsertInfo
            {
                Stream = stream,
                FieldSeparator = '^',
                TableName = "TrafficStats",
                ColumnNames = new String[] { "ID", "BatchStart", "CDPN", "OperatorId", "TotalDuration", "Attempts" }
            };
            base.InsertBulkToTable(dbApplyStream);
        }

        public void Update(List<Entities.TrafficStats> itemsToUpdate)
        {
            string query = @"UPDATE ts
                                SET Attempts = updatedTS.Attempts, TotalDuration = updatedTS.TotalDuration
                                FROM TrafficStats ts JOIN @UpdateTS updatedTS ON ts.ID = updatedTS.ID";
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("BatchStart", typeof(DateTime));
            dt.Columns.Add("CDPN", typeof(string));
            dt.Columns.Add("OperatorId", typeof(int));
            dt.Columns.Add("TotalDuration", typeof(decimal));
            dt.Columns.Add("Attempts", typeof(int));
            dt.BeginLoadData();
            foreach(var itm in itemsToUpdate)
            {
                var dr = dt.NewRow();
                dr["ID"] = itm.SummaryItemId;
                dr["BatchStart"] = itm.BatchStart;
                dr["CDPN"] = itm.CDPN;
                dr["OperatorId"] = itm.OperatorId;
                dr["TotalDuration"] = itm.TotalDuration;
                dr["Attempts"] = itm.Attempts;
                dt.Rows.Add(dr);
            }
            dt.EndLoadData();
            ExecuteNonQueryText(query,
                (cmd) =>
                {
                    SqlParameter prm = new SqlParameter("@UpdateTS", System.Data.SqlDbType.Structured);
                    prm.TypeName = "dbo.TrafficStatsTable";
                    prm.Value = dt;
                    cmd.Parameters.Add(prm);
                });
        }
    }
}
