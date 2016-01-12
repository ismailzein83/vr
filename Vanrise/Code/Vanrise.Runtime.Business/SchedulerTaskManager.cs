﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskManager
    {
        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Runtime.Entities.SchedulerTask> GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            int ownerId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredTasks(input, ownerId));
        }

        public List<SchedulerTask> GetSchedulesInfo()
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            int ownerId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            return datamanager.GetAllTasks(ownerId);
        }

        public Vanrise.Runtime.Entities.SchedulerTask GetTask(int taskId)
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            return datamanager.GetTask(taskId);

        }
        public List<SchedulerTask> GetTasksbyActionType(int actionType)
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            return datamanager.GetTasksbyActionType(actionType);
        }

        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            ISchedulerTaskTriggerTypeDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskTriggerTypeDataManager>();
            return datamanager.GetAll();
        }

        public List<SchedulerTaskActionType> GetSchedulerTaskActionTypes()
        {
            ISchedulerTaskActionTypeDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskActionTypeDataManager>();
            List<SchedulerTaskActionType> lstSchedulerTaskActionTypes = datamanager.GetAll();
            List<SchedulerTaskActionType> lsSchedulerTaskActionTypesOutput = new List<SchedulerTaskActionType>();
            foreach (SchedulerTaskActionType actionType in lstSchedulerTaskActionTypes)
            {
                if (actionType.Info.RequiredPermissions != null)
                {
                    if (SecurityContext.Current.IsAllowed(actionType.Info.RequiredPermissions))
                        lsSchedulerTaskActionTypesOutput.Add(actionType);
                }
                else
                    lsSchedulerTaskActionTypesOutput.Add(actionType);
                
            }
            return lsSchedulerTaskActionTypesOutput;
        }

        public Vanrise.Entities.InsertOperationOutput<SchedulerTask> AddTask(SchedulerTask taskObject)
        {
            InsertOperationOutput<SchedulerTask> insertOperationOutput = new InsertOperationOutput<SchedulerTask>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int taskId = -1;
            taskObject.OwnerId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool insertActionSucc = dataManager.AddTask(taskObject, out taskId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                taskObject.TaskId = taskId;
                insertOperationOutput.InsertedObject = taskObject;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTask> UpdateTask(SchedulerTask taskObject)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();

            bool updateActionSucc = dataManager.UpdateTaskInfo(taskObject.TaskId, taskObject.Name, taskObject.IsEnabled, taskObject.TriggerTypeId, taskObject.ActionTypeId, 
                taskObject.TaskSettings);
            UpdateOperationOutput<SchedulerTask> updateOperationOutput = new UpdateOperationOutput<SchedulerTask>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = taskObject;
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteTask(int taskId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool deleted = dataManager.DeleteTask(taskId);

            if (deleted)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
    }
}
