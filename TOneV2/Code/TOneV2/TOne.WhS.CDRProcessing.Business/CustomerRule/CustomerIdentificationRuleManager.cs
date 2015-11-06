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
    public class CustomerIdentificationRuleManager : Vanrise.Rules.RuleManager<CustomerIdentificationRule, CustomerIdentificationRuleDetail>
    {
        public CustomerIdentificationRule GetMatchRule(CustomerIdentificationRuleTarget target)
        {
            var ruleTree = GetRuleTree();
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as CustomerIdentificationRule;
        }
        RuleTree GetRuleTree()
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_CustomerIdentificationRules"),
                () =>
                {
                    return new Vanrise.Rules.RuleTree(base.GetAllRules().Values, GetRuleStructureBehaviors());
                });
        }
        IEnumerable<BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new CustomerRule.Rules.StructureRulesBehaviors.RuleBehaviorByInCarrier());
            ruleStructureBehaviors.Add(new CustomerRule.Rules.StructureRulesBehaviors.RuleBehaviorByInTrunk());
            ruleStructureBehaviors.Add(new CustomerRule.Rules.StructureRulesBehaviors.RuleBehaviorByCDPNPrefix());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<CustomerIdentificationRuleDetail> GetFilteredCustomerIdentificationRules(Vanrise.Entities.DataRetrievalInput<CustomerIdentificationRuleQuery> input)
        {
            Func<CustomerIdentificationRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()))
                && (input.Query.CustomerIds == null || input.Query.CustomerIds.Contains(prod.Settings.CustomerId))
                  && (input.Query.CDPN == null || prod.Criteria.CDPNPrefixes.Contains(input.Query.CDPN.ToLower()))
                    && (input.Query.InCarrier == null || prod.Criteria.InCarriers.Contains(input.Query.InCarrier.ToLower()))
                      && (input.Query.InTrunk == null || prod.Criteria.InTrunks.Contains(input.Query.InTrunk.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override CustomerIdentificationRuleDetail MapToDetails(CustomerIdentificationRule rule)
        {
            return new CustomerIdentificationRuleDetail
            {
                Entity = rule
            };
        }
    }
}
