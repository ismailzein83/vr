using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface ILockServiceDataManager : IDataManager
    {
        bool TryLock(int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, out TransactionLockingDetails lockingDetails);

        void UpdateServiceURL(string serviceUrl);

        bool UpdateLockingDetails(int currentRuntimeProcessId, TransactionLockingDetails lockingDetails);

        string GetServiceURL();
    }
}
