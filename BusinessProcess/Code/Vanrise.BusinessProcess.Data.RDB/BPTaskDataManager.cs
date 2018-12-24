using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPTaskDataManager : IBPTaskDataManager
    {
        public void CancelNotCompletedTasks(long processInstanceId)
        {
            throw new NotImplementedException();
        }

        public List<BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public BPTask GetTask(long taskId)
        {
            throw new NotImplementedException();
        }

        public List<BPTask> GetUpdated(ref object lastUpdateHandle, int nbOfRows, int? processInstanceId, int? userId)
        {
            throw new NotImplementedException();
        }

        public bool InsertTask(string title, long processInstanceId, Guid typeId, List<int> assignedUserIds, BPTaskStatus bpTaskStatus, BPTaskData taskData, string assignedUsersDescription, out long taskId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTaskExecution(ExecuteBPTaskInput input, BPTaskStatus bpTaskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
