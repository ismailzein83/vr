using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPBusinessRuleSet")]
    public class BPBusinessRuleSetController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredBPBusinessRuleSets")]
        public object GetFilteredBPBusinessRuleSets(Vanrise.Entities.DataRetrievalInput<BPBusinessRuleSetQuery> input)
        {
            BPBusinessRuleSetManager manager = new BPBusinessRuleSetManager();
            return GetWebResponse(input, manager.GetFilteredBPBusinessRuleSets(input));
        }

        [HttpPost]
        [Route("AddBusinessRuleSet")]
        public Vanrise.Entities.InsertOperationOutput<BPBusinessRuleSetDetail> AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            BPBusinessRuleSetManager manager = new BPBusinessRuleSetManager();
            return manager.AddBusinessRuleSet(businessRuleSetObj);
        }

        [HttpPost]
        [Route("UpdateBusinessRuleSet")]
        public Vanrise.Entities.UpdateOperationOutput<BPBusinessRuleSetDetail> UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            BPBusinessRuleSetManager manager = new BPBusinessRuleSetManager();
            return manager.UpdateBusinessRuleSet(businessRuleSetObj);
        }

        [HttpGet]
        [Route("GetRuleActionsExtensionConfigs")]
        public IEnumerable<BPBusinessRuleActionType> GetRuleActionsExtensionConfigs(string serializedFilter)
        {
            ActionTypesInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ActionTypesInfoFilter>(serializedFilter) : null;
            BPBusinessRuleSetEffectiveActionManager manager = new BPBusinessRuleSetEffectiveActionManager();
            return manager.GetRuleActionsExtensionConfigs(filter);
        }
        [HttpGet]
        [Route("GetBusinessRuleSetsInfo")]
        public List<BPBusinessRuleSet> GetBusinessRuleSetsInfo(string serializedFilter)
        {
            BPBusinessRuleSetInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<BPBusinessRuleSetInfoFilter>(serializedFilter) : null;
            BPBusinessRuleSetManager manager = new BPBusinessRuleSetManager();
            return manager.GetBusinessRuleSetsInfo(filter);
        }

        [HttpGet]
        [Route("GetBusinessRuleSetsByID")]
        public BPBusinessRuleSet GetBusinessRuleSetsByID(int businessRuleSetId)
        {
            BPBusinessRuleSetManager manager = new BPBusinessRuleSetManager();
            return manager.GetBusinessRuleSetsByID(businessRuleSetId);
        }

    }
}