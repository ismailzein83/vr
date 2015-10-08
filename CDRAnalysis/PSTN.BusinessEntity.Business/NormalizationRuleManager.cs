using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.Entities.Normalization.Actions;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using System.Linq;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager
    {
        static Vanrise.Rules.RuleTree _rules ;
        static NormalizationRuleManager()
        {
            _rules = (new NormalizationRuleManager()).GetStructuredRules();
        }
        public List<NormalizationRule> GetEffectiveRules()
        {
            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
            return dataManager.GetEffective();
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            CDRToNormalizeInfo cdrInfo_CGPN = CreateCGPN_CDR(cdr);
            ApplyRuleforCGPN(cdr, cdrInfo_CGPN);


            CDRToNormalizeInfo cdrInfo_CDPN = CreateCDPN_CDR(cdr);
            ApplyRuleforCDPN(cdr, cdrInfo_CDPN);
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

        private static CDRToNormalizeInfo CreateCGPN_CDR(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            CDRToNormalizeInfo cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            return cdrInfo_CGPN;
        }

        private static CDRToNormalizeInfo CreateCDPN_CDR(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            CDRToNormalizeInfo cdrInfo_CDPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            return cdrInfo_CDPN;
        }

        private void ApplyRuleforCDPN(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr, CDRToNormalizeInfo cdrInfo_CDPN)
        {
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

        private void ApplyRuleforCGPN(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr, CDRToNormalizeInfo cdrInfo_CGPN)
        {
            NormalizationRule matchRule_CGPN = GetMostMatchedRule(_rules, cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                string phoneNumber = cdr.CGPN;
                foreach (var actionSettings in matchRule_CGPN.Settings.Actions)
                {
                    var behavior = GetActionBehavior(actionSettings.BehaviorId);
                    behavior.Execute(actionSettings, ref phoneNumber);
                }
                cdr.CGPN = phoneNumber;
            }
        }

        

        NormalizationRuleActionBehavior GetActionBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<NormalizationRuleActionBehavior>(behaviorId);
        }

        public Vanrise.Rules.RuleTree GetStructuredRules()
        {            
            List<Vanrise.Rules.BaseRule> rules = new List<Vanrise.Rules.BaseRule>();//GEt rules from database
            rules.AddRange(GetEffectiveRules());


            //foreach

            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 }, PhoneNumberLength = null, PhoneNumberPrefix = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "xx" } } }
            //});

            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberLength = 8, PhoneNumberPrefix = "177", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 } },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "xxxx" } } }
            //});


            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, PhoneNumberPrefix = "177", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 }, PhoneNumberLength = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "xxx" } } }
            //});

            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CGPN, SwitchIds = new List<int>() { 3 }, PhoneNumberLength = null, PhoneNumberPrefix = null, TrunkIds = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "x" } } }
            //});
          



            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, SwitchIds = new List<int>() { 3 }, PhoneNumberLength = null, PhoneNumberPrefix = null, TrunkIds = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "y" } } }
            //});
            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 }, PhoneNumberLength = null, PhoneNumberPrefix = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "yy" } } }
            //});

            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, PhoneNumberPrefix = "99", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 }, PhoneNumberLength = null },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "yyy" } } }
            //});

            //rules.Add(new NormalizationRule
            //{
            //    Criteria = new NormalizationRuleCriteria { PhoneNumberType = NormalizationPhoneNumberType.CDPN, PhoneNumberLength = 6, PhoneNumberPrefix = "99", SwitchIds = new List<int>() { 3 }, TrunkIds = new List<int>() { 142 } },
            //    Settings = new NormalizationRuleSettings() { Actions = new List<NormalizationRuleActionSettings>() { new AddPrefixActionSettings() { BehaviorId = 1, Prefix = "yyyy" } } }
            //});


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

            BigResult<NormalizationRuleDetail> bigResult = dataManager.GetFilteredNormalizationRules(input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
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
                insertOperationOutput.InsertedObject = dataManager.GetNormalizationRuleDetailByID(normalizationRuleId);
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<NormalizationRuleDetail> UpdateNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            UpdateOperationOutput<NormalizationRuleDetail> updateOperationOutput = new UpdateOperationOutput<NormalizationRuleDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();

            bool updated = dataManager.UpdateNormalizationRule(normalizationRuleObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = dataManager.GetNormalizationRuleDetailByID(normalizationRuleObj.NormalizationRuleId);
            }

            return updateOperationOutput;
        }
        
        public DeleteOperationOutput<object> DeleteNormalizationRule(int normalizationRuleId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();

            bool deleted = dataManager.DeleteNormalizationRule(normalizationRuleId);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
