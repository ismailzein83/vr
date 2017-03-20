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
        [HttpPost]
        [Route("GetFilteredTasks")]
        public object GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            if (!_taskActionTypeManager.DoesUserHaveViewAccess())
                return GetUnauthorizedResponse();
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return GetWebResponse(input, manager.GetFilteredTasks(input));
        }

        [HttpPost]
        [Route("GetFilteredMyTasks")]
        public object GetFilteredMyTasks(Vanrise.Entities.DataRetrievalInput<SchedulerTaskQuery> input)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return GetWebResponse(input, manager.GetFilteredMyTasks(input));
        }

        [HttpGet]
        [Route("GetTask")]
        public SchedulerTask GetTask(Guid taskId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetTask(taskId);
        }

        [HttpGet]
        [Route("GetSchedulesInfo")]
        public List<SchedulerTaskInfo> GetSchedulesInfo()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetTasksInfo();
        }

        [HttpGet]
        [Route("GetMySchedulesInfo")]
        public List<SchedulerTaskInfo> GetMySchedulesInfo()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetMyTasksInfo();
        }

        [HttpGet]
        [Route("GetSchedulerTaskTriggerTypes")]
        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskTriggerTypes();
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

            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.AddTask(taskObject);
        }

        [HttpPost]
        [Route("UpdateTask")]
        public object UpdateTask(SchedulerTask taskObject)
        {
            if (!_taskActionTypeManager.DoesUserHaveConfigureSpecificTaskAccess(taskObject))
                return GetUnauthorizedResponse();

            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.UpdateTask(taskObject);
        }

        [HttpGet]
        [Route("DeleteTask")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteTask(Guid taskId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.DeleteTask(taskId);
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
    }
}