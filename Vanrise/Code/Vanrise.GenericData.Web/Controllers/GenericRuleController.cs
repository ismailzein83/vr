﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRule")]
    public class GenericRuleController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredGenericRules")]
        public object GetFilteredGenericRules(Vanrise.Entities.DataRetrievalInput<GenericRuleQuery> input)
        {
            var manager = GetManager(input.Query.RuleDefinitionId);
            return GetWebResponse(input, manager.GetFilteredRules(input));
        }

        [HttpPost]
        [Route("AddGenericRule")]
        public Vanrise.Entities.InsertOperationOutput<GenericRule> AddGenericRule(GenericRule rule)
        {
            var manager = GetManager(rule.DefinitionId);
            return manager.AddGenericRule(rule);
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