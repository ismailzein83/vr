using System.Collections.Generic;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Data
{
    public interface IBlockedAttemptDataManager : IDataManager
    {
        IEnumerable<BlockedAttempt> GetAllFilteredBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input);
    }
}
