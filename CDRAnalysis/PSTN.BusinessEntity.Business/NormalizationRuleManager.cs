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


            CDRToNormalizeInfo cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };


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



            CDRToNormalizeInfo cdrInfo_CDPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.InTrunkId };



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

            //CDRToNormalizeInfo cdrInfo_MSISDN = new CDRToNormalizeInfo { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
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


            //CDRToNormalizeInfo cdrInfo_Destination = new CDRToNormalizeInfo { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
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

        static Vanrise.Rules.RuleTree _rules = (new NormalizationRuleManager()).GetStructuredRules();

        NormalizationRuleActionBehavior GetActionBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<NormalizationRuleActionBehavior>(behaviorId);
        }

        public Vanrise.Rules.RuleTree GetStructuredRules()
        {            
            List<Vanrise.Rules.BaseRule> rules = new List<Vanrise.Rules.BaseRule>();//GEt rules from database
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



            var ruleStructureBehaviors = GetRuleStructureBehaviors();
            return new Vanrise.Rules.RuleTree(rules, ruleStructureBehaviors);
        }

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new Entities.Normalization.StructureRuleBehaviors.RuleBehaviorByNumberType());
            ruleStructureBehaviors.Add(new Entities.Normalization.StructureRuleBehaviors.RuleBehaviorBySwitch());
            ruleStructureBehaviors.Add(new Entities.Normalization.StructureRuleBehaviors.RuleBehaviorByTrunk());
            ruleStructureBehaviors.Add(new Entities.Normalization.StructureRuleBehaviors.RuleBehaviorByNumberPrefix());
            ruleStructureBehaviors.Add(new Entities.Normalization.StructureRuleBehaviors.RuleBehaviorByNumberLength());
            return ruleStructureBehaviors;
        }

        public NormalizationRule GetMostMatchedRule(Vanrise.Rules.RuleTree ruleTree, CDRToNormalizeInfo cdr)
        {
            return ruleTree.GetMatchRule(cdr) as NormalizationRule;
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
