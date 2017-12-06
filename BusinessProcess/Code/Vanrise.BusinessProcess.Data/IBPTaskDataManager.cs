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
        bool InsertTask(string title, long processInstanceId, Guid typeId, List<int> assignedUserIds, BPTaskStatus bpTaskStatus, BPTaskData taskData, string assignedUsersDescription, out long taskId);

        BPTask GetTask(long taskId);

        void UpdateTaskExecution(ExecuteBPTaskInput input, BPTaskStatus bpTaskStatus);

        List<BPTask> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int? processInstanceId, int? userId);

        List<BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId);

        void CancelNotCompletedTasks(long processInstanceId);
    }
}
