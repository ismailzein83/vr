using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

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

        public List<int> GetAssignedUsers(long taskId)
        {
            var task = GetTask(taskId);
            task.ThrowIfNull("task", taskId);
            task.AssignedUsers.ThrowIfNull("task.AssignedUsers", taskId);
            return task.AssignedUsers;
        }

        public BPTaskUpdateOutput GetMyTasksUpdated(object lastUpdateHandle, int nbOfRows, BPTaskFilter bpTaskFilter)
        {
            return GetUpdated(lastUpdateHandle, nbOfRows, null, ContextFactory.GetContext().GetLoggedInUserId(), bpTaskFilter, false);
        }

        public List<BPTaskDetail> GetMyTasksBeforeId(BPTaskBeforeIdInput input)
        {
            return GetBeforeId(input.LessThanID, input.NbOfRows, null, ContextFactory.GetContext().GetLoggedInUserId(), input.BPTaskFilter, false);
        }

        public BPTaskDefaultActionsState GetInitialBPTaskDefaultActionsState(long bpTaskId)
        {
            var bpTask = GetTask(bpTaskId);
            var takenByUserId = bpTask.TakenBy;
            return EvaluateBPTaskDefaultActionsState(bpTask.AssignedUsers, takenByUserId);
        }


        public BPTaskUpdateOutput GetProcessTaskUpdated(object lastUpdateHandle, int nbOfRows, int processInstanceId)
        {
            return GetUpdated(lastUpdateHandle, nbOfRows, processInstanceId, null, null, true);
        }

        public List<BPTaskDetail> GetProcessTaskBeforeId(BPTaskBeforeIdInput input)
        {
            return GetBeforeId(input.LessThanID, input.NbOfRows, input.ProcessInstanceId, null, null, true);
        }

        public ExecuteBPTaskOutput ExecuteTask(ExecuteBPTaskInput input)
        {
            var executeTaskOutput = new ExecuteBPTaskOutput() { Result = ExecuteBPTaskResult.Succeeded };

            var securityContext = Vanrise.Security.Entities.ContextFactory.GetContext();
            var loggedUserId = securityContext.GetLoggedInUserId();
            input.ExecutedBy = loggedUserId;
            BPTask task = GetTask(input.TaskId);

            bool hasAdminManagePermission = HasAdminManagePermission(securityContext, loggedUserId);

            if (!hasAdminManagePermission)
            {
                if (task.TakenBy.HasValue)
                {
                    if (task.TakenBy.Value != loggedUserId)
                    {
                        executeTaskOutput.Result = ExecuteBPTaskResult.Failed;
                        executeTaskOutput.OutputMessage = "You don't have the permission to execute this task.";
                        return executeTaskOutput;
                    }
                }
                else
                {
                    if (task.AssignedUsers == null || !task.AssignedUsers.Contains(loggedUserId))
                    {
                        executeTaskOutput.Result = ExecuteBPTaskResult.Failed;
                        executeTaskOutput.OutputMessage = "You are not allowed to execute this task.";
                        return executeTaskOutput;
                    }
                }
            }

            if (input.TaskData != null)
            {
                IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
                taskDataManager.UpdateTask(input.TaskId, input.TaskData);
            }

            IBPEventDataManager eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
            eventDataManager.InsertEvent(task.ProcessInstanceId, BPTask.GetTaskWFBookmark(task.BPTaskId), input);
            return executeTaskOutput;
        }

        public BPTask GetTask(long taskId)
        {
            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            return taskDataManager.GetTask(taskId);
        }

        public BPTaskDetail GetTaskDetail(long taskId)
        {
            int? loggedInUser = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            if (!loggedInUser.HasValue)
                return null;

            BPTask task = GetTask(taskId);
            return BPTaskDetailMapper(task, loggedInUser.Value, false);
        }

        public BPTaskDefaultActionsState TakeTask(long taskId)
        {
            var securityContext = ContextFactory.GetContext();
            var loggedUserId = securityContext.GetLoggedInUserId();
            return AssignTask_Private(taskId, loggedUserId);
        }

        public BPTaskDefaultActionsState AssignTask(long taskId, int userId)
        {
            return AssignTask_Private(taskId, userId);
        }

        public BPTaskDefaultActionsState ReleaseTask(long taskId)
        {
            BPTask task = GetTask(taskId);

            if (!task.TakenBy.HasValue)
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);

            var securityContext = ContextFactory.GetContext();
            var loggedUserId = securityContext.GetLoggedInUserId();
            bool hasAdminManagePermission = HasAdminManagePermission(securityContext, loggedUserId);

            if (!hasAdminManagePermission && task.TakenBy.Value != loggedUserId)
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);

            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            if (taskDataManager.ReleaseTask(taskId))
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, null);

            task = GetTask(taskId);//to get the latest value of task
            return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);
        }
        #endregion

        #region mapper
        private BPTaskDetail BPTaskDetailMapper(BPTask bpTask, int loggedInUser, bool displayAssignedTasks)
        {
            if (bpTask == null)
                return null;

            BPTaskTypeManager bpTaskTypeManager = new BPTaskTypeManager();
            UserManager userManager = new UserManager();

            bpTask.ExecutedByIdDescription = bpTask.ExecutedById.HasValue ? userManager.GetUserName(bpTask.ExecutedById.Value) : null;
            bpTask.TakenByDescription = bpTask.TakenBy.HasValue ? userManager.GetUserName(bpTask.TakenBy.Value) : null;

            var taskType = bpTaskTypeManager.GetBPTaskTypeByTaskId(bpTask.BPTaskId);

            return new BPTaskDetail()
            {
                Entity = bpTask,
                AutoOpenTask = taskType.Settings.AutoOpenTask,
                IsAssignedToCurrentUser = bpTask.AssignedUsers != null && bpTask.AssignedUsers.Contains(loggedInUser),
                ShowOnGrid = displayAssignedTasks || !bpTask.TakenBy.HasValue || bpTask.TakenBy.Value == loggedInUser,
                TaskTypeName = taskType.Name
            };
        }
        #endregion

        #region private methods
        private BPTaskDefaultActionsState AssignTask_Private(long taskId, int userId)
        {
            BPTask task = GetTask(taskId);

            if (task.TakenBy.HasValue)
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);

            var securityContext = ContextFactory.GetContext();
            bool hasAdminManagePermission = HasAdminManagePermission(securityContext, userId);

            if (!hasAdminManagePermission && (task.AssignedUsers == null || !task.AssignedUsers.Contains(userId)))
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);

            IBPTaskDataManager taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            if (taskDataManager.AssignTask(taskId, userId))
                return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, userId);

            task = GetTask(taskId);//to get the latest value of task
            return EvaluateBPTaskDefaultActionsState(task.AssignedUsers, task.TakenBy);
        }

        private BPTaskDefaultActionsState EvaluateBPTaskDefaultActionsState(List<int> assignedUsers, int? takenByUserId)
        {
            var securityContext = ContextFactory.GetContext();
            var loggedUserId = securityContext.GetLoggedInUserId();

            bool hasAdminManagePermission = HasAdminManagePermission(securityContext, loggedUserId);

            var visibility = new BPTaskDefaultActionsState();

            if (takenByUserId.HasValue)
            {
                if (hasAdminManagePermission || loggedUserId == takenByUserId.Value)
                {
                    visibility.ShowRelease = true;
                }
            }
            else
            {
                if (hasAdminManagePermission || (assignedUsers != null && assignedUsers.Contains(loggedUserId)))
                {
                    visibility.ShowTake = true;
                    visibility.ShowAssign = true;
                }
            }

            return visibility;
        }

        private List<BPTaskDetail> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId, BPTaskFilter bPTaskFilter, bool displayAssignedTasks)
        {
            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
            var securityContext = Vanrise.Security.Entities.ContextFactory.GetContext();
            int loggedUserId = securityContext.GetLoggedInUserId();

            List<BPTask> bpTasks = dataManager.GetBeforeId(lessThanID, nbOfRows, processInstanceId, userId, BPTaskStatusAttribute.GetClosedStatuses(), bPTaskFilter);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask, loggedUserId, displayAssignedTasks));
            }
            return bpTaskDetails;
        }

        private BPTaskUpdateOutput GetUpdated(object lastUpdateHandle, int nbOfRows, int? processInstanceId, int? userId, BPTaskFilter bPTaskFilter, bool displayAssignedTasks)
        {
            BPTaskUpdateOutput bpTaskUpdateOutput = new BPTaskUpdateOutput();
            var securityContext = Vanrise.Security.Entities.ContextFactory.GetContext();
            int loggedUserId = securityContext.GetLoggedInUserId();

            IBPTaskDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();

            List<BPTask> bpTasks = dataManager.GetUpdated(ref lastUpdateHandle, nbOfRows, processInstanceId, userId, BPTaskStatusAttribute.GetClosedStatuses(), bPTaskFilter);
            List<BPTaskDetail> bpTaskDetails = new List<BPTaskDetail>();
            foreach (BPTask bpTask in bpTasks)
            {
                bpTaskDetails.Add(BPTaskDetailMapper(bpTask, loggedUserId, displayAssignedTasks));
            }

            bpTaskUpdateOutput.ListBPTaskDetails = bpTaskDetails;
            bpTaskUpdateOutput.LastUpdateHandle = lastUpdateHandle;
            return bpTaskUpdateOutput;
        }
        private bool HasAdminManagePermission(ISecurityContext securityContext, int loggedUserId)
        {
            var requiredPermissionEntry = new RequiredPermissionEntry() { PermissionOptions = new List<string>() { "Manage" }, EntityId = new Guid("A99A836D-0C03-4946-A0E2-5A758354807B") };
            return securityContext.IsAllowed(new RequiredPermissionSettings() { Entries = new List<RequiredPermissionEntry>() { requiredPermissionEntry } }, loggedUserId);
        }
        #endregion
    }
}