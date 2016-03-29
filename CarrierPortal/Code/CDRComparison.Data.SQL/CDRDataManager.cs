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
        static string[] s_Columns = new string[]
        {
            "ID",
            "CDPN",
            "CGPN",
            "Time",
            "DurationInSec",
            "IsPartnerCDR"
        };

        public void LoadCDRs(Action<CDR> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionItem_GetByNULLCaseID", (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(new CDR()
                    {
                        CDPN = (reader["CDPN"] as string),
                        CGPN = (reader["CGPN"] as string),
                        DurationInSec = (GetReaderValue<Decimal>(reader, "Duration")),
                        Time = (GetReaderValue<DateTime>(reader, "AttemptTime")),
                    });
                }
            });
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "0^{0}^{1}^{2}^{3}^{4}",
                record.CDPN,
                record.CGPN,
                record.Time,
                record.DurationInSec
                //record.IsPartnerCDR
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
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }
    }
}
