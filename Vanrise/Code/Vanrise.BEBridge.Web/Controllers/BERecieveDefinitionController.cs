using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.BEBridge.Business;
using Vanrise.BEBridge.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BEBridge.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BERecieveDefinition")]
    public class BERecieveDefinitionController : BaseAPIController
    {
        BEReceiveDefinitionManager _manager = new BEReceiveDefinitionManager();
        [HttpGet]
        [Route("GetBERecieveDefinitionsInfo")]
        public object GetBERecieveDefinitionsInfo()
        {
            return _manager.GetBEReceiveDefinitionsInfo();
        }
        [HttpPost]
        [Route("GetFilteredBeReceiveDefinitions")]
        public object GetFilteredBeReceiveDefinitions(DataRetrievalInput<BEReceiveDefinitionQuery> input)
        {
            input.SortByColumnName = "Entity.BEReceiveDefinitionId";
            return GetWebResponse(input, _manager.GetFilteredBeReceiveDefinitions(input));
        }
        [HttpGet]
        [Route("GetReceiveDefinition")]
        public BEReceiveDefinition GetReceiveDefinition(Guid receiveDefinitionId)
        {
            return _manager.GetReceiveDefinition(receiveDefinitionId);
        }
        [HttpPost]
        [Route("UpdateRedeciveDefinition")]
        public UpdateOperationOutput<BEReceiveDefinitionDetail> UpdateRedeciveDefinition(BEReceiveDefinition beReceiveDefinition)
        {
            return _manager.UpdateRedeciveDefinition(beReceiveDefinition);
        }
        [HttpPost]
        [Route("AddReceiveDefinition")]
        public InsertOperationOutput<BEReceiveDefinitionDetail> AddStatusChargingSet(BEReceiveDefinition beReceiveDefinition)
        {
            return _manager.AddReceiveDefinition(beReceiveDefinition);
        }
    }
}