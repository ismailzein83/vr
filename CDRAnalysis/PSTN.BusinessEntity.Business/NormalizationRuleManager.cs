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
            CDRToNormalizeInfo cdrInfo_CGPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
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


            CDRToNormalizeInfo cdrInfo_CDPN = new CDRToNormalizeInfo { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
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

            CDRToNormalizeInfo cdrInfo_MSISDN = new CDRToNormalizeInfo { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
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


            CDRToNormalizeInfo cdrInfo_Destination = new CDRToNormalizeInfo { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
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

        NormalizationRuleActionBehavior GetActionBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<NormalizationRuleActionBehavior>(behaviorId);
        }

        public Vanrise.Rules.RuleTree GetStructuredRules()
        {            
            List<Vanrise.Rules.BaseRule> rules = new List<Vanrise.Rules.BaseRule>();//GEt rules from database
            rules.AddRange(GetEffectiveRules());

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
            BigResult<NormalizationRule> normalizationRules = dataManager.GetFilteredNormalizationRules(input);

            BigResult<NormalizationRuleDetail> normalizationRuleDetails = new BigResult<NormalizationRuleDetail>();
            normalizationRuleDetails.ResultKey = normalizationRules.ResultKey;
            normalizationRuleDetails.TotalCount = normalizationRules.TotalCount;

            List<NormalizationRuleDetail> data = new List<NormalizationRuleDetail>();

            foreach (NormalizationRule normalizationRule in normalizationRules.Data)
            {
                NormalizationRuleDetail normalizationRuleDetail = GetNormalizationRuleDetail(normalizationRule);
                data.Add(normalizationRuleDetail);
            }

            normalizationRuleDetails.Data = data;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, normalizationRuleDetails);
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

                NormalizationRule normalizationRule = dataManager.GetNormalizationRuleByID(normalizationRuleId);
                insertOperationOutput.InsertedObject = GetNormalizationRuleDetail(normalizationRule);
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

                NormalizationRule normalizationRule = dataManager.GetNormalizationRuleByID(normalizationRuleObj.NormalizationRuleId);
                updateOperationOutput.UpdatedObject = GetNormalizationRuleDetail(normalizationRule);
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

        private NormalizationRuleDetail GetNormalizationRuleDetail(NormalizationRule normalizationRule)
        {
            NormalizationRuleDetail normalizationRuleDetail = new NormalizationRuleDetail();

            normalizationRuleDetail.NormalizationRuleId = normalizationRule.NormalizationRuleId;
            normalizationRuleDetail.BeginEffectiveDate = normalizationRule.BeginEffectiveDate;
            normalizationRuleDetail.EndEffectiveDate = normalizationRule.EndEffectiveDate;

            if (normalizationRule.Criteria.SwitchIds != null)
            {
                normalizationRuleDetail.SwitchCount = normalizationRule.Criteria.SwitchIds.Count;

                SwitchManager switchManager = new SwitchManager();
                List<SwitchInfo> switches = switchManager.GetSwitchesByIds(normalizationRule.Criteria.SwitchIds);
                List<string> switchNames = GetSwitchNames(switches);
                normalizationRuleDetail.SwitchNames = string.Join<string>(", ", switchNames);
            }
            else
            {
                normalizationRuleDetail.SwitchCount = 0;
            }

            if (normalizationRule.Criteria.TrunkIds != null)
            {
                normalizationRuleDetail.TrunkCount = normalizationRule.Criteria.TrunkIds.Count;

                SwitchTrunkManager trunkManager = new SwitchTrunkManager();
                List<SwitchTrunkInfo> trunks = trunkManager.GetSwitchTrunksByIds(normalizationRule.Criteria.TrunkIds);
                List<string> trunkNames = GetTrunkNames(trunks);
                normalizationRuleDetail.TrunkNames = string.Join<string>(", ", trunkNames);
            }
            else
            {
                normalizationRuleDetail.TrunkCount = 0;
            }

            normalizationRuleDetail.PhoneNumberType = normalizationRule.Criteria.PhoneNumberType;
            normalizationRuleDetail.PhoneNumberLength = normalizationRule.Criteria.PhoneNumberLength;
            normalizationRuleDetail.PhoneNumberPrefix = normalizationRule.Criteria.PhoneNumberPrefix;

            if (normalizationRule.Settings.Actions != null)
            {
                normalizationRuleDetail.ActionDescriptions = new List<string>();

                foreach (var action in normalizationRule.Settings.Actions)
                {   
                    normalizationRuleDetail.ActionDescriptions.Add(action.GetDescription());
                }
            }

            return normalizationRuleDetail;
        }

        private List<string> GetSwitchNames(List<SwitchInfo> switches)
        {
            List<string> switchNames = new List<string>();

            foreach (SwitchInfo switchInfo in switches)
            {
                switchNames.Add(switchInfo.Name);
            }

            return switchNames;
        }

        private List<string> GetTrunkNames(List<SwitchTrunkInfo> trunks)
        {
            List<string> trunkNames = new List<string>();

            foreach (SwitchTrunkInfo trunk in trunks)
            {
                trunkNames.Add(trunk.Name);
            }

            return trunkNames;
        }

    }
}
