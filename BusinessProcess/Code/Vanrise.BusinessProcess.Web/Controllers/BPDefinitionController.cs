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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPDefinition")]
    public class BPDefinitionController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredBPDefinitions")]
        public object GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            BPDefinitionManager manager = new BPDefinitionManager();
            return GetWebResponse(input, manager.GetFilteredBPDefinitions(input));
        }

        [HttpGet]
        [Route("GetBPDefinitionsInfo")]
        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(string serializedFilter)
        {
            BPDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<BPDefinitionInfoFilter>(serializedFilter) : null;
            BPDefinitionManager manager = new BPDefinitionManager();
            return manager.GetBPDefinitionsInfo(filter);
        }
    }
}