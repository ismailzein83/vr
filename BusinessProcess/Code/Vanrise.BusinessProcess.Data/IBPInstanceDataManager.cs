using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstanceDataManager : IDataManager
    {

        List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId);

        List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input);
    }
}
