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
        [Route("GetBPBusinessRuleSetsEffectiveActions")]
        public object GetBPBusinessRuleSetsEffectiveActions(BPBusinessRuleEffectiveActionQuery input)
        {
            return _manager.GetBPBusinessRuleSetsEffectiveActions(input);
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
        [HttpGet]
        [Route("GetRuleSetEffectiveActions")]
        public List<BPBusinessRuleActionDetails> GetRuleSetEffectiveActions(int bpBusinessRuleSetId)
        {
            return _manager.GetRuleSetEffectiveActions(bpBusinessRuleSetId);
        }
    }
}



