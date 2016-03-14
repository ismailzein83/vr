using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;
using Vanrise.Common;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPTask")]
    public class BPTaskController : BaseAPIController
    {
        [HttpPost]
        [Route("GetProcessTaskUpdated")]
        public BPTaskUpdateOutput GetProcessTaskUpdated(BPTaskUpdateInput input)
        {
            BPTaskManager manager = new BPTaskManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetProcessTaskUpdated(ref maxTimeStamp, input.NbOfRows, input.ProcessInstanceId);
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
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetMyTasksUpdated(ref maxTimeStamp, input.NbOfRows);
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
            input.ExecutedBy = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            manager.ExecuteTask(input);
        }

        [HttpGet]
        [Route("GetBPTaskType")]
        public BPTaskType GetBPTaskType(int taskTypeId)
        {
            BPTaskManager manager = new BPTaskManager();
            return manager.GetBPTaskType(taskTypeId);
        }

    }
}