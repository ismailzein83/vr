using System;
using TOne.Analytics.Entities;
namespace TOne.Analytics.Data
{
    public interface ITrafficStatisticDataManager : IDataManager
    {
        BigResult<TrafficStatisticGroupSummary> GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending);
    }
}
