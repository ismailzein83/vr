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

        IEnumerable<Vanrise.Rules.Entities.BaseRuleSet> GetRuleSets()
        {
            List<Vanrise.Rules.Entities.BaseRuleSet> ruleSets = new List<Vanrise.Rules.Entities.BaseRuleSet>();
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetBySwitchTrunkLength());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetBySwitchTrunk());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetBySwitchLength());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetByTrunkLength());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetBySwitch());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetByTrunk());
            ruleSets.Add(new Entities.Normalization.RuleSets.RuleSetByLength());
            return ruleSets;
        }

        public NormalizationRule GetMostMatchedRule(Vanrise.Rules.Entities.StructuredRules rules, CDRToNormalizeInfo cdr)
        {
            Vanrise.Rules.Business.RuleManager ruleManager = new Vanrise.Rules.Business.RuleManager();
            return ruleManager.GetMostMatchedRule(rules, cdr) as NormalizationRule;
        }
    }
}
