using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskStateManager
    {
        #region Public Methods

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTaskState> UpdateTaskState(SchedulerTaskState taskStateObject)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();

            bool updateActionSucc = dataManager.UpdateTaskState(taskStateObject);
            UpdateOperationOutput<SchedulerTaskState> updateOperationOutput = new UpdateOperationOutput<SchedulerTaskState>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = taskStateObject;
            }
            return updateOperationOutput;
        }

        public bool TryLockTask(Guid taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            return dataManager.TryLockTask(taskId, currentRuntimeProcessId, runningRuntimeProcessesIds);
        }

        public void UnlockTask(Guid taskId)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            dataManager.UnlockTask(taskId);
        }

        public SchedulerTaskStateUpdateOutput GetUpdated(List<Guid> taskIds)
        {
            SchedulerTaskManager schedulerTaskManager = new SchedulerTaskManager();
            List<SchedulerTaskStateDetail> schedulerTaskStateDetails = new List<SchedulerTaskStateDetail>();

            IEnumerable<SchedulerTaskState> taskStates = GetSchedulerTaskStateByTaskIds(taskIds);

            if (taskStates != null)
            {
                foreach (SchedulerTaskState schedulerTaskState in taskStates)
                {
                    var schedulerTask = schedulerTaskManager.GetTask(schedulerTaskState.TaskId);
                    schedulerTask.ThrowIfNull("schedulerTask", schedulerTaskState.TaskId);
                    schedulerTask.TriggerInfo.ThrowIfNull("schedulerTask.TriggerInfo", schedulerTaskState.TaskId);

                    schedulerTaskStateDetails.Add(SchedulerTaskStateDetailMapper(schedulerTaskState, schedulerTask));
                }
            }

            SchedulerTaskStateUpdateOutput bpInstanceUpdateOutput = new SchedulerTaskStateUpdateOutput();
            bpInstanceUpdateOutput.ListSchedulerTaskStateDetails = schedulerTaskStateDetails;

            return bpInstanceUpdateOutput;
        }

        public SchedulerTaskState GetSchedulerTaskStateByTaskId(Guid taskId)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            return dataManager.GetSchedulerTaskStateByTaskId(taskId);
        }

        public List<SchedulerTaskState> GetSchedulerTaskStateByTaskIds(List<Guid> taskIds)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            return dataManager.GetSchedulerTaskStateByTaskIds(taskIds);
        }

        public List<SchedulerTaskState> GetAllScheduleTaskStates()
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            return dataManager.GetAllScheduleTaskStates();
        }

        public void InsertSchedulerTaskState(Guid taskId)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            dataManager.InsertSchedulerTaskState(taskId);
        }

        public Vanrise.Entities.DeleteOperationOutput<SchedulerTaskState> DeleteTaskState(Guid taskId)
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            dataManager.DeleteTaskState(taskId);

            DeleteOperationOutput<SchedulerTaskState> deleteOperationOutput = new DeleteOperationOutput<SchedulerTaskState>()
            {
                Result = DeleteOperationResult.Succeeded
            };

            return deleteOperationOutput;
        }

        public void RunSchedulerTask(Guid taskId)
        {
            var schedulerTask = new SchedulerTaskManager().GetTask(taskId);
            schedulerTask.ThrowIfNull("schedulerTask", taskId);
            schedulerTask.TriggerInfo.ThrowIfNull("schedulerTask.TriggerInfo", taskId);

            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            dataManager.RunSchedulerTask(taskId, schedulerTask.TriggerInfo.AllowRunIfEnabled);
        }

        #endregion

        #region private methods

        private SchedulerTaskStateDetail SchedulerTaskStateDetailMapper(SchedulerTaskState taskState, SchedulerTask task)
        {
            if (taskState == null)
                return null;

            SchedulerTaskStateDetail schedulerTaskStateDetail = new SchedulerTaskStateDetail()
            {
                Entity = taskState,
                IsEnabled = task.IsEnabled,
                AllowRunIfEnabled = task.TriggerInfo.AllowRunIfEnabled
            };

            return schedulerTaskStateDetail;
        }

        #endregion
    }
}
