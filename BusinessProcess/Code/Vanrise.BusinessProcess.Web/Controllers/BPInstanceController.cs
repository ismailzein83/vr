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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPInstance")]
    public class BPInstanceController : BaseAPIController
    {
        [HttpPost]
        [Route("GetUpdated")]
        public BPInstanceUpdateOutput GetUpdated(BPInstanceUpdateInput input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetUpdated(ref maxTimeStamp, input.NbOfRows, input.DefinitionsId);
        }

        [HttpPost]
        [Route("GetBeforeId")]
        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return manager.GetBeforeId(input);
        }

        [HttpPost]
        [Route("GetFilteredBPInstances")]
        public object GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return GetWebResponse(input, manager.GetFilteredBPInstances(input));
        }

    }
}