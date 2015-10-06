using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.Entities.Normalization.Actions;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager
    {
        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {


            //CDRToNormalizeInfo cdrInfo_CGPN = new CDRToNormalizeInfo();

            //if (cdr.SwitchID.HasValue && cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId.Value };

            //else if (!cdr.SwitchID.HasValue && cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId.Value };

            //else if (cdr.SwitchID.HasValue && !cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CGPN };

            //else if (!cdr.SwitchID.HasValue && !cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, PhoneNumberType = NormalizationPhoneNumberType.CGPN };


            //NormalizationRule matchRule_CGPN = GetMostMatchedRule(_rules, cdrInfo_CGPN);
            //if (matchRule_CGPN != null)
            //{
            //    string phoneNumber = cdr.CDPN;
            //    foreach (var actionSettings in matchRule_CGPN.Settings.Actions)
            //    {
            //        var behavior = GetActionBehavior(actionSettings.BehaviorId);
            //        behavior.Execute(actionSettings, ref phoneNumber);
            //    }
            //    cdr.CDPN = phoneNumber;
            //}



            //CDRToNormalizeInfo cdrInfo_CDPN = new CDRToNormalizeInfo();

            //if (cdr.SwitchID.HasValue && cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.InTrunkId.Value };

            //else if (!cdr.SwitchID.HasValue && cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.InTrunkId.Value };

            //else if (cdr.SwitchID.HasValue && !cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID.Value, PhoneNumberType = NormalizationPhoneNumberType.CDPN };

            //else if (!cdr.SwitchID.HasValue && !cdr.InTrunkId.HasValue)
            //    cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, PhoneNumberType = NormalizationPhoneNumberType.CDPN };


            //NormalizationRule matchRule_CDPN = GetMostMatchedRule(_rules, cdrInfo_CDPN);
            //if (matchRule_CDPN != null)
            //{
            //    string phoneNumber = cdr.CDPN;
            //    foreach (var actionSettings in matchRule_CDPN.Settings.Actions)
            //    {
            //        var behavior = GetActionBehavior(actionSettings.BehaviorId);
            //        behavior.Execute(actionSettings, ref phoneNumber);
            //    }
            //    cdr.CDPN = phoneNumber;
            //}

        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            //int switchId = 0;
            //if (cdr.SwitchID.HasValue)
            //    switchId = cdr.SwitchID.Value;


            //int inTrunkId = 0;
            //if (cdr.InTrunkId.HasValue)
            //    inTrunkId = cdr.InTrunkId.Value;

            //int outTrunkId = 0;
            //if (cdr.OutTrunkId.HasValue)
            //    outTrunkId = cdr.OutTrunkId.Value;


            //CDRToNormalizeInfo cdrInfo_MSISDN = new CDRToNormalizeInfo { PhoneNumber = cdr.MSISDN, SwitchId = switchId, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = inTrunkId };
            //NormalizationRule matchRule_MSISDN = GetMostMatchedRule(_rules, cdrInfo_MSISDN);
            //if (matchRule_MSISDN != null)
            //{
            //    string phoneNumber = cdr.MSISDN;
            //    foreach (var actionSettings in matchRule_MSISDN.Settings.Actions)
            //    {
            //        var behavior = GetActionBehavior(actionSettings.BehaviorId);
            //        behavior.Execute(actionSettings, ref phoneNumber);
            //    }
            //    cdr.MSISDN = phoneNumber;
            //}


            //CDRToNormalizeInfo cdrInfo_Destination = new CDRToNormalizeInfo { PhoneNumber = cdr.Destination, SwitchId = switchId, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = outTrunkId };
            //NormalizationRule matchRule_Destination = GetMostMatchedRule(_rules, cdrInfo_Destination);
            //if (matchRule_Destination != null)
            //{
            //    string phoneNumber = cdr.Destination;
            //    foreach (var actionSettings in matchRule_Destination.Settings.Actions)
            //    {
            //        var behavior = GetActionBehavior(actionSettings.BehaviorId);
            //        behavior.Execute(actionSettings, ref phoneNumber);
            //    }
            //    cdr.Destination = phoneNumber;
            //}

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

        public NormalizationRule GetNormalizationRuleByID(int normalizationRuleId)
        {
            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
            return dataManager.GetNormalizationRuleByID(normalizationRuleId);
        }

        public List<Vanrise.Entities.TemplateConfig> GetNormalizationRuleActionBehaviorTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.NormalizationRuleActionBehaviorConfigType);
        
        }

        public InsertOperationOutput<NormalizationRuleDetail> AddNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            InsertOperationOutput<NormalizationRuleDetail> insertOperationOutput = new InsertOperationOutput<NormalizationRuleDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int normalizationRuleId = -1;

            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
            bool inserted = dataManager.AddNormalizationRule(normalizationRuleObj, out normalizationRuleId);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                normalizationRuleObj.NormalizationRuleId = normalizationRuleId;
                
                NormalizationRuleDetail detail = new NormalizationRuleDetail();
                detail.NormalizationRuleId = normalizationRuleId;
                //detail.BeginEffectiveDate = 
                //insertOperationOutput.InsertedObject = normalizationRuleDetail;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
    }
}
