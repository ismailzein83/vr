using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IStatisticBatchDataManager : IDataManager
    {
        bool TryLock(int typeId, DateTime batchStart, int currentRuntimeProcessId, IEnumerable<int> runningProcessIds, out StatisticBatchInfo batchInfo, out bool isInfoCorrupted);
    }
}
