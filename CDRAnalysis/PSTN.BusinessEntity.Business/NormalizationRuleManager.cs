using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.Entities.Normalization.Actions;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager
    {
        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {

            CDRToNormalizeInfo cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId.Value };
            NormalizationRule matchRule_CGPN = GetMostMatchedRule(_rules, cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                string phoneNumber = cdr.CDPN;
                foreach (var actionSettings in matchRule_CGPN.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.CDPN = phoneNumber;
            }


            CDRToNormalizeInfo cdrInfo_CDPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId.Value };
            NormalizationRule matchRule_CDPN = GetMostMatchedRule(_rules, cdrInfo_CDPN);
            if (matchRule_CDPN != null)
            {
                string phoneNumber = cdr.CDPN;
                foreach (var actionSettings in matchRule_CDPN.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.CDPN = phoneNumber;
            }

        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            CDRToNormalizeInfo cdrInfo_MSISDN = new CDRToNormalizeInfo { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId.Value };
            NormalizationRule matchRule_MSISDN = GetMostMatchedRule(_rules, cdrInfo_MSISDN);
            if (matchRule_MSISDN != null)
            {
                string phoneNumber = cdr.MSISDN;
                foreach (var actionSettings in matchRule_MSISDN.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.MSISDN = phoneNumber;
            }


            CDRToNormalizeInfo cdrInfo_Destination = new CDRToNormalizeInfo { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId.Value };
            NormalizationRule matchRule_Destination = GetMostMatchedRule(_rules, cdrInfo_Destination);
            if (matchRule_Destination != null)
            {
                string phoneNumber = cdr.Destination;
                foreach (var actionSettings in matchRule_Destination.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.Destination = phoneNumber;
            }

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
            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberLength = 8, PhoneNumberPrefix = "177", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "0" } } }
            });
            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberLength = 8, PhoneNumberPrefix = "181", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 143 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "0" } } }
            });

            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberLength = 10, PhoneNumberPrefix = "727", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 144 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "0" } } }
            });

            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberLength = 7, PhoneNumberPrefix = "765", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 145 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "01" } } }
            });

            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, PhoneNumberLength = 10, PhoneNumberPrefix = "999", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 147 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new SubstringActionSettings() { BehaviorId = 2, StartIndex = 0, Length = 3 }, new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "01" } } }
            });

            rules.Add(new NormalizationRule
            {
                Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, PhoneNumberLength = 7, PhoneNumberPrefix = "778", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 148 } },
                Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "01" } } }
            });



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

        public Vanrise.Entities.IDataRetrievalResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredNormalizationRules(input));
        }
    }
}
