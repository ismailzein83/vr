using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPBusinessRuleSetEffectiveAction")]
    public class BPBusinessRuleSetEffectiveActionController : BaseAPIController
    {
        BPBusinessRuleSetEffectiveActionManager _manager = new BPBusinessRuleSetEffectiveActionManager();
           
        [HttpPost]
        [Route("GetFilteredBPBusinessRuleSetsEffectiveActions")]
        public object GetFilteredBPBusinessRuleSetsEffectiveActions(Vanrise.Entities.DataRetrievalInput<BPBusinessRuleEffectiveActionQuery> input)
        {
            return _manager.GetFilteredBPBusinessRuleSetsEffectiveActions(input);
        }

        [HttpGet]
        [Route("GetRuleActionsExtensionConfigs")]
        public IEnumerable<BPBusinessRuleActionType> GetRuleActionsExtensionConfigs(string serializedFilter)
        {
            ActionTypesInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ActionTypesInfoFilter>(serializedFilter) : null;
            return _manager.GetRuleActionsExtensionConfigs(filter);
        }
        [HttpGet]
        [Route("GetParentActionDescription")]
        public string GetParentActionDescription(int ruleSetId, Guid ruleDefinitionId)
        {
            return _manager.GetParentActionDescription(ruleSetId,ruleDefinitionId);
        }
    }
}