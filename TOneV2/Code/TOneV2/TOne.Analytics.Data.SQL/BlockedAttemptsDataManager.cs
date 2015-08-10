using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class BlockedAttemptsDataManager : BaseTOneDataManager, IBlockedAttemptsDataManager
    {
        public Vanrise.Entities.BigResult<string> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SwitchName", "SwitchID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText("", (cmd) =>
                {
                    //cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query));
                    //cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query));
                    //cmd.Parameters.Add(new SqlParameter("@nRecords", input.Query));
                });
            };
            RetrieveData(input, createTempTableAction, BlockedAttemptsMapper, mapper);
            return null;
        }

        TrafficStatistic BlockedAttemptsMapper(IDataReader reader)
        {
            TrafficStatistic obj = new TrafficStatistic();
            BlockedAttemptsFromReader(obj, reader);
            return obj;
        }
        void BlockedAttemptsFromReader(TrafficStatistic trafficStatistics, IDataReader reader)
        {
            trafficStatistics.FirstCDRAttempt = GetReaderValue<DateTime>(reader, "FirstCDRAttempt");
            trafficStatistics.LastCDRAttempt = GetReaderValue<DateTime>(reader, "LastCDRAttempt");
            trafficStatistics.Attempts = GetReaderValue<int>(reader, "Attempts");
            trafficStatistics.FailedAttempts = GetReaderValue<int>(reader, "FailedAttempts");
            trafficStatistics.DeliveredAttempts = GetReaderValue<int>(reader, "DeliveredAttempts");
            trafficStatistics.SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts");
            trafficStatistics.DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInSeconds") / 60;
            trafficStatistics.MaxDurationInMinutes = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds") / 60;
            trafficStatistics.CeiledDuration = GetReaderValue<long>(reader, "CeiledDuration");
            trafficStatistics.ACD = GetReaderValue<Decimal>(reader, "ACD");
            trafficStatistics.PDDInSeconds = GetReaderValue<Decimal>(reader, "PDDInSeconds");
            trafficStatistics.UtilizationInSeconds = GetReaderValue<Decimal>(reader, "UtilizationInSeconds");
            trafficStatistics.NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls");
            trafficStatistics.DeliveredNumberOfCalls = GetReaderValue<int>(reader, "DeliveredNumberOfCalls");
            trafficStatistics.PGAD = GetReaderValue<Decimal>(reader, "PGAD");
        }
        public string CreateTempTableIfNotExists(string tempTableName, BlockedAttemptsFilter filter)
        {
            return "";
        }
    }
}
