using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPTaskManager
    {
        public CreateBPTaskOutput CreateTask(CreateBPTaskInput createBPTaskInput)
        {
            if (createBPTaskInput == null)
                throw new ArgumentNullException("createBPTaskInput");
            if (createBPTaskInput.TaskInformation == null)
                throw new ArgumentNullException("createBPTaskInput.TaskInformation");
            if (createBPTaskInput.TaskInformation.AssignedTo == null)
                throw new ArgumentNullException("createBPTaskInput.TaskInformation.AssignedTo");

            var assignedUserIds = createBPTaskInput.TaskInformation.AssignedTo.GetUserIds(null);
            if (assignedUserIds == null || assignedUserIds.Count() == 0)
                throw new Exception(String.Format("Could not resolve AssignedTo '{0}'", createBPTaskInput.TaskInformation.AssignedTo));

            IBPDataManager bpDataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            var processInstance = bpDataManager.GetInstance(createBPTaskInput.ProcessInstanceId);
            if (processInstance == null)
                throw new Exception(String.Format("Process Instance '{0}' not exists!", createBPTaskInput.ProcessInstanceId));
            var statusAttribute = BPInstanceStatusAttribute.GetAttribute(processInstance.Status);
            if (statusAttribute != null && statusAttribute.IsClosed)
                throw new Exception(String.Format("Process Instance '{0}' is closed. Status is '{1}'", createBPTaskInput.ProcessInstanceId, processInstance.Status));

            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            long taskId;
            bool taskCreated = taskDataManager.InsertTask(createBPTaskInput.Title, createBPTaskInput.ProcessInstanceId, createBPTaskInput.TypeId, assignedUserIds, BPTaskStatus.New, createBPTaskInput.TaskInformation, out taskId);
            var output = new CreateBPTaskOutput
            {
                Result = taskCreated ? CreateBPTaskResult.Succeeded : CreateBPTaskResult.Failed,
                TaskId = taskId,
                WFBookmarkName = BPTask.GetTaskWFBookmark(taskId)
            };
            return output;
        }

        public void SetTaskCompleted(ExecuteBPTaskInput executeBPTaskInput, out BPTask task)
        {
            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            task = taskDataManager.GetTask(executeBPTaskInput.TaskId);
            if (task.TaskExecutionInformation == null)
                task.TaskExecutionInformation = new BPTaskExecutionInformation();
            if (task.TaskExecutionInformation.Comments == null)
                task.TaskExecutionInformation.Comments = new List<BPTaskComment>();
            task.TaskExecutionInformation.TakenAction = executeBPTaskInput.TakenAction;
            if (executeBPTaskInput.Comment != null)
                task.TaskExecutionInformation.Comments.Add(executeBPTaskInput.Comment);
            taskDataManager.UpdateTaskExecution(executeBPTaskInput.TaskId, executeBPTaskInput.ExecutedBy, BPTaskStatus.Completed, task.TaskExecutionInformation);
        }
    }
}
