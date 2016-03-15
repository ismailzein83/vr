using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTaskManager
    {
        #region public methods
        public CreateBPTaskOutput CreateTask(CreateBPTaskInput createBPTaskInput)
        {
            if (createBPTaskInput == null)
                throw new ArgumentNullException("createBPTaskInput");
            if (createBPTaskInput.TaskData == null)
                throw new ArgumentNullException("createBPTaskInput.TaskInformation");
            if (createBPTaskInput.AssignedTo == null)
                throw new ArgumentNullException("createBPTaskInput.AssignedTo");

            var assignedUserIds = createBPTaskInput.AssignedTo.GetUserIds(null);
            if (assignedUserIds == null || assignedUserIds.Count() == 0)
                throw new Exception(String.Format("Could not resolve AssignedTo '{0}'", createBPTaskInput.AssignedTo));

            var bpTaskType = GetBPTaskType(createBPTaskInput.TaskName);
            if (bpTaskType == null)
                throw new Exception(String.Format("Could not resolve BPTaskType '{0}'", createBPTaskInput.TaskName));

            IBPDataManager bpDataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            var processInstance = bpDataManager.GetInstance(createBPTaskInput.ProcessInstanceId);
            if (processInstance == null)
                throw new Exception(String.Format("Process Instance '{0}' not exists!", createBPTaskInput.ProcessInstanceId));
            var statusAttribute = BPInstanceStatusAttribute.GetAttribute(processInstance.Status);
            if (statusAttribute != null && statusAttribute.IsClosed)
                throw new Exception(String.Format("Process Instance '{0}' is closed. Status is '{1}'", createBPTaskInput.ProcessInstanceId, processInstance.Status));

            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            long taskId;
            bool taskCreated = taskDataManager.InsertTask(createBPTaskInput.Title, createBPTaskInput.ProcessInstanceId, bpTaskType.BPTaskTypeId, assignedUserIds, BPTaskStatus.New, createBPTaskInput.TaskData, createBPTaskInput.AssignedTo.GetDescription(null), out taskId);
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

            task.TaskExecutionInformation.Notes = executeBPTaskInput.Notes;
            task.TaskExecutionInformation.Decision = executeBPTaskInput.Decision;
            task.TaskExecutionInformation.TakenAction = executeBPTaskInput.TakenAction;

            taskDataManager.UpdateTaskExecution(executeBPTaskInput.TaskId, executeBPTaskInput.ExecutedBy, BPTaskStatus.Completed, task.TaskExecutionInformation);
        }

        public BPTaskUpdateOutput GetMyTasksUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            return GetUpdated(ref maxTimeStamp, nbOfRows, null, userId);
        }

        public List<BPTaskDetail> GetMyTasksBeforeId(BPTaskBeforeIdInput input)
        {
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            return GetBeforeId(input.LessThanID, input.NbOfRows, null, userId);
        }

        public BPTaskUpdateOutput GetProcessTaskUpdated(ref byte[] maxTimeStamp, int nbOfRows, int processInstanceId)
        {
            return GetUpdated(ref maxTimeStamp, nbOfRows, processInstanceId, null);
        }

        public List<BPTaskDetail> GetProcessTaskBeforeId(BPTaskBeforeIdInput input)
        {
            return GetBeforeId(input.LessThanID, input.NbOfRows, input.ProcessInstanceId, null);
        }

        public void ExecuteTask(ExecuteBPTaskInput input)
        {
            BPTask task = GetTask(input.TaskId);
            IBPEventDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
            dataManager.InsertEvent(task.ProcessInstanceId, BPTask.GetTaskWFBookmark(task.BPTaskId), input);
        }

        public BPTask GetTask(long taskId)
        {
            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            return taskDataManager.GetTask(taskId);
        }

        public BPTaskType GetBPTaskType(int taskTypeId)
        {
            var bpTaskTypes = GetCachedBPTaskTypes();

            if (bpTaskTypes == null)
                return null;

            BPTaskType bpTaskType;
            if (bpTaskTypes.TryGetValue(taskTypeId, out bpTaskType))
                return bpTaskType;

            return null;
        }

        public BPTaskType GetBPTaskType(string taskTypeName)
        {
            var bpTaskTypes = GetCachedBPTaskTypes();

            if (bpTaskTypes == null)
                return null;

            foreach (KeyValuePair<int, BPTaskType> item in bpTaskTypes)
            {
                if (item.Value.Name == taskTypeName)
                    return item.Value;
            }
            return null;
        }
        #endregion

        #region mapper
        private BPTaskDetail BPTaskDetailMapper(BPTask bpTask)
        {
            if (bpTask == null)
                return null;
            return new BPTaskDetail()
            {
                Entity = bpTask
            };
        }
        #endregion

        #region private methods
        private List<BPTaskDetail> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            List<BPTask> bpTasks = dataManager.GetBeforeId(lessThanID, nbOfRows, processInstanceId, userId);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask));
            }
            return bpTaskDetails;
        }

        private BPTaskUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int? processInstanceId, int? userId)
        {
            BPTaskUpdateOutput bpTaskUpdateOutput = new BPTaskUpdateOutput();

            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            List<BPTask> bpTasks = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, processInstanceId, userId);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask));
            }

            bpTaskUpdateOutput.ListBPTaskDetails = bpTaskDetails;
            bpTaskUpdateOutput.MaxTimeStamp = maxTimeStamp;
            return bpTaskUpdateOutput;
        }

        private Dictionary<int, BPTaskType> GetCachedBPTaskTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPTaskTypes",
               () =>
               {
                   IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
                   IEnumerable<BPTaskType> data = dataManager.GetBPTaskTypes();
                   return data.ToDictionary(cn => cn.BPTaskTypeId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPTaskTypesUpdated(ref _updateHandle);
            }
        }
        #endregion


    }
}