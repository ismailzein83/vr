using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class NormalizationRuleManager : Vanrise.Rules.RuleManager<NormalizationRule>
    { 
        public NormalizationRule GetMatchRule(NormalizationRuleTarget target)
        {
            var ruleTree = GetRuleTree(target.RuleType);
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as NormalizationRule;
        }

        Vanrise.Rules.RuleTree GetRuleTree(NormalizationRuleType ruleType)
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_{0}", ruleType),
                () =>
                {
                    var rules = GetFilteredRules(rule => rule.Settings.RuleType == ruleType);
                    return new Vanrise.Rules.RuleTree(rules, GetRuleStructureBehaviors());
                });
        }

        //public void FindSwitch_ApplyTimeOffset(Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr, )
        //{
          


        //  foreach (var i in cdrs)
        //  {

        //  }
        //}

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

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberType());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorBySwitch());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByTrunk());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberPrefix());
            ruleStructureBehaviors.Add(new Rules.StructureRulesBehaviors.RuleBehaviorByNumberLength());
            return ruleStructureBehaviors;
        }       

        public Vanrise.Entities.IDataRetrievalResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            //var allNormalizationRules = GetCachedNormalizationRules();

            Func<NormalizationRule, bool> filterResult = (item) =>
                        (
                            input.Query.PhoneNumberTypes == null ||
                            input.Query.PhoneNumberTypes.Contains(item.Criteria.PhoneNumberType)
                        )
                        &&
                        (
                            input.Query.EffectiveDate == null ||
                            (
                                item.BeginEffectiveDate <= input.Query.EffectiveDate &&
                                (item.EndEffectiveDate == null || item.EndEffectiveDate >= input.Query.EffectiveDate)
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
                            (item.Criteria.SwitchIds != null && ListContains(input.Query.SwitchIds, item.Criteria.SwitchIds))
                        )
                        &&
                        (
                            input.Query.TrunkIds == null ||
                            (item.Criteria.TrunkIds != null && ListContains(input.Query.TrunkIds, item.Criteria.TrunkIds))
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

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, GetFilteredRules(filterResult).ToBigResult(input, filterResult, NormalizationRuleDetailMapper));

            //return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allNormalizationRules.ToBigResult(input, filterResult, NormalizationRuleDetailMapper));
        }

        public NormalizationRule GetNormalizationRuleById(int normalizationRuleId)
        {
            var allNormalizationRules = GetCachedNormalizationRules();
            return allNormalizationRules.Find(x => x.NormalizationRuleId == normalizationRuleId);
        }

        public List<Vanrise.Entities.TemplateConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.AdjustNumberActionConfigType);
        }

        public List<Vanrise.Entities.TemplateConfig> GetNormalizationRuleSetAreaSettingsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SetAreaConfigType);
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
                insertOperationOutput.InsertedObject = NormalizationRuleDetailMapper(normalizationRuleObj);
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
                updateOperationOutput.UpdatedObject = NormalizationRuleDetailMapper(normalizationRuleObj);
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

        #region Private Members

        private List<NormalizationRuleDetail> GetNormalizationRuleDetails(IEnumerable<NormalizationRule> normalizationRules)
        {
            List<NormalizationRuleDetail> normalizationRuleDetails = new List<NormalizationRuleDetail>();

            foreach (NormalizationRule normalizationRule in normalizationRules)
            {
                normalizationRuleDetails.Add(NormalizationRuleDetailMapper(normalizationRule));
            }

            return normalizationRuleDetails;
        }

        private NormalizationRuleDetail NormalizationRuleDetailMapper(NormalizationRule normalizationRule)
        {
            NormalizationRuleDetail normalizationRuleDetail = new NormalizationRuleDetail();

            normalizationRuleDetail.NormalizationRuleId = normalizationRule.NormalizationRuleId;
            normalizationRuleDetail.BeginEffectiveDate = normalizationRule.BeginEffectiveDate;
            normalizationRuleDetail.EndEffectiveDate = normalizationRule.EndEffectiveDate;

            if (normalizationRule.Criteria.SwitchIds != null)
            {
                normalizationRuleDetail.SwitchCount = normalizationRule.Criteria.SwitchIds.Count;

                SwitchManager switchManager = new SwitchManager();
                IEnumerable<SwitchInfo> switches = switchManager.GetSwitchesByIds(normalizationRule.Criteria.SwitchIds);
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

                TrunkManager trunkManager = new TrunkManager();
                List<TrunkInfo> trunks = trunkManager.GetTrunksByIds(normalizationRule.Criteria.TrunkIds);
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

            normalizationRuleDetail.Descriptions = (normalizationRule.Settings != null) ? normalizationRule.Settings.GetDescriptions() : null;

            normalizationRuleDetail.Description = normalizationRule.Description;

            return normalizationRuleDetail;
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            INormalizationRuleDataManager _dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreNormalizationRulesUpdated(ref _updateHandle);
            }
        }

        private List<NormalizationRule> GetCachedNormalizationRules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetNormalizationRules",
               () =>
               {
                   INormalizationRuleDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<INormalizationRuleDataManager>();
                   return dataManager.GetNormalizationRules();
               });
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

        private List<string> GetTrunkNames(List<TrunkInfo> trunks)
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
