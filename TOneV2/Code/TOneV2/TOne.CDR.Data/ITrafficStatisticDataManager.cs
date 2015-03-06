using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;

namespace TOne.CDR.Data
{
    public interface ITrafficStatisticDataManager : IDataManager
    {
        void UpdateTrafficStatisticBatch(string batchStart, string batchEnd, Dictionary<string, TrafficStatistic> trafficStatisticsByKey);

        void UpdateTrafficStatisticDailyBatch(string batchStart, string batchEnd, Dictionary<string, TrafficStatisticDaily> trafficStatisticsByKey);
    }
}
