using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Runtime.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SchedulerTask")]
    public class SchedulerTaskController : Vanrise.Web.Base.BaseAPIController
    {

        SchedulerTaskActionTypeManager _taskActionTypeManager = new SchedulerTaskActionTypeManager();
        SchedulerTaskManager _schedulerTaskManager = new SchedulerTaskManager();
        [HttpPost]
        [Route("GetFilteredTasks")]
        public object GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            if (!_taskActionTypeManager.DoesUserHaveViewAccess())
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _schedulerTaskManager.GetFilteredTasks(input));
        }

        [HttpPost]
        [Route("GetFilteredMyTasks")]
        public object GetFilteredMyTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            return GetWebResponse(input, _schedulerTaskManager.GetFilteredMyTasks(input));
        }

        [HttpGet]
        [Route("GetTask")]
        public SchedulerTask GetTask(Guid taskId)
        {
            return _schedulerTaskManager.GetTask(taskId, true);
        }

        [HttpGet]
        [Route("GetSchedulesInfo")]
        public List<SchedulerTaskInfo> GetSchedulesInfo()
        {
            return _schedulerTaskManager.GetTasksInfo();
        }

        [HttpGet]
        [Route("GetMySchedulesInfo")]
        public List<SchedulerTaskInfo> GetMySchedulesInfo()
        {
            return _schedulerTaskManager.GetMyTasksInfo();
        }

        [HttpGet]
        [Route("GetSchedulerTaskTriggerTypes")]
        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            return new SchedulerTaskTriggerTypeManager().GetSchedulerTaskTriggerTypes();
        }
        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess()
        {
            return _taskActionTypeManager.DoesUserHaveConfigureAccess();
        }

        [HttpPost]
        [Route("AddTask")]
        public object AddTask(SchedulerTask taskObject)
        {
            if (!_taskActionTypeManager.DoesUserHaveConfigureSpecificTaskAccess(taskObject))
                return GetUnauthorizedResponse();

            return _schedulerTaskManager.AddTask(taskObject);
        }

        [HttpPost]
        [Route("UpdateTask")]
        public object UpdateTask(SchedulerTask taskObject)
        {
            if (!_taskActionTypeManager.DoesUserHaveConfigureSpecificTaskAccess(taskObject))
                return GetUnauthorizedResponse();

            return _schedulerTaskManager.UpdateTask(taskObject);
        }

        [HttpGet]
        [Route("DeleteTask")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteTask(Guid taskId)
        {
            return _schedulerTaskManager.DeleteTask(taskId);
        }

        [HttpPost]
        [Route("GetUpdated")]
        public SchedulerTaskStateUpdateOutput GetUpdated(SchedulerTaskUpdateInput input)
        {
            SchedulerTaskStateManager manager = new SchedulerTaskStateManager();
            return manager.GetUpdated(input.Filter.TaskIds);
        }

        [HttpGet]
        [Route("RunSchedulerTask")]
        public void RunSchedulerTask(Guid taskId)
        {
            if (!_taskActionTypeManager.DoesUserHaveRunSpecificTaskAccess(taskId))
                GetUnauthorizedResponse();
            SchedulerTaskStateManager manager = new SchedulerTaskStateManager();
            manager.RunSchedulerTask(taskId);
        }

        [HttpGet]
        [Route("DisableTask")]
        public object DisableTask(Guid taskId)
        {
            if (!_schedulerTaskManager.DoesUserHaveConfigureSpecificTaskAccess(taskId))
                return GetUnauthorizedResponse();

            return _schedulerTaskManager.DisableTask(taskId);
        }

        [HttpGet]
        [Route("EnableTask")]
        public object EnableTask(Guid taskId)
        {
            if (!_schedulerTaskManager.DoesUserHaveConfigureSpecificTaskAccess(taskId))
                return GetUnauthorizedResponse();

            return _schedulerTaskManager.EnableTask(taskId);
        }
        [HttpGet]
        [Route("DoesUserHaveConfigureAllTaskAccess")]
        public bool DoesUserHaveConfigureAllTaskAccess()
        {
            return _schedulerTaskManager.DoesUserHaveConfigureAllTaskAccess();
        }

        [HttpGet]
        [Route("EnableAllTasks")]
        public bool EnableAllTasks()
        {
            if (!DoesUserHaveConfigureAllTaskAccess())
                return false;
            return _schedulerTaskManager.EnableAllTasks();
        }
        
        [HttpGet]
        [Route("DisableAllTasks")]
        public bool DisableAllTasks()
        {
            if (!DoesUserHaveConfigureAllTaskAccess())
                return false;
            return _schedulerTaskManager.DisableAllTasks();
        }

        [HttpGet]
        [Route("GetTaskManagmentInfo")]
        public SchedulerTaskManagmentInfo GetTaskManagmentInfo()
        {
            return _schedulerTaskManager.GetTaskManagmentInfo();
        }

    }
}