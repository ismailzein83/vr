using System;
using System.Collections.Generic;
using TOne.Analytics.Entities;
namespace TOne.Analytics.Data
{
    public interface ITrafficStatisticDataManager : IDataManager
    {
        TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticFilter filter, bool withSummary, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending);

        IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to);
     
    }
}
