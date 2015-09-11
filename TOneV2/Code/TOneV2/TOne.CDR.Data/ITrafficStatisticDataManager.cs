using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using Vanrise.Data;

namespace TOne.CDR.Data
{
    public interface ITrafficStatisticDataManager : IDataManager, IBulkApplyDataManager<TrafficStatistic>
    {
        void UpdateTrafficStatisticBatch(DateTime batchStart, DateTime batchEnd, TrafficStatisticsByKey trafficStatisticsByKey);

        void UpdateTrafficStatisticDailyBatch(DateTime batchDate,TrafficStatisticsDailyByKey trafficStatisticsByKey);
        void ApplyTrafficStatsForDB(Object preparedTrafficStats);
        void SaveTrafficStatsForDB(List<TrafficStatistic> trafficStatistics);
    }
}
