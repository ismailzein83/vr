using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        static string[] s_Columns = new string[]
        {
            "CDPN",
            "CGPN",
             "IsPartnerCDR",
            "AttemptTime",
            "Duration",
           
        };

        public void LoadCDRs(Action<CDR> onBatchReady)
        {
            ExecuteReaderText(GetLoadCDRsQuery(), (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(new CDR()
                    {
                        CDPN = (reader["CDPN"] as string),
                        CGPN = (reader["CGPN"] as string),
                        DurationInSec = (GetReaderValue<Decimal>(reader, "Duration")),
                        Time = (GetReaderValue<DateTime>(reader, "AttemptTime")),
                        IsPartnerCDR = (GetReaderValue<Boolean>(reader, "IsPartnerCDR")),
                    });
                }
            },null);
        }
        private string GetLoadCDRsQuery()
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT [ID],
		                                 [CDPN],
		                                 [CGPN],
		                                 [IsPartnerCDR],
		                                 [AttemptTime],
		                                 [Duration]
	                               FROM  [CDRComparison_Dev].[dbo].[CDR]
	                               ORDER BY [CDPN]");
            return query.ToString();
        }
        public int GetAllCDRsCount()
        {
            object count = ExecuteScalarText("SELECT COUNT(*) FROM dbo.CDR",null);
            return (int)count;
        }


        #region Bulk Insert Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}",
                record.CDPN,
                record.CGPN,
                record.IsPartnerCDR ? "1" : "0",
                record.Time,
                record.DurationInSec
               
            );
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CDR]",
                Stream = streamForBulkInsert,
                ColumnNames = s_Columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BaseBulkInsertInfo);
        }

        #endregion
    }
}
