using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ActionDefinition")]
    [JSONWithTypeAttribute]
    public class ActionDefinitionController:BaseAPIController
    {
        ActionDefinitionManager _manager = new ActionDefinitionManager();
       
        [HttpPost]
        [Route("AddActionDefinition")]
        public Vanrise.Entities.InsertOperationOutput<ActionDefinitionDetail> AddActionDefinition(ActionDefinition actionDefinition)
        {
            return _manager.AddActionDefinition(actionDefinition);
        }

        [HttpPost]
        [Route("UpdateActionDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<ActionDefinitionDetail> UpdateActionDefinition(ActionDefinition actionDefinition)
        {
            return _manager.UpdateActionDefinition(actionDefinition);
        }

        [HttpPost]
        [Route("GetFilteredActionDefinitions")]
        public object GetFilteredActionDefinitions(Vanrise.Entities.DataRetrievalInput<ActionDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredActionDefinitions(input));
        }

        [HttpGet]
        [Route("GetActionDefinition")]
        public ActionDefinition GetActionDefinition(Guid actionDefinitionId)
        {
            return _manager.GetActionDefinition(actionDefinitionId);
        }
        [HttpGet]
        [Route("GetActionBPDefinitionExtensionConfigs")]
        public IEnumerable<ActionBPDefinitionConfig> GetActionBPDefinitionExtensionConfigs()
        {
            return _manager.GetActionBPDefinitionExtensionConfigs();
        }
        [HttpGet]
        [Route("GetProvisionerDefinitionExtensionConfigs")]
        public IEnumerable<ProvisionerDefinitionConfig> GetProvisionerDefinitionExtensionConfigs()
        {
            return _manager.GetProvisionerDefinitionExtensionConfigs();
        }
        [HttpGet]
        [Route("GetAccountProvisionDefinitionPostActionConfigs")]
        public IEnumerable<ProvisionDefinitionPostActionConfig> GetAccountProvisionDefinitionPostActionConfigs()
        {
            return _manager.GetAccountProvisionDefinitionPostActionConfigs();
        }
        
         [HttpGet]
         [Route("GetActionDefinitionsInfo")]
        public IEnumerable<ActionDefinitionInfo> GetActionDefinitionsInfo(string filter = null)
        {
            ActionDefinitionInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ActionDefinitionInfoFilter>(filter) : null;
            return _manager.GetActionDefinitionsInfo(deserializedFilter);
        }

        
    }
}