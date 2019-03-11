using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTaskAssigneeContext : IBPTaskAssigneeContext
    {
        public int ProcessInitiaterUserId { get; set; }
    }

    public class BPTaskManager
    {
        #region public methods
        public CreateBPTaskOutput CreateTask(CreateBPTaskInput createBPTaskInput)
        {
            if (createBPTaskInput == null)
                throw new ArgumentNullException("createBPTaskInput");
            if (createBPTaskInput.TaskData == null)
                throw new ArgumentNullException("createBPTaskInput.TaskData");

            BPTaskTypeManager bpTaskTypeManager = new BPTaskTypeManager();
            BPTaskType bpTaskType;
            if (createBPTaskInput.TaskData.TaskTypeId.HasValue)
                bpTaskType = bpTaskTypeManager.GetBPTaskType(createBPTaskInput.TaskData.TaskTypeId.Value);
            else
                bpTaskType = bpTaskTypeManager.GetBPTaskType(createBPTaskInput.TaskName);

            if (bpTaskType == null)
                throw new Exception(String.Format("Could not resolve BPTaskType '{0}'", createBPTaskInput.TaskName));

            //IBPDataManager bpDataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            //var processInstance = bpDataManager.GetInstance(createBPTaskInput.ProcessInstanceId);
            //if (processInstance == null)
            //    throw new Exception(String.Format("Process Instance '{0}' not exists!", createBPTaskInput.ProcessInstanceId));
            //var statusAttribute = BPInstanceStatusAttribute.GetAttribute(processInstance.Status);
            //if (statusAttribute != null && statusAttribute.IsClosed)
            //    throw new Exception(String.Format("Process Instance '{0}' is closed. Status is '{1}'", createBPTaskInput.ProcessInstanceId, processInstance.Status));

            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            long taskId;
            bool taskCreated = taskDataManager.InsertTask(createBPTaskInput.Title, createBPTaskInput.ProcessInstanceId, bpTaskType.BPTaskTypeId, createBPTaskInput.AssignedUserIds, BPTaskStatus.New, createBPTaskInput.TaskData, createBPTaskInput.AssignedUserIdsDescription, out taskId);
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
            taskDataManager.UpdateTaskExecution(executeBPTaskInput, BPTaskStatus.Completed);

            task = taskDataManager.GetTask(executeBPTaskInput.TaskId);
        }

        public BPTaskUpdateOutput GetMyTasksUpdated(object lastUpdateHandle, int nbOfRows)
        {
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            return GetUpdated(lastUpdateHandle, nbOfRows, null, userId);
        }

        public List<BPTaskDetail> GetMyTasksBeforeId(BPTaskBeforeIdInput input)
        {
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            return GetBeforeId(input.LessThanID, input.NbOfRows, null, userId);
        }

        public BPTaskUpdateOutput GetProcessTaskUpdated(object lastUpdateHandle, int nbOfRows, int processInstanceId)
        {
            return GetUpdated(lastUpdateHandle, nbOfRows, processInstanceId, null);
        }

        public List<BPTaskDetail> GetProcessTaskBeforeId(BPTaskBeforeIdInput input)
        {
            return GetBeforeId(input.LessThanID, input.NbOfRows, input.ProcessInstanceId, null);
        }

        public void ExecuteTask(ExecuteBPTaskInput input)
        {
            input.ExecutedBy = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            BPTask task = GetTask(input.TaskId);
            IBPEventDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
            dataManager.InsertEvent(task.ProcessInstanceId, BPTask.GetTaskWFBookmark(task.BPTaskId), input);
        }

        public BPTask GetTask(long taskId)
        {
            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            return taskDataManager.GetTask(taskId);
        }
        #endregion

        #region mapper
        private BPTaskDetail BPTaskDetailMapper(BPTask bpTask, int loggedInUser)
        {
            if (bpTask == null)
                return null;
            BPTaskTypeManager bpTaskTypeManager = new BPTaskTypeManager();

            return new BPTaskDetail()
            {
                Entity = bpTask,
                AutoOpenTask = bpTaskTypeManager.GetBPTaskTypeByTaskId(bpTask.BPTaskId).Settings.AutoOpenTask,
                IsAssignedToCurrentUser = bpTask.AssignedUsers != null && bpTask.AssignedUsers.Contains(loggedInUser)
            };
        }
        #endregion

        #region private methods
        private List<BPTaskDetail> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            int loggedInUser = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();

            List<BPTask> bpTasks = dataManager.GetBeforeId(lessThanID, nbOfRows, processInstanceId, userId);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask, loggedInUser));
            }
            return bpTaskDetails;
        }

        private BPTaskUpdateOutput GetUpdated(object lastUpdateHandle, int nbOfRows, int? processInstanceId, int? userId)
        {
            BPTaskUpdateOutput bpTaskUpdateOutput = new BPTaskUpdateOutput();
            int loggedInUser = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();

            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            List<BPTask> bpTasks = dataManager.GetUpdated(ref lastUpdateHandle, nbOfRows, processInstanceId, userId);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask, loggedInUser));
            }

            bpTaskUpdateOutput.ListBPTaskDetails = bpTaskDetails;
            bpTaskUpdateOutput.LastUpdateHandle = lastUpdateHandle;
            return bpTaskUpdateOutput;
        }

        #endregion
    }
}