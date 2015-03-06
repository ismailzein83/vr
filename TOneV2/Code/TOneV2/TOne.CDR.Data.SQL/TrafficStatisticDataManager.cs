using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using TOne.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class TrafficStatisticDataManager : BaseTOneDataManager, ITrafficStatisticDataManager
    {
        public void UpdateTrafficStatisticBatch(string batchStart, string batchEnd, Dictionary<string, TrafficStatistic> trafficStatisticsByKey)
        {
            //TODO: 
            // 1 - Retrieve Dictionary<String,int> of group key from the TrafficStats table where Attempts >= BatchStart and Attempts < BatchEnd
            // 2 - divide the batch into two lists one to update and one to insert
            // 3 - call the update procedure and prepare the insert batch in two parallel tasks
            // 4 - call bcp on the insert batch
            throw new NotImplementedException();
        }

        public void UpdateTrafficStatisticDailyBatch(string batchStart, string batchEnd, Dictionary<string, TrafficStatisticDaily> trafficStatisticsByKey)
        {
            //Same as UpdateTrafficStatisticBatch for the TrafficStatsDaily
            throw new NotImplementedException();
        }
    }
}
