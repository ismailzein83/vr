using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPValidationMessageDataManager : IDataManager
    {
        void Insert(IEnumerable<BPValidationMessage> messages);

        List<BPValidationMessage> GetBeforeId(BPValidationMessageBeforeIdInput input);

        List<BPValidationMessage> GetUpdated(BPValidationMessageUpdateInput input);

        IEnumerable<BPValidationMessage> GetFilteredBPValidationMessage(BPValidationMessageQuery query);
    }
}
