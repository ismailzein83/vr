using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRule")]
    public class GenericRuleController : Vanrise.Web.Base.BaseAPIController
    {
        GenericRuleDefinitionManager _manager = new GenericRuleDefinitionManager();

        [HttpPost]
        [Route("GetFilteredGenericRules")]
        public object GetFilteredGenericRules(Vanrise.Entities.DataRetrievalInput<GenericRuleQuery> input)
        {
            if (!_manager.DoesUserHaveViewAccess(input.Query.RuleDefinitionId))
                return GetUnauthorizedResponse();
            
            var manager = GetManager(input.Query.RuleDefinitionId);
            return GetWebResponse(input, manager.GetFilteredRules(input));
        }

        [HttpGet]
        [Route("GetGenericRule")]
        public GenericRule GetGenericRule(int ruleDefinitionId, int ruleId)
        {
            var manager = GetManager(ruleDefinitionId);
            return manager.GetGenericRule(ruleId);
        }

        [HttpPost]
        [Route("AddGenericRule")]
        public object AddGenericRule(GenericRule rule)
        {
            if (!DoesUserHaveAddAccess(rule.DefinitionId))
             return   GetUnauthorizedResponse();

            var manager = GetManager(rule.DefinitionId);
            return manager.AddGenericRule(rule);
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(int ruleDefinitionId)
        {
            return _manager.DoesUserHaveAddAccess(ruleDefinitionId);
        }

        [HttpPost]
        [Route("UpdateGenericRule")]
        public object UpdateGenericRule(GenericRule rule)
        {
            if (!DoesUserHaveEditAccess(rule.DefinitionId))
                return GetUnauthorizedResponse();

            var manager = GetManager(rule.DefinitionId);
            return manager.UpdateGenericRule(rule);
           
        }

        [HttpGet]
        [Route("DoesUserHaveEditAccess")]
        public bool DoesUserHaveEditAccess(int ruleDefinitionId)
        {
            return _manager.DoesUserHaveEditAccess(ruleDefinitionId);
        }

        [HttpPost]
        [Route("DeleteGenericRule")]
        public Vanrise.Entities.DeleteOperationOutput<GenericRuleDetail> DeleteGenericRule(GenericRule rule)
        {
            var manager = GetManager(rule.DefinitionId);
            return manager.DeleteGenericRule(rule.RuleId);
        }

        IGenericRuleManager GetManager(int ruleDefinitionId)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);
            
            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
    }
}