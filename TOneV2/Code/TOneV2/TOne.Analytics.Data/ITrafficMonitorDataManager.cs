using System;
using System.Collections.Generic;
using TOne.Analytics.Entities;
namespace TOne.Analytics.Data
{
    public interface ITrafficMonitorDataManager : IDataManager
    {
        GenericSummaryBigResult<TrafficStatistic> GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input);
        
        IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to);
     
    }
}
