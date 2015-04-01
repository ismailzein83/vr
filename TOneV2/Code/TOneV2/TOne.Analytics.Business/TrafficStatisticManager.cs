using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class TrafficStatisticManager
    {
        public BigResult<TrafficStatisticGroupSummary> GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending)
        {
            ITrafficStatisticDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            return dataManager.GetTrafficStatisticSummary(tempTableKey, groupKeys, from, to, fromRow, toRow, orderBy, isDescending);
        }
    }
}
