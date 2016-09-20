using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
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
                    var rules = GetFilteredRules(rule => rule.Settings.RuleType == ruleType && rule.BeginEffectiveTime <= DateTime.Now && (rule.EndEffectiveTime >= DateTime.Now || rule.EndEffectiveTime == null));
                    return new Vanrise.Rules.RuleTree(rules, GetRuleStructureBehaviors());
                });
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            NormalizationRuleAdjustNumberTarget cdrInfo_CGPN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_CGPN = GetMatchRule(cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_CGPN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    actionSettings.Execute(null, cdrInfo_CGPN);
                }
                cdr.CGPN = cdrInfo_CGPN.PhoneNumber;
            }


            NormalizationRuleAdjustNumberTarget cdrInfo_CDPN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_CDPN = GetMatchRule(cdrInfo_CDPN);
            if (matchRule_CDPN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_CDPN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    actionSettings.Execute(null, cdrInfo_CDPN);
                }
                cdr.CDPN = cdrInfo_CDPN.PhoneNumber;
            }
        }

        public void Normalize(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            NormalizationRuleAdjustNumberTarget cdrInfo_MSISDN = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_MSISDN = GetMatchRule(cdrInfo_MSISDN);
            if (matchRule_MSISDN != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_MSISDN.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    actionSettings.Execute(null, cdrInfo_MSISDN);
                }
                cdr.MSISDN = cdrInfo_MSISDN.PhoneNumber;
            }


            NormalizationRuleAdjustNumberTarget cdrInfo_Destination = new NormalizationRuleAdjustNumberTarget { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_Destination = GetMatchRule(cdrInfo_Destination);
            if (matchRule_Destination != null)
            {
                NormalizationRuleAdjustNumberSettings normalizationRuleAdjustNumberSettings = matchRule_Destination.Settings as NormalizationRuleAdjustNumberSettings;
                foreach (var actionSettings in normalizationRuleAdjustNumberSettings.Actions)
                {
                    actionSettings.Execute(null, cdrInfo_Destination);
                }
                cdr.Destination = cdrInfo_Destination.PhoneNumber;
            }

        }

        public void SetAreaCode(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr)
        {
            NormalizationRuleSetAreaTarget cdrInfo_CGPN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.CGPN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_CGPN = GetMatchRule(cdrInfo_CGPN);
            if (matchRule_CGPN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_CGPN.Settings as NormalizationRuleSetAreaSettings;
                normalizationRuleSetAreaSettings.Execute(null, cdrInfo_CGPN);
                cdr.CGPNAreaCode = cdrInfo_CGPN.AreaCode;
            }


            NormalizationRuleSetAreaTarget cdrInfo_CDPN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.CDPN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_CDPN = GetMatchRule(cdrInfo_CDPN);
            if (matchRule_CDPN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_CDPN.Settings as NormalizationRuleSetAreaSettings;
                normalizationRuleSetAreaSettings.Execute(null, cdrInfo_CDPN);
                cdr.CDPNAreaCode = cdrInfo_CDPN.AreaCode;
            }
        }

        public void SetAreaCode(Vanrise.Fzero.CDRImport.Entities.CDR cdr)
        {
            NormalizationRuleSetAreaTarget cdrInfo_MSISDN = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.MSISDN, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CGPN, TrunkId = cdr.InTrunkId };
            NormalizationRule matchRule_MSISDN = GetMatchRule(cdrInfo_MSISDN);
            if (matchRule_MSISDN != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_MSISDN.Settings as NormalizationRuleSetAreaSettings;
                normalizationRuleSetAreaSettings.Execute(null, cdrInfo_MSISDN);
                cdr.MSISDNAreaCode = cdrInfo_MSISDN.AreaCode;
            }


            NormalizationRuleSetAreaTarget cdrInfo_Destination = new NormalizationRuleSetAreaTarget { PhoneNumber = cdr.Destination, SwitchId = cdr.SwitchId, PhoneNumberType = NormalizationPhoneNumberType.CDPN, TrunkId = cdr.OutTrunkId };
            NormalizationRule matchRule_Destination = GetMatchRule(cdrInfo_Destination);
            if (matchRule_Destination != null)
            {
                NormalizationRuleSetAreaSettings normalizationRuleSetAreaSettings = matchRule_Destination.Settings as NormalizationRuleSetAreaSettings;
                normalizationRuleSetAreaSettings.Execute(null, cdrInfo_Destination);
                cdr.DestinationAreaCode = cdrInfo_Destination.AreaCode;
            }

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
            var normalizationRules = base.GetAllRules();

            Func<NormalizationRule, bool> filterExpression = (item) =>
                (
                    input.Query.PhoneNumberTypes == null ||
                    input.Query.PhoneNumberTypes.Contains(item.Criteria.PhoneNumberType)
                )
                &&
                (
                    input.Query.EffectiveDate == null ||
                    (
                        item.BeginEffectiveTime <= input.Query.EffectiveDate &&
                        (item.EndEffectiveTime == null || item.EndEffectiveTime >= input.Query.EffectiveDate)
                    )
                )
                &&
                (
                    input.Query.PhoneNumberPrefix == null ||
                    (item.Criteria.PhoneNumberPrefix != null && item.Criteria.PhoneNumberPrefix.Contains(input.Query.PhoneNumberPrefix))
                )
                &&
                (
                    input.Query.PhoneNumberLength == null ||
                    (item.Criteria.PhoneNumberLength != null && item.Criteria.PhoneNumberLength == input.Query.PhoneNumberLength)
                )
                &&
                (
                    input.Query.SwitchIds == null ||
                    (item.Criteria.SwitchIds != null && input.Query.SwitchIds.Where(n => item.Criteria.SwitchIds.Contains(n)).Count() > 0)
                )
                &&
                (
                    input.Query.TrunkIds == null ||
                    (item.Criteria.TrunkIds != null && input.Query.TrunkIds.Where(n => item.Criteria.TrunkIds.Contains(n)).Count() > 0)
                )
                &&
                (
                    input.Query.RuleTypes == null ||
                    input.Query.RuleTypes.Contains(item.Settings.RuleType)
                )
                &&
                (
                    input.Query.Description == null ||
                    (item.Description != null && item.Description.Contains(input.Query.Description))
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, normalizationRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        public IEnumerable<AdjustNumberActionConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<AdjustNumberActionConfig>(AdjustNumberActionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<SetAreaConfig> GetNormalizationRuleSetAreaSettingsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SetAreaConfig>(SetAreaConfig.EXTENSION_TYPE);
        }

        public override NormalizationRuleDetail MapToDetails(NormalizationRule rule)
        {
            return NormalizationRuleDetailMapper(rule);
        }

        #region Private Members

        private NormalizationRuleDetail NormalizationRuleDetailMapper(NormalizationRule rule)
        {
            NormalizationRuleDetail detail = new NormalizationRuleDetail();
            detail.Entity = rule;

            detail.PhoneNumberTypeDescription = rule.Criteria.PhoneNumberType.ToString();

            DescriptionAttribute descriptionAttribute = Vanrise.Common.Utilities.GetEnumAttribute<NormalizationRuleType, DescriptionAttribute>(rule.Settings.RuleType);
            detail.RuleTypeDescription = descriptionAttribute.Description;

            if (rule.Criteria.SwitchIds != null)
            {
                detail.SwitchCount = rule.Criteria.SwitchIds.Count;

                SwitchManager switchManager = new SwitchManager();
                IEnumerable<SwitchInfo> switches = switchManager.GetSwitchesByIds(rule.Criteria.SwitchIds);
                List<string> switchNames = switches.Select(x => x.Name).ToList();

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
                List<string> trunkNames = trunks.Select(x => x.Name).ToList();

                detail.TrunkNames = string.Join<string>(", ", trunkNames);
            }
            else
            {
                detail.TrunkCount = 0;
            }

            detail.Descriptions = (rule.Settings != null) ? rule.Settings.GetDescriptions() : null;

            return detail;
        }

        #endregion
    }
}
