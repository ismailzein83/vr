using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskActionTypeManager
    {

        public IEnumerable<SchedulerTaskActionType> GetSchedulerTaskActionTypes(SchedulerTaskActionTypeFilter filter)
        {
            Func<SchedulerTaskActionType, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (schedulerTaskActionType) =>
                {
                    if (schedulerTaskActionType.Info.Editor == null)
                        return false;

                    if (filter.Filters != null && !CheckIfFilterIsMatch(schedulerTaskActionType, filter.Filters))
                        return false;

                    return true;
                };
            }
            return GetCachedSchedulerTaskActionTypes().FindAllRecords(filterExpression);
        }

        #region private methods
        private Dictionary<Guid, SchedulerTaskActionType> GetCachedSchedulerTaskActionTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSchedulerTaskActionTypes",
               () =>
               {
                   ISchedulerTaskActionTypeDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskActionTypeDataManager>();
                   IEnumerable<SchedulerTaskActionType> data = dataManager.GetAll();
                   return data.ToDictionary(at => at.ActionTypeId, at => at);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISchedulerTaskActionTypeDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskActionTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreSchedulerTaskActionTypesUpdated(ref _updateHandle);
            }
        }
        private ActionTypeExtendedSettings GetTaskActionExtendedSettings(ActionTypeInfo actionTypeInfo)
        {
            if (actionTypeInfo != null && actionTypeInfo.ExtendedSettings != null)
                return actionTypeInfo.ExtendedSettings;
            return new DefaultTaskActionExtentedSettings();
        }
        private bool CheckIfFilterIsMatch(SchedulerTaskActionType schedulerTaskActionType, List<ISchedulerTaskActionTypeFilter> filters)
        {
            var context = new SchedulerTaskActionTypeFilterContext { SchedulerTaskActionType = schedulerTaskActionType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region  Security
        public bool DoesUserHaveViewAccess()
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId);
        }
        public bool DoesUserHaveSpecialTaskViewAccess()
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveSpecialTaskViewAccess(userId);
        }
        public bool DoesUserHaveConfigureAccess()
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return DoesUserHaveConfigureAccess(userId);
        }
        public bool DoesUserHaveViewAccess(int userId)
        {
            return GetAllowedSchedulerTaskActionTypes(userId).Count > 0;
        }
        public bool DoesUserHaveSpecialTaskViewAccess(int userId)
        {
            return GetUserAllowedSchedulerTaskActionTypes(userId).Count > 0;
        }

        public bool DoesUserHaveConfigureAccess(int userId)
        {
            return GetAllowedConfigureSchedulerTaskActionTypes(userId).Count > 0;
        }
        public HashSet<Guid> GetAllowedSchedulerTaskActionTypes(int userId)
        {
            var actionTypeIds = new HashSet<Guid>();
            var items = GetCachedSchedulerTaskActionTypes().Values;
            foreach (var item in items)
            {
                if (DoesUserHaveViewAccess(userId, item.Info) && (!item.Info.IsUserTask) && item.Info.Editor!=null)
                    actionTypeIds.Add(item.ActionTypeId);
            }

            return actionTypeIds;
        }
        public HashSet<Guid> GetUserAllowedSchedulerTaskActionTypes(int userId)
        {
            var actionTypeIds = new HashSet<Guid>();
            var items = GetCachedSchedulerTaskActionTypes().Values;
            foreach (var item in items)
            {
                if (DoesUserHaveViewAccess(userId, item.Info))
                    actionTypeIds.Add(item.ActionTypeId);
            }
            return actionTypeIds;
        }

        public HashSet<Guid> GetAllowedConfigureSchedulerTaskActionTypes(int userId)
        {
            var actionTypeIds = new HashSet<Guid>();
            var items = GetCachedSchedulerTaskActionTypes().Values;
            foreach (var item in items)
            {
                if (DoesUserHaveConfigureAccess(userId, item.Info))
                    actionTypeIds.Add(item.ActionTypeId);
            }

            return actionTypeIds;
        }
       
       
        public bool DoesUserHaveViewAccess(int userId, ActionTypeInfo actionTypeInfo)
        {
            var definitionContext = new ActionTypeDoesUserHaveViewAccessContext { UserId = userId, ActionTypeInfo = actionTypeInfo };
            return GetTaskActionExtendedSettings(actionTypeInfo).DoesUserHaveViewAccess(definitionContext);
        }

        public bool DoesUserHaveConfigureAccess(int userId, ActionTypeInfo actionTypeInfo)
        {
            var definitionContext = new ActionTypeDoesUserHaveConfigureAccessContext { UserId = userId, ActionTypeInfo = actionTypeInfo };
            return GetTaskActionExtendedSettings(actionTypeInfo).DoesUserHaveConfigureTaskAccess(definitionContext);
        }
        public bool DoesUserHaveViewSpecificTaskAccess(SchedulerTask schedulerTask)
        {
            var context = new ActionTypeDoesUserHaveViewSpecificInstanceAccessContext
            {
                DefinitionContext = new ActionTypeDoesUserHaveViewAccessContext
                {
                    ActionTypeInfo = schedulerTask.ActionInfo,
                    UserId = ContextFactory.GetContext().GetLoggedInUserId()
                },
                TaskActionArgument = schedulerTask.TaskSettings.TaskActionArgument
            };

            return GetTaskActionExtendedSettings(schedulerTask.ActionInfo).DoesUserHaveViewSpecificTaskAccess(context);
        }

        public bool DoesUserHaveConfigureSpecificTaskAccess(SchedulerTask schedulerTask)
        {
            var context = new ActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext
            {
                DefinitionContext = new ActionTypeDoesUserHaveConfigureAccessContext
                {
                    ActionTypeInfo = schedulerTask.ActionInfo,
                    UserId = ContextFactory.GetContext().GetLoggedInUserId()
                },
                TaskActionArgument = schedulerTask.TaskSettings.TaskActionArgument
            };
            return GetTaskActionExtendedSettings(schedulerTask.ActionInfo).DoesUserHaveConfigureSpecificTaskAccess(context);
        }

        public bool DoesUserHaveRunSpecificTaskAccess(Guid schedulerTaskId)
        {
            var schedulerTask = new SchedulerTaskManager().GetTask(schedulerTaskId);
            return DoesUserHaveRunSpecificTaskAccess(schedulerTask);
        }

        public bool DoesUserHaveRunSpecificTaskAccess(SchedulerTask schedulerTask)
        {
            var context = new ActionTypeDoesUserHaveRunSpecificInstanceAccessContext
            {
                DefinitionContext = new ActionTypeDoesUserHaveRunAccessContext
                {
                    ActionTypeInfo = schedulerTask.ActionInfo,
                    UserId = ContextFactory.GetContext().GetLoggedInUserId()
                },
                TaskActionArgument = schedulerTask.TaskSettings.TaskActionArgument
            };
            return GetTaskActionExtendedSettings(schedulerTask.ActionInfo).DoesUserHaveRunSpecificTaskAccess(context);
        }


        #endregion

    }
}
