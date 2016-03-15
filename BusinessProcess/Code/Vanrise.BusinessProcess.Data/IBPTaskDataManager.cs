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
        bool InsertTask(string title, long processInstanceId, int typeId, IEnumerable<int> assignedUserIds, BPTaskStatus bpTaskStatus, BPTaskData taskData, string assignedUsersDescription, out long taskId);

        BPTask GetTask(long taskId);

        void UpdateTaskExecution(long taskId, int executedByUserId, BPTaskStatus bpTaskStatus, BPTaskExecutionInformation bPTaskExecutionInformation);

        List<BPTask> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int? processInstanceId, int? userId);

        List<BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId);

        IEnumerable<BPTaskType> GetBPTaskTypes();

        bool AreBPTaskTypesUpdated(ref object _updateHandle);
    }
}
