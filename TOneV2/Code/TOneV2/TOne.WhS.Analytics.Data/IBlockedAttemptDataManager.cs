using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Data
{
    public interface IBlockedAttemptDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<BlockedAttempt> GetBlockedAttemptData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptInput> input);
    }
}
