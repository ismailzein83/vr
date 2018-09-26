using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteSyncDefinition")]
    [JSONWithTypeAttribute]
    public class RouteSyncDefinitionController : BaseAPIController
    {
        RouteSyncDefinitionManager _manager = new RouteSyncDefinitionManager();

        [HttpPost]
        [Route("GetFilteredRouteSyncDefinitions")]
        public object GetFilteredRouteSyncDefinitions(Vanrise.Entities.DataRetrievalInput<RouteSyncDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRouteSyncDefinitions(input), "RouteSync Definitions");
        }

        [HttpGet]
        [Route("GetRouteSyncDefinition")]
        public RouteSyncDefinition GetRouteSyncDefinition(int RouteSyncDefinitionId)
        {
            return _manager.GetRouteSyncDefinitionById(RouteSyncDefinitionId);
        }

        [HttpPost]
        [Route("AddRouteSyncDefinition")]
        public Vanrise.Entities.InsertOperationOutput<RouteSyncDefinitionDetail> AddRouteSyncDefinition(RouteSyncDefinition RouteSyncDefinitionItem)
        {
            return _manager.AddRouteSyncDefinition(RouteSyncDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateRouteSyncDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<RouteSyncDefinitionDetail> UpdateRouteSyncDefinition(RouteSyncDefinition RouteSyncDefinitionItem)
        {
            return _manager.UpdateRouteSyncDefinition(RouteSyncDefinitionItem);
        }

        [HttpGet]
        [Route("GetRouteReaderExtensionConfigs")]
        public IEnumerable<RouteReaderConfig> GetRouteReaderExtensionConfigs()
        {
            return _manager.GetRouteReaderExtensionConfigs();
        }

        [HttpGet]
        [Route("GetRouteSyncDefinitionsInfo")]
        public IEnumerable<RouteSyncDefinitionInfo> GetRouteSyncDefinitionsInfo()
        {
            return _manager.GetRouteSyncDefinitionsInfo();
        }
    }
}