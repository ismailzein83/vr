using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IReleaseCodeDataManager : IDataManager
    {
        GenericSummaryBigResult<ReleaseCodeStatistic> GetReleaseCodeStatistic(
            Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input);
    }
}
