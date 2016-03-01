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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SchedulerTask")]
    public class SchedulerTaskController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredTasks")]
        public object GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return GetWebResponse(input, manager.GetFilteredTasks(input));
        }

        [HttpGet]
        [Route("GetTask")]
        public SchedulerTask GetTask(int taskId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetTask(taskId);
        }

        [HttpGet]
        [Route("GetSchedulesInfo")]
        public List<SchedulerTask> GetSchedulesInfo()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulesInfo();
        }
        
        [HttpGet]
        [Route("GetSchedulerTaskTriggerTypes")]
        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskTriggerTypes();
        }

        [HttpGet]
        [Route("GetSchedulerTaskActionTypes")]
        public List<SchedulerTaskActionType> GetSchedulerTaskActionTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskActionTypes();
        }

        [HttpPost]
        [Route("AddTask")]
        public Vanrise.Entities.InsertOperationOutput<SchedulerTask> AddTask(SchedulerTask taskObject)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.AddTask(taskObject);
        }

        [HttpPost]
        [Route("UpdateTask")]
        public Vanrise.Entities.UpdateOperationOutput<SchedulerTask> UpdateTask(SchedulerTask taskObject)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.UpdateTask(taskObject);
        }

        [HttpGet]
        [Route("DeleteTask")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteTask(int taskId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.DeleteTask(taskId);
        }
    }
}