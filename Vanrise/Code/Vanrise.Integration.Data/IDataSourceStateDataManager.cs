using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceStateDataManager : IDataManager
    {
        void UpdateStateAndUnlock(int dataSourceId, BaseAdapterState updatedState);

        bool TryLockAndGet(int dataSourceId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, out BaseAdapterState state);
    }
}
