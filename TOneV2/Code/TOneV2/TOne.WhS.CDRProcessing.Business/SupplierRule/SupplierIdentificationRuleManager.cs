using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()))
                && (input.Query.SupplierIds == null || input.Query.SupplierIds.Contains(prod.Settings.SupplierId))
                  && (input.Query.CDPN == null || prod.Criteria.CDPNPrefixes.Any(x=>x.ToLower().Contains(input.Query.CDPN.ToLower())))
                    && (input.Query.OutCarrier == null || prod.Criteria.OutCarriers.Any(x=>x.ToLower().Contains(input.Query.OutCarrier.ToLower())))
                      && (input.Query.OutTrunk == null || prod.Criteria.OutTrunks.Any(x=>x.ToLower().Contains(input.Query.OutTrunk.ToLower())))
                       && (input.Query.EffectiveDate == null || (prod.BeginEffectiveTime <= input.Query.EffectiveDate && (prod.EndEffectiveTime == null || prod.EndEffectiveTime >= input.Query.EffectiveDate)));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SupplierIdentificationRuleDetail MapToDetails(SupplierIdentificationRule rule)
        {
            CarrierAccountManager CarrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = CarrierAccountManager.GetCarrierAccount(rule.Settings.SupplierId);
            return new SupplierIdentificationRuleDetail{
                Entity=rule,
                CDPNPrefixes = GetDescription(rule.Criteria.CDPNPrefixes),
                OutTrunks = GetDescription(rule.Criteria.OutTrunks),
                OutCarriers = GetDescription(rule.Criteria.OutCarriers),
                SupplierName=carrierAccount.Name
            };

        }
        private string GetDescription(IEnumerable<string> list)
        {
            if (list != null)
                return string.Join(", ", list);
            else
                return null;
        } 
    }
}
