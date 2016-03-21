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
            return GetWebResponse(input, manager.GetFilteredBPValidationMessage(input));
        }
    }
}