using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Rules;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager : Vanrise.Rules.RuleManager<NormalizationRule, NormalizationRuleDetail>
    {
        public NormalizationRule GetMatchRule(NormalizationRuleTarget target)
        {
            var ruleTree = GetRuleTree(target.RuleType);
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as NormalizationRule;
        }

        RuleTree GetRuleTree(NormalizationRuleType ruleType)
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_{0}", ruleType),
                () =>
                {
                    var details = GetFilteredRules(rule => rule.Entity.Settings.RuleType == ruleType);
                    IEnumerable<BaseRule> rules = details.Select(x => x.Entity);
                    return new Vanrise.Rules.RuleTree(rules, GetRuleStructureBehaviors());
                });
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            NormalizationRuleAdjustNumberTarget cdrInfo_CGPN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_CGPN = GetMatchRule(cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_CGPN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    var behavior = GetAdjustNumberActionBehavior(actionSettings.ConfigId);
                    behavior.Execute(actionSettings, cdrInfo_CGPN);
                }
                cdr.CGPN = cdrInfo_CGPN.PhoneNumber;
            }


            NormalizationRuleAdjustNumberTarget cdrInfo_CDPN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_CDPN = GetMatchRule(cdrInfo_CDPN);
            if (matchRule_CDPN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_CDPN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    var behavior = GetAdjustNumberActionBehavior(actionSettings.ConfigId);
                    behavior.Execute(actionSettings, cdrInfo_CDPN);
                }
                cdr.CDPN = cdrInfo_CDPN.PhoneNumber;
            }
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            NormalizationRuleAdjustNumberTarget cdrInfo_MSISDN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_MSISDN = GetMatchRule(cdrInfo_MSISDN);
            if (matchRule_MSISDN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_MSISDN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    var behavior = GetAdjustNumberActionBehavior(actionSettings.ConfigId);
                    behavior.Execute(actionSettings, cdrInfo_MSISDN);
                }
                cdr.MSISDN = cdrInfo_MSISDN.PhoneNumber;
            }


            NormalizationRuleAdjustNumberTarget cdrInfo_Destination = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_Destination = GetMatchRule(cdrInfo_Destination);
            if (matchRule_Destination != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_Destination.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    var behavior = GetAdjustNumberActionBehavior(actionSettings.ConfigId);
                    behavior.Execute(actionSettings, cdrInfo_Destination);
                }
                cdr.Destination = cdrInfo_Destination.PhoneNumber;
            }

        }

        public void SetAreaCode(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            NormalizationRuleSetAreaTarget cdrInfo_CGPN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_CGPN = GetMatchRule(cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_CGPN.Settings as NormalizationRuleSetAreaSettings;
                var behavior = GetSetAreaBehavior(normalizationRuleSetAreaSettings.ConfigId);
                behavior.Execute(normalizationRuleSetAreaSettings, cdrInfo_CGPN);
                cdr.CGPNAreaCode = cdrInfo_CGPN.AreaCode;
            }


            NormalizationRuleSetAreaTarget cdrInfo_CDPN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_CDPN = GetMatchRule(cdrInfo_CDPN);
            if (matchRule_CDPN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_CDPN.Settings as NormalizationRuleSetAreaSettings;
                var behavior = GetSetAreaBehavior(normalizationRuleSetAreaSettings.ConfigId);
                behavior.Execute(normalizationRuleSetAreaSettings, cdrInfo_CDPN);
                cdr.CDPNAreaCode = cdrInfo_CDPN.AreaCode;
            }
        }

        public void SetAreaCode(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            NormalizationRuleSetAreaTarget cdrInfo_MSISDN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_MSISDN = GetMatchRule(cdrInfo_MSISDN);
            if (matchRule_MSISDN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_MSISDN.Settings as NormalizationRuleSetAreaSettings;
                var behavior = GetSetAreaBehavior(normalizationRuleSetAreaSettings.ConfigId);
                behavior.Execute(normalizationRuleSetAreaSettings, cdrInfo_MSISDN);
                cdr.MSISDNAreaCode = cdrInfo_MSISDN.AreaCode;
            }


            NormalizationRuleSetAreaTarget cdrInfo_Destination = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchID, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_Destination = GetMatchRule(cdrInfo_Destination);
            if (matchRule_Destination != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_Destination.Settings as NormalizationRuleSetAreaSettings;
                var behavior = GetSetAreaBehavior(normalizationRuleSetAreaSettings.ConfigId);
                behavior.Execute(normalizationRuleSetAreaSettings, cdrInfo_Destination);
                cdr.DestinationAreaCode = cdrInfo_Destination.AreaCode;
            }

        }

        Entities.NormalizationRuleAdjustNumberActionBehavior GetAdjustNumberActionBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<Entities.NormalizationRuleAdjustNumberActionBehavior>(behaviorId);
        }

        Entities.NormalizationRuleSetAreaBehavior GetSetAreaBehavior(int behaviorId)
        {
            Vanrise.Common.TemplateConfigManager templateConfigManager = new Vanrise.Common.TemplateConfigManager();
            return templateConfigManager.GetBehavior<Entities.NormalizationRuleSetAreaBehavior>(behaviorId);
        }

        IEnumerable<BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberType());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorBySwitch());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByTrunk());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberPrefix());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberLength());
            return ruleStructureBehaviors;
        }       

        public IDataRetrievalResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            Func<NormalizationRuleDetail, bool> filterExpression = (item) =>
                (
                    input.Query.PhoneNumberTypes == null ||
                    input.Query.PhoneNumberTypes.Contains(item.Entity.Criteria.PhoneNumberType)
                )
                &&
                (
                    input.Query.EffectiveDate == null ||
                    (
                        item.Entity.BeginEffectiveTime <= input.Query.EffectiveDate &&
                        (item.Entity.EndEffectiveTime == null || item.Entity.EndEffectiveTime >= input.Query.EffectiveDate)
                    )
                )
                &&
                (
                    input.Query.PhoneNumberPrefix == null ||
                    (item.Entity.Criteria.PhoneNumberPrefix != null && item.Entity.Criteria.PhoneNumberPrefix.Contains(input.Query.PhoneNumberPrefix))
                )
                &&
                (
                    input.Query.PhoneNumberLength == null ||
                    (item.Entity.Criteria.PhoneNumberLength != null && item.Entity.Criteria.PhoneNumberLength == input.Query.PhoneNumberLength)
                )
                &&
                (
                    input.Query.SwitchIds == null ||
                    (item.Entity.Criteria.SwitchIds != null && ListContains(input.Query.SwitchIds, item.Entity.Criteria.SwitchIds))
                )
                &&
                (
                    input.Query.TrunkIds == null ||
                    (item.Entity.Criteria.TrunkIds != null && ListContains(input.Query.TrunkIds, item.Entity.Criteria.TrunkIds))
                )
                &&
                (
                    input.Query.RuleTypes == null ||
                    input.Query.RuleTypes.Contains(item.Entity.Settings.RuleType)
                )
                &&
                (
                    input.Query.Description == null ||
                    (item.Entity.Description != null && item.Entity.Description.Contains(input.Query.Description))
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, GetFilteredRules(filterExpression).ToBigResult(input, filterExpression));
        }

        public NormalizationRuleDetail GetNormalizationRuleById(int normalizationRuleId)
        {
            NormalizationRuleDetail detail = GetRule(normalizationRuleId);
            return detail;
        }

        public List<TemplateConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.AdjustNumberActionConfigType);
        }

        public List<TemplateConfig> GetNormalizationRuleSetAreaSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SetAreaConfigType);
        }

        public InsertOperationOutput<NormalizationRuleDetail> AddNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            return AddRule(normalizationRuleObj);
        }

        public UpdateOperationOutput<NormalizationRuleDetail> UpdateNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            return UpdateRule(normalizationRuleObj);
        }

        public DeleteOperationOutput<NormalizationRuleDetail> DeleteNormalizationRule(int ruleId)
        {
            return DeleteRule(ruleId);
        }

        protected override NormalizationRuleDetail MapToDetails(NormalizationRule rule)
        {
            return NormalizationRuleDetailMapper(rule);
        }

        #region Private Members

        private NormalizationRuleDetail NormalizationRuleDetailMapper(NormalizationRule rule)
        {
            NormalizationRuleDetail detail = new NormalizationRuleDetail();
            detail.Entity = rule;


            if (rule.Criteria.SwitchIds != null)
            {
                detail.SwitchCount = rule.Criteria.SwitchIds.Count;

                SwitchManager switchManager = new SwitchManager();
                IEnumerable<SwitchInfo> switches = switchManager.GetSwitchesByIds(rule.Criteria.SwitchIds);
                List<string> switchNames = GetSwitchNames(switches);

                detail.SwitchNames = string.Join<string>(", ", switchNames);
            }
            else
            {
                detail.SwitchCount = 0;
            }

            if (rule.Criteria.TrunkIds != null)
            {
                detail.TrunkCount = rule.Criteria.TrunkIds.Count;

                TrunkManager trunkManager = new TrunkManager();
                IEnumerable<TrunkInfo> trunks = trunkManager.GetTrunksByIds(rule.Criteria.TrunkIds);
                List<string> trunkNames = GetTrunkNames(trunks);

                detail.TrunkNames = string.Join<string>(", ", trunkNames);
            }
            else
            {
                detail.TrunkCount = 0;
            }

            detail.Descriptions = (rule.Settings != null) ? rule.Settings.GetDescriptions() : null;

            return detail;
        }

        private List<string> GetSwitchNames(IEnumerable<SwitchInfo> switches)
        {
            List<string> switchNames = new List<string>();

            foreach (SwitchInfo switchInfo in switches)
            {
                switchNames.Add(switchInfo.Name);
            }

            return switchNames;
        }

        private List<string> GetTrunkNames(IEnumerable<TrunkInfo> trunks)
        {
            List<string> trunkNames = new List<string>();

            foreach (TrunkInfo trunk in trunks)
            {
                trunkNames.Add(trunk.Name);
            }

            return trunkNames;
        }

        private bool ListContains(List<int> filterIds, List<int> itemIds)
        {
            foreach (int itemId in itemIds)
            {
                if (filterIds.Contains(itemId))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
