using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface ISummaryTransformationDataManager : IDataManager
    {
        bool TryLock(int typeId, DateTime batchStart, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds);

        void UnLock(int typeId, DateTime batchStart);
    }
}
