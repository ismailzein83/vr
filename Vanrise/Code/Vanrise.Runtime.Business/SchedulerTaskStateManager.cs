using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskStateManager
    {

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
        public List<SchedulerTaskState> GetDueTasks()
        {
            ISchedulerTaskStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskStateDataManager>();
            return dataManager.GetDueTasks();
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
            SchedulerTaskStateUpdateOutput bpInstanceUpdateOutput = new SchedulerTaskStateUpdateOutput();
            IEnumerable<SchedulerTaskState> taskStates = GetSchedulerTaskStateByTaskIds(taskIds);

            List<SchedulerTaskStateDetail> schedulerTaskStateDetails = new List<SchedulerTaskStateDetail>();

            if (taskStates != null)
            {
                foreach (SchedulerTaskState schedulerTaskState in taskStates)
                {
                    schedulerTaskStateDetails.Add(SchedulerTaskStateDetailMapper(schedulerTaskState));
                }
            }

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

            bool deleteActionSucc = dataManager.DeleteTaskState(taskId);
            DeleteOperationOutput<SchedulerTaskState> deleteOperationOutput = new DeleteOperationOutput<SchedulerTaskState>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        } 
        #region private methods
        
        private SchedulerTaskStateDetail SchedulerTaskStateDetailMapper(SchedulerTaskState task)
        {
            if (task == null)
                return null;
            return new SchedulerTaskStateDetail() { Entity = task };
        }
        #endregion
    }
}
