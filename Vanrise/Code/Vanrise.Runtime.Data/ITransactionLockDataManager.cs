using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface ITransactionLockDataManager : IDataManager
    {
        void Add(TransactionLockItem lockItem);

        void Delete(Guid lockItemId);

        List<TransactionLockItem> GetLocksForNotRunningProcesses(List<int> runningProcessesIds);

        List<TransactionLockItem> GetAll();
    }
}
