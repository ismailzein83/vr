using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class BlockedAttemptsDataManager:BaseTOneDataManager, ICDRDataManager
    {
        public Vanrise.Entities.BigResult<string> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SwitchName", "SwitchID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText("", (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query));
                    cmd.Parameters.Add(new SqlParameter("@nRecords", input.Query));
                });
            };

            return RetrieveData(input, createTempTableAction, CDRDataMapper, mapper);
        }
        public string CreateTempTableIfNotExists(string tempTableName, BlockedAttemptsFilter filter)
        {

        }
    }
}
