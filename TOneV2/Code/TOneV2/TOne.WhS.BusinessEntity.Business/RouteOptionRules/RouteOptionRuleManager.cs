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
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public RouteOptionRule GetMostMatchedRule(Vanrise.Rules.RuleTree ruleTree, RouteOptionIdentifier routeOptionIdentifier)
        {
            return ruleTree.GetMatchRule(routeOptionIdentifier) as RouteOptionRule;
        }
    }
}
