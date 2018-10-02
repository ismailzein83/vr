using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPValidationMessage")]
    public class BPValidationMessageController : BaseAPIController
    {
        [HttpPost]
        [Route("GetUpdated")]
        public BPValitaionMessageUpdateOutput GetUpdated(BPValidationMessageUpdateInput input)
        {
            BPValidationMessageManager manager = new BPValidationMessageManager();
            return manager.GetUpdated(input);
        }

        [HttpPost]
        [Route("GetBeforeId")]
        public List<BPValidationMessageDetail> GetBeforeId(BPValidationMessageBeforeIdInput input)
        {
            BPValidationMessageManager manager = new BPValidationMessageManager();
            return manager.GetBeforeId(input);
        }

        [HttpPost]
        [Route("GetFilteredBPValidationMessage")]
        public object GetFilteredBPValidationMessage(Vanrise.Entities.DataRetrievalInput<BPValidationMessageQuery> input)
        {
            BPValidationMessageManager manager = new BPValidationMessageManager();
            return GetWebResponse(input, manager.GetFilteredBPValidationMessage(input), "Messages Validations");
        }
    }
}