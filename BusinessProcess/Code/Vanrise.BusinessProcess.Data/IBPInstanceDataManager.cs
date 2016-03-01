using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {
        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId);
        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input);
        BigResult<BPInstanceDetail> GetFilteredBPInstances(DataRetrievalInput<BPInstanceQuery> input);
    }
}
