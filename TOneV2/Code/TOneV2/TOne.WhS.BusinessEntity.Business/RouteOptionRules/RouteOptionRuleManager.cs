using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteOptionRuleManager : Vanrise.Rules.RuleManager<RouteOptionRule>
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
            ruleStructureBehaviors.Add(new RouteOptionRules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());
            ruleStructureBehaviors.Add(new RouteOptionRules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new RouteRules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public RouteOptionRule GetMostMatchedRule(Vanrise.Rules.RuleTree ruleTree, RouteOptionIdentifier routeOptionIdentifier)
        {
            return ruleTree.GetMatchRule(routeOptionIdentifier) as RouteOptionRule;
        }
    }
}
