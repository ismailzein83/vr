﻿using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
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
            return GetWebResponse(input, manager.GetFilteredBPDefinitions(input, Security.Entities.ContextFactory.GetContext().GetLoggedInUserId()));
        }

        [HttpPost]
        [Route("GetFilteredBPDefinitionsForTechnical")]
        public object GetFilteredBPDefinitionsForTechnical(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            BPDefinitionManager manager = new BPDefinitionManager();
            return GetWebResponse(input, manager.GetFilteredBPDefinitions(input, null));
        }

        [HttpGet]
        [Route("GetBPDefinitionsInfo")]
        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(string serializedFilter)
        {
            BPDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<BPDefinitionInfoFilter>(serializedFilter) : null;
            BPDefinitionManager manager = new BPDefinitionManager();
            return manager.GetBPDefinitionsInfo(filter);
        }

        [HttpGet]
        [Route("GetBPDefintion")]
        public BPDefinition GetBPDefintion(int bpDefinitionId)
        {
            BPDefinitionManager manager = new BPDefinitionManager();
            return manager.GetBPDefinition(bpDefinitionId);
        }

        [HttpGet]
        [Route("GetDefinitions")]
        public IEnumerable<BPDefinition> GetDefinitions()
        {
            BPDefinitionManager manager = new BPDefinitionManager();
            return manager.GetBPDefinitions();
        }
    }
}