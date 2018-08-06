using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Common.Data;
namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskManager
    {
        #region Ctor/Fields

        static SchedulerTaskActionTypeManager _schedulerTaskActionTypeManager = new SchedulerTaskActionTypeManager();
        IVRActionLogger _vrActionLogger = BusinessManagerFactory.GetManager<IVRActionLogger>();
        #endregion
        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Runtime.Entities.SchedulerTaskDetail> GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            var allScheduledTasks = GetCachedSchedulerTasks();

            Func<SchedulerTask, bool> filterExpression = (itm) =>
                {
                    if (!_schedulerTaskActionTypeManager.DoesUserHaveViewSpecificTaskAccess(itm))
                        return false;


                    if (itm.ActionTypeId == Guid.Parse("B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68"))//1 is for Data Source Tasks
                        return false;

                    if (input.Query == null)
                        return true;

                    if (!string.IsNullOrEmpty(input.Query.NameFilter) && !itm.Name.ToLower().Contains(input.Query.NameFilter.ToLower()))
                        return false;

                    if (input.Query.Filters != null && input.Query.Filters.Any(item => !item.IsMatched(itm)))
                        return false;

                    return true;
                };
            //(itm.ActionTypeId != 1) && (input.Query == null || ((string.IsNullOrEmpty(input.Query.NameFilter) || itm.Name.ToLower().Contains(input.Query.NameFilter.ToLower()))
            //&& (input.Query.Filters == null || input.Query.Filters.FirstOrDefault(item => !item.IsMatched(itm)) == null)));
            _vrActionLogger.LogGetFilteredAction(SchedulerTaskLoggableEntity.Instance, input);
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

        public List<SchedulerTaskInfo> GetMyTasksInfo()
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            int ownerId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            Func<SchedulerTask, bool> filterExpression = (itm) =>
                 itm.OwnerId == ownerId;

            IEnumerable<SchedulerTask> tasks = allScheduledTasks.FindAllRecords(filterExpression);
            if (tasks == null)
                return null;

            List<SchedulerTaskInfo> listTasksInfos = new List<SchedulerTaskInfo>();
            foreach (SchedulerTask schedulerTask in tasks)
            {
                listTasksInfos.Add(
                 new SchedulerTaskInfo()
                {
                    TaskId = schedulerTask.TaskId,
                    Name = schedulerTask.Name
                });
            }
            return listTasksInfos;
        }
        public string GetSchedulerTaskName(SchedulerTask schedulerTask)
        {
            if (schedulerTask == null)
                return null;
            return schedulerTask.Name;
        }
        public List<SchedulerTaskInfo> GetTasksInfo()
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            Func<SchedulerTask, bool> filterExpression = (itm) => (true);

            IEnumerable<SchedulerTask> tasks = allScheduledTasks.FindAllRecords(filterExpression);
            if (tasks == null)
                return null;

            List<SchedulerTaskInfo> listTasksInfos = new List<SchedulerTaskInfo>();
            foreach (SchedulerTask schedulerTask in tasks)
            {
                listTasksInfos.Add(
                 new SchedulerTaskInfo()
                 {
                     TaskId = schedulerTask.TaskId,
                     Name = schedulerTask.Name
                 });
            }
            return listTasksInfos;
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

        public Vanrise.Runtime.Entities.SchedulerTask GetTask(Guid taskId, bool isViewedFromUI)
        {
            var allScheduledTasks = GetCachedSchedulerTasks();
            SchedulerTask task;
            allScheduledTasks.TryGetValue(taskId, out task);
            if (task != null && isViewedFromUI)
                _vrActionLogger.LogObjectViewed(SchedulerTaskLoggableEntity.Instance, task);
            return task;
        }
        public Vanrise.Runtime.Entities.SchedulerTask GetTask(Guid taskId)
        {
            return GetTask(taskId, false);
        }
        public List<SchedulerTask> GetTasksbyActionType(Guid actionType)
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

        public Vanrise.Entities.InsertOperationOutput<SchedulerTaskDetail> AddTask(SchedulerTask taskObject)
        {
            InsertOperationOutput<SchedulerTaskDetail> insertOperationOutput = new InsertOperationOutput<SchedulerTaskDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            taskObject.TaskId = Guid.NewGuid();
            taskObject.OwnerId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool insertActionSucc = dataManager.AddTask(taskObject);

            if (insertActionSucc)
            {
                if (taskObject.TaskSettings != null && taskObject.TaskSettings.TaskActionArgument != null)
                    taskObject.TaskSettings.TaskActionArgument.OnAfterSaveAction(new BaseTaskActionArgumentOnAfterSaveActionContext { TaskId = taskObject.TaskId });

                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                _vrActionLogger.TrackAndLogObjectAdded(SchedulerTaskLoggableEntity.Instance, taskObject);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SchedulerTaskDetailMapper(taskObject);
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTaskDetail> UpdateTask(SchedulerTask taskObject)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();

            bool updateActionSucc = dataManager.UpdateTaskInfo(taskObject.TaskId, taskObject.Name, taskObject.IsEnabled, taskObject.TriggerTypeId, taskObject.ActionTypeId,
                taskObject.TaskSettings);
            UpdateOperationOutput<SchedulerTaskDetail> updateOperationOutput = new UpdateOperationOutput<SchedulerTaskDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                if (taskObject.TaskSettings != null && taskObject.TaskSettings.TaskActionArgument != null)
                    taskObject.TaskSettings.TaskActionArgument.OnAfterSaveAction(new BaseTaskActionArgumentOnAfterSaveActionContext { TaskId = taskObject.TaskId });

                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                _vrActionLogger.TrackAndLogObjectUpdated(SchedulerTaskLoggableEntity.Instance, taskObject);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SchedulerTaskDetailMapper(taskObject);
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteTask(Guid taskId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool deleted = dataManager.DeleteTask(taskId);

            if (deleted)
            {
                SchedulerTaskStateManager schedulerTaskStateManager = new SchedulerTaskStateManager();
                schedulerTaskStateManager.DeleteTaskState(taskId);

                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTaskDetail> DisableTask(Guid taskId)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool updateActionSucc = dataManager.DisableTask(taskId);
            UpdateOperationOutput<SchedulerTaskDetail> updateOperationOutput = new UpdateOperationOutput<SchedulerTaskDetail>()
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            if (updateActionSucc)
            {
                var taskObject = GetTask(taskId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SchedulerTaskDetailMapper(taskObject);
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTaskDetail> EnableTask(Guid taskId)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool updateActionSucc = dataManager.EnableTask(taskId);
            UpdateOperationOutput<SchedulerTaskDetail> updateOperationOutput = new UpdateOperationOutput<SchedulerTaskDetail>()
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            if (updateActionSucc)
            {
                var taskObject = GetTask(taskId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SchedulerTaskDetailMapper(taskObject);
            }


            return updateOperationOutput;
        }

        public bool DoesUserHaveConfigureSpecificTaskAccess(Guid taskId)
        {
            var schedulerTask = GetTask(taskId);
            return _schedulerTaskActionTypeManager.DoesUserHaveConfigureSpecificTaskAccess(schedulerTask);
        }

        public bool DoesUserHaveConfigureAllTaskAccess()
        {
            var allScheduledTasks = GetAllTasksExceptDS();
            foreach (var task in allScheduledTasks)
                if (!DoesUserHaveConfigureSpecificTaskAccess(task.TaskId))
                    return false;
            return true;
        }

        public bool DisableAllTasks()
        {
            var tasks = GetAllTasksExceptDS();
            foreach (var task in tasks)
            {
                if (!task.IsEnabled)
                    continue;
                else
                {
                    var output = DisableTask(task.TaskId);
                    if (output.Result != UpdateOperationResult.Succeeded)
                        return false;
                }

            }
            return true;
        }

        public SchedulerTaskManagmentInfo GetTaskManagmentInfo()
        {
            var tasks = GetAllTasksExceptDS();
            return new SchedulerTaskManagmentInfo()
            {
                ShowEnableAll = tasks.FindRecord(x => !x.IsEnabled) != null,
                ShowDisableAll = tasks.FindRecord(x => x.IsEnabled) != null
            };
        }
        public bool EnableAllTasks()
        {
            var tasks = GetAllTasksExceptDS();
            foreach (var task in tasks)
            {
                if (task.IsEnabled)
                    continue;
                else
                {
                    var output = EnableTask(task.TaskId);
                    if (output.Result != UpdateOperationResult.Succeeded)
                        return false;
                }
            }
            return true;
        }
        private class SchedulerTaskLoggableEntity : VRLoggableEntityBase
        {
            public static SchedulerTaskLoggableEntity Instance = new SchedulerTaskLoggableEntity();

            private SchedulerTaskLoggableEntity()
            {

            }

            static SchedulerTaskManager s_schedulerTaskManager = new SchedulerTaskManager();

            public override string EntityUniqueName
            {
                get { return "VR_Runtime_SchedulerTask"; }
            }

            public override string ModuleName
            {
                get { return "Runtime"; }
            }

            public override string EntityDisplayName
            {
                get { return "Scheduler Task"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Runtime_SchedulerTask_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SchedulerTask schedulerTask = context.Object.CastWithValidate<SchedulerTask>("context.Object");
                return schedulerTask.TaskId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SchedulerTask schedulerTask = context.Object.CastWithValidate<SchedulerTask>("context.Object");
                return s_schedulerTaskManager.GetSchedulerTaskName(schedulerTask);
            }
        }
        #region private methods
        private Dictionary<Guid, SchedulerTask> GetCachedSchedulerTasks()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSchedulerTasks",
               () =>
               {
                   ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
                   IEnumerable<SchedulerTask> data = dataManager.GetSchedulerTasks();
                   return data.ToDictionary(cn => cn.TaskId, cn => cn);
               });
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
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
            return new SchedulerTaskDetail()
            {
                Entity = task,
                AllowEdit = _schedulerTaskActionTypeManager.DoesUserHaveConfigureSpecificTaskAccess(task),
                AllowRun = _schedulerTaskActionTypeManager.DoesUserHaveRunSpecificTaskAccess(task)
            };
        }


        private List<SchedulerTask> GetAllTasksExceptDS()
        {
            return GetCachedSchedulerTasks().Values.FindAllRecords(x => x.ActionTypeId != Guid.Parse("B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68")).ToList();
        }
        #endregion

    }
}
