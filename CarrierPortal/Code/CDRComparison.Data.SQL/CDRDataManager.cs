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
    }
}
