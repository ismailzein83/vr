using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskManager
    {
        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Runtime.Entities.SchedulerTaskDetail> GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            var allScheduledTasks = GetCachedSchedulerTasks();

            Func<SchedulerTask, bool> filterExpression = (itm) =>
                (input.Query == null || ((string.IsNullOrEmpty(input.Query.NameFilter) || itm.Name.ToLower().Contains(input.Query.NameFilter.ToLower()))
                && (input.Query.Filters == null || input.Query.Filters.Where(item => item.IsMatched(itm)).ToList().Count == input.Query.Filters.Count)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allScheduledTasks.ToBigResult(input, filterExpression, SchedulerTaskDetailMapper));
        }

        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Runtime.Entities.SchedulerTaskDetail> GetFilteredMyTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            if (input.Query == null)
                input.Query = new SchedulerTaskQuery();

            if (input.Query.Filters == null)
                input.Query.Filters = new List<ISchedulerTaskFilter>();

            input.Query.Filters.Add(new UserTaskFilter() { UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId() });
            return GetFilteredTasks(input);
        }

        public List<SchedulerTask> GetMySchedulesInfo()
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            int ownerId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            Func<SchedulerTask, bool> filterExpression = (itm) =>
                 itm.OwnerId == ownerId;

            IEnumerable<SchedulerTask> tasks = allScheduledTasks.FindAllRecords(filterExpression);
            if (tasks == null)
                return null;
            return tasks.ToList();
        }


        public List<SchedulerTask> GetAllScheduleTasks()
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            Func<SchedulerTask, bool> filterExpression = (itm) => (true);

            IEnumerable<SchedulerTask> tasks = allScheduledTasks.FindAllRecords(filterExpression);
            if (tasks == null)
                return null;
            return tasks.ToList();
        }

        public Vanrise.Runtime.Entities.SchedulerTask GetTask(int taskId)
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            SchedulerTask task;
            allScheduledTasks.TryGetValue(taskId, out task);
            return task;
        }
        public List<SchedulerTask> GetTasksbyActionType(int actionType)
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            Func<SchedulerTask, bool> filterExpression = (itm) =>
                 itm.ActionTypeId == actionType;

            IEnumerable<SchedulerTask> tasks = allScheduledTasks.FindAllRecords(filterExpression);
            if (tasks == null)
                return null;
            return tasks.ToList();
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
                    if (Vanrise.Security.Entities.ContextFactory.GetContext().IsAllowed(actionType.Info.RequiredPermissions))
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
            taskObject.OwnerId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool insertActionSucc = dataManager.AddTask(taskObject, out taskId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        #region private methods
        private Dictionary<int, SchedulerTask> GetCachedSchedulerTasks()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSchedulerTasks",
               () =>
               {
                   ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
                   IEnumerable<SchedulerTask> data = dataManager.GetSchedulerTasks();
                   return data.ToDictionary(cn => cn.TaskId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreSchedulerTasksUpdated(ref _updateHandle);
            }
        }

        private SchedulerTaskDetail SchedulerTaskDetailMapper(SchedulerTask task)
        {
            if (task == null)
                return null;
            return new SchedulerTaskDetail() { Entity = task };
        }
        #endregion
    }
}
