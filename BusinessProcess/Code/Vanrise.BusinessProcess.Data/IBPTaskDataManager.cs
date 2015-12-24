using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTaskDataManager : IDataManager
    {
        bool InsertTask(string title, long processInstanceId, int typeId, IEnumerable<int> assignedUserIds, BPTaskStatus status, BPTaskInformation taskInformation, out long taskId);

        BPTask GetTask(long p);

        void UpdateTaskExecution(long taskId, int executedByUserId, BPTaskStatus bPTaskStatus, BPTaskExecutionInformation bPTaskExecutionInformation);
    }
}
