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
        void UpdateTrafficStatisticBatch(DateTime batchStart, DateTime batchEnd, Dictionary<string, TrafficStatistic> trafficStatisticsByKey);

        void UpdateTrafficStatisticDailyBatch(DateTime batchStart, DateTime batchEnd, Dictionary<string, TrafficStatisticDaily> trafficStatisticsByKey);
    }
}
