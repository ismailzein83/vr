using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager
    {
        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {

        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {

        }

        public StructuredNormalizationRules StructureRules(List<NormalizationRule> rules) 
        {
            List<NormalizationRulesByCriteria> rulesByCriteria = new List<NormalizationRulesByCriteria>() ;
            rulesByCriteria.Add(new Normalization.RulesByCriteria.RulesBySwitchTrunkPrefixLength());
            rulesByCriteria.Add(new Normalization.RulesByCriteria.RulesBySwitchTrunkPrefix());
            rulesByCriteria.Add(new Normalization.RulesByCriteria.RulesBySwitchTrunkLength());

            StructuredNormalizationRules structuredRules = new StructuredNormalizationRules();
            NormalizationRulesByCriteria current = null;
            foreach (var r in rulesByCriteria)
            {
                r.SetSource(rules);
                if (!r.IsEmpty())
                {
                    if (current != null)
                        current.NextRuleSet = r;
                    else
                        structuredRules.FirstRuleSet = r;
                    current = r;
                }
            }
            return structuredRules;
        }

        public NormalizationRule GetMostMatchedRule(StructuredNormalizationRules rules, int switchId, int trunkId, string phoneNumber)
        {
            return GetMostMatchedRule(rules.FirstRuleSet, switchId, trunkId, phoneNumber);
        }

        NormalizationRule GetMostMatchedRule(NormalizationRulesByCriteria rulesByCriteria, int switchId, int trunkId, string phoneNumber)
        {
            if (rulesByCriteria == null)
                return null;
            NormalizationRule rule = rulesByCriteria.GetMostMatchedRule(switchId, trunkId, phoneNumber);
            if (rule != null)
                return rule;
            else
                return GetMostMatchedRule(rulesByCriteria.NextRuleSet, switchId, trunkId, phoneNumber);
        }
    }
}
