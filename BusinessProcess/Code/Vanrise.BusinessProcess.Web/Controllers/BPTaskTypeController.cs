using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPTaskType")]
    public class BPTaskTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBPTaskTypeByTaskId")]
        public BPTaskType GetBPTaskTypeByTaskId(long taskId)
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBPTaskTypeByTaskId(taskId);
        }
        [HttpGet]
        [Route("GetBaseBPTaskTypeSettingsConfigs")]
        public object GetBaseBPTaskTypeSettingsConfigs()
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBaseBPTaskTypeSettingsConfigs();
        }
    }
}