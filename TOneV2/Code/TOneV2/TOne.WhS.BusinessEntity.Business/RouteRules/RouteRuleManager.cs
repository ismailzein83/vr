using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule, RouteRuleDetail>
    {        
        public RouteRule GetMatchRule(RouteRuleTarget target)
        {
            var ruleTrees = GetRuleTreesByPriority();
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(target) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        Vanrise.Rules.RuleTree[] GetRuleTreesByPriority()
        {
            return GetCachedOrCreate("GetRuleTreesByPriority",
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeRuleTypes = GetRouteRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.BaseRule> currentRules = null;
                    foreach (var ruleType in routeRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        { 
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.BaseRule>();
                        }
                        var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.TemplateConfigID);
                        if (ruleTypeRules != null)
                            currentRules.AddRange(ruleTypeRules);
                    }
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    return ruleTrees.ToArray();
                });
        }

        int GetRuleTypePriority(TemplateConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Settings != null ? (ruleTypeConfig.Settings as RouteRuleTypeSettings).Priority : int.MaxValue;
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteRuleTypesTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteRuleConfigType);
        }

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            var routeRules = base.GetAllRules();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeRules.ToBigResult(input, null));
        }

        protected override RouteRuleDetail MapToDetails(RouteRule rule)
        {
            return new RouteRuleDetail()
            {
                Rule = rule
            };
        }
    }
}
