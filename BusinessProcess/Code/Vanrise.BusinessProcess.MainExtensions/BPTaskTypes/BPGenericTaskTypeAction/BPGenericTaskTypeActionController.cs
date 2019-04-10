using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPGenericTaskTypeAction")]
    public class BPGenericTaskTypeActionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetTaskTypeActions")]
        public List<BPGenericTaskTypeAction> GetTaskTypeActions(long taskId)
        {
            BPGenericTaskTypeActionManager manager = new BPGenericTaskTypeActionManager();
            return manager.GetTaskTypeActions(taskId);
        }
        [HttpPost]
        [Route("GetMappingFieldsDescription")]
        public BPGenericTaskTypeActionMappingFieldsOutput GetMappingFieldsDescription(BPGenericTaskTypeActionMappingFieldsInput input)
        {
            BPGenericTaskTypeActionManager manager = new BPGenericTaskTypeActionManager();
            return manager.GetMappingFieldsDescription(input);
        }
    }
}
