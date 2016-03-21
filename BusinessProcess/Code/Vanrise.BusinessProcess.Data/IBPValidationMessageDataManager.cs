using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPValidationMessageDataManager : IDataManager
    {
        void Insert(IEnumerable<BPValidationMessage> messages);

        List<BPValidationMessage> GetBeforeId(BPValidationMessageBeforeIdInput input);

        List<BPValidationMessage> GetUpdated(BPValidationMessageUpdateInput input);

        Vanrise.Entities.BigResult<BPValidationMessageDetail> GetFilteredBPValidationMessage(Vanrise.Entities.DataRetrievalInput<BPValidationMessageQuery> input);
    }
}
