﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRuleManager
    {
        public Vanrise.Rules.RuleTree GetStructuredRules()
        {
            List<Vanrise.Rules.BaseRule> rules = null;
            //TODO: get rules from database

            var ruleStructureBehaviors = GetRuleStructureBehaviors();
            return new Vanrise.Rules.RuleTree(rules, ruleStructureBehaviors);
        }

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public RouteRule GetMostMatchedRule(Vanrise.Rules.RuleTree ruleTree, RouteIdentifier routeIdentifier)
        {
            return ruleTree.GetMatchRule(routeIdentifier) as RouteRule;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredRouteRules(input));
        }

        public RouteRule GetRouteRule(int routeRuleId)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            return dataManager.GetRouteRule(routeRuleId);
        }


        public TOne.Entities.InsertOperationOutput<RouteRule> AddRouteRule(RouteRule routeRule)
        {
            TOne.Entities.InsertOperationOutput<RouteRule> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RouteRule>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routeRuleId = -1;

            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            bool insertActionSucc = dataManager.Insert(routeRule, out routeRuleId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routeRule.RouteRuleId = routeRuleId;
                insertOperationOutput.InsertedObject = routeRule;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RouteRule> UpdateRouteRule(RouteRule routeRule)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();

            bool updateActionSucc = dataManager.Update(routeRule);
            TOne.Entities.UpdateOperationOutput<RouteRule> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RouteRule>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = routeRule;
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRouteRule(int routeRuleId)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routeRuleId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


        public List<Vanrise.Entities.TemplateConfig> GetCodeCriteriaGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CodeCriteriaGroupConfigType);
        }
    }
}
