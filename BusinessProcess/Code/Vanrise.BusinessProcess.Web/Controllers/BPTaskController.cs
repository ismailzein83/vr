using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPTask")]
    public class BPTaskController : BaseAPIController
    {
        [HttpPost]
        [Route("GetProcessTaskUpdated")]
        public BPTaskUpdateOutput GetProcessTaskUpdated(BPTaskUpdateInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetProcessTaskUpdated(input.LastUpdateHandle, input.NbOfRows, input.ProcessInstanceId);
        }

        [HttpPost]
        [Route("GetProcessTaskBeforeId")]
        public List<BPTaskDetail> GetProcessTaskBeforeId(BPTaskBeforeIdInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetProcessTaskBeforeId(input);
        }

        [HttpPost]
        [Route("GetMyUpdatedTasks")]
        public BPTaskUpdateOutput GetMyTasksUpdated(BPTaskUpdateInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetMyTasksUpdated(input.LastUpdateHandle, input.NbOfRows);
        }

        [HttpPost]
        [Route("GetMyTasksBeforeId")]
        public List<BPTaskDetail> GetMyTasksBeforeId(BPTaskBeforeIdInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetMyTasksBeforeId(input);
        }

        [HttpPost]
        [Route("ExecuteTask")]
        public void ExecuteTask(ExecuteBPTaskInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            manager.ExecuteTask(input);
        }

        [HttpGet]
        [Route("GetTask")]
        public BPTask GetTask(long taskId)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetTask(taskId);
        }

    }
}