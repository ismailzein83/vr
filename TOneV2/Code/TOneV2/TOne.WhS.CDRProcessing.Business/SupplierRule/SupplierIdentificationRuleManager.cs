using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common;
using Vanrise.Rules;
namespace TOne.WhS.CDRProcessing.Business
{
    public class SupplierIdentificationRuleManager : Vanrise.Rules.RuleManager<SupplierIdentificationRule, SupplierIdentificationRuleDetail>
    {
        public SupplierIdentificationRule GetMatchRule(SupplierIdentificationRuleTarget target)
        {
            var ruleTree = GetRuleTree();
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as SupplierIdentificationRule;
        }
        RuleTree GetRuleTree()
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_SupplierIdentificationRules"),
                () =>
                {
                    return new Vanrise.Rules.RuleTree(base.GetAllRules().Values, GetRuleStructureBehaviors());
                });
        }
        IEnumerable<BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new SupplierRule.Rules.StructureRulesBehaviors.RuleBehaviorByOutCarrier());
            ruleStructureBehaviors.Add(new SupplierRule.Rules.StructureRulesBehaviors.RuleBehaviorByOutTrunk());
            ruleStructureBehaviors.Add(new SupplierRule.Rules.StructureRulesBehaviors.RuleBehaviorByCDPNPrefix());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierIdentificationRuleDetail> GetFilteredSupplierIdentificationRules(Vanrise.Entities.DataRetrievalInput<SupplierIdentificationRuleQuery> input)
        {
            Func<SupplierIdentificationRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SupplierIdentificationRuleDetail MapToDetails(SupplierIdentificationRule rule)
        {
            return new SupplierIdentificationRuleDetail{
                Entity=rule
            };
        }
    }
}
