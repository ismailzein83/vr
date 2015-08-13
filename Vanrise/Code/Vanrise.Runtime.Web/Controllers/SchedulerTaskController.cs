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
    public class SchedulerTaskController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return GetWebResponse(input, manager.GetFilteredTasks(input));
        }

        [HttpGet]
        public SchedulerTask GetTask(int taskId)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetTask(taskId);
        }

        [HttpGet]
        public List<SchedulerTaskTriggerType> GetSchedulerTaskTriggerTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskTriggerTypes();
        }

        [HttpGet]
        public List<SchedulerTaskActionType> GetSchedulerTaskActionTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskActionTypes();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<SchedulerTask> AddTask(SchedulerTask taskObject)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.AddTask(taskObject);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<SchedulerTask> UpdateTask(SchedulerTask taskObject)
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.UpdateTask(taskObject);
        }

    }
}