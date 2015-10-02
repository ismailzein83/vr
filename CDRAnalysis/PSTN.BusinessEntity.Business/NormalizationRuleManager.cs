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

            CDRToNormalizeInfo cdrInfo1 = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID.Value , PhoneNumberType= NormalizationPhoneNumberType.CDPN,    TrunkId=cdr.InTrunkId.Value };

            CDRToNormalizeInfo cdrInfo2 = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.OutTrunkId.Value };

            NormalizationRule matchRule1 = GetMostMatchedRule(_rules, cdrInfo1);
            if (matchRule1 != null)
            {
                string phoneNumber = cdr.CDPN;
                foreach (var actionSettings in matchRule1.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.CDPN = phoneNumber;
            }


            NormalizationRule matchRule2 = GetMostMatchedRule(_rules, cdrInfo2);
            if(matchRule2 != null)
            {
                string phoneNumber = cdr.CDPN;
                foreach (var actionSettings in matchRule2.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.CDPN = phoneNumber;
            }
                
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {

        }



        static Vanrise.Rules.Entities.StructuredRules _rules = (new NormalizationRuleManager()).GetStructuredRules();

        NormalizationRuleActionBehavior GetActionBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<NormalizationRuleActionBehavior>(behaviorId);
        }

        
       

        public Vanrise.Rules.Entities.StructuredRules GetStructuredRules()
        {
            List<Vanrise.Rules.Entities.BaseRule> rules = new List<Vanrise.Rules.Entities.BaseRule>();//GEt rules from database
            rules.Add(new NormalizationRule { Criteria = new NormalizationRuleCriteria { SwitchIds= new List<int> { 2, 4 }, PhoneNumberPrefix = "961" } });
            var ruleSets = GetRuleSets();
            Vanrise.Rules.Business.RuleManager ruleManager = new Vanrise.Rules.Business.RuleManager();
            return ruleManager.StructureRules(rules, ruleSets);
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
