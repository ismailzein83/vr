using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Common;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;

namespace Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators
{
    public class StandardPolicyEvaluator : VoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("B0B5BC1F-E899-4AE5-AEFB-4FCD5D1BA140"); } }

        public Guid? RateValueRuleDefinitionId { get; set; }
        public Guid? RateTypeRuleDefinitionId { get; set; }
        public Guid? ExtraChargeRuleDefinitionId { get; set; }
        public Guid? TariffRuleDefinitionId { get; set; }

        public override void ExportRates(IVoiceChargingPolicyEvaluatorExportRatesContext context)
        {
            BusinessEntityFieldTypeFilter accountFieldFilter = new BusinessEntityFieldTypeFilter() { BusinessEntityIds = new List<object>() { context.AccountId } };
            KeyValuePair<string, BusinessEntityFieldTypeFilter> accountFilter = new KeyValuePair<string, BusinessEntityFieldTypeFilter>("Account", accountFieldFilter);

            BusinessEntityFieldTypeFilter serviceTypeFieldFilter = new BusinessEntityFieldTypeFilter() { BusinessEntityIds = new List<object>() { context.ServiceTypeId.ToString() } };
            KeyValuePair<string, BusinessEntityFieldTypeFilter> serviceTypeFilter = new KeyValuePair<string, BusinessEntityFieldTypeFilter>("ServiceType", serviceTypeFieldFilter);

            BusinessEntityFieldTypeFilter chargingPolicyFieldFilter = new BusinessEntityFieldTypeFilter() { BusinessEntityIds = new List<object>() { context.ChargingPolicyId } };
            KeyValuePair<string, BusinessEntityFieldTypeFilter> chargingPolicyFilter = new KeyValuePair<string, BusinessEntityFieldTypeFilter>("ChargingPolicy", chargingPolicyFieldFilter);

            Guid rateValueRuleDefinitionId = GetRateValueRuleDefinitionId(context.ServiceTypeId);
            GenericRuleQuery rateValueRuleGenericRuleQuery = new GenericRuleQuery()
            {
                EffectiveDate = context.EffectiveDate,
                RuleDefinitionId = rateValueRuleDefinitionId,
                CriteriaFieldValues = new Dictionary<string, object>() { }
            };
            rateValueRuleGenericRuleQuery.CriteriaFieldValues.Add(accountFilter.Key, accountFilter.Value);
            rateValueRuleGenericRuleQuery.CriteriaFieldValues.Add(chargingPolicyFilter.Key, chargingPolicyFilter.Value);
            rateValueRuleGenericRuleQuery.CriteriaFieldValues.Add(serviceTypeFilter.Key, serviceTypeFilter.Value);

            List<RateValueRule> rateValueRules = new RateValueRuleManager().GetApplicableFilteredRules(rateValueRuleDefinitionId, rateValueRuleGenericRuleQuery);
            List<string> exportRateValueRuleDataHeaders;
            List<string[]> exportRateValueRuleDataList;

            ExtractDataFromRule(rateValueRuleDefinitionId, rateValueRules != null ? rateValueRules.Select(itm => itm as GenericRule).ToList() : null, out exportRateValueRuleDataHeaders, out exportRateValueRuleDataList);
            context.RateValueRuleData = new ExportRuleData() { Headers = exportRateValueRuleDataHeaders, Data = exportRateValueRuleDataList };

            Guid tariffRuleDefinitionId = GetTariffRuleDefinitionId(context.ServiceTypeId);
            GenericRuleQuery tariffRuleGenericRuleQuery = new GenericRuleQuery()
            {
                EffectiveDate = context.EffectiveDate,
                RuleDefinitionId = tariffRuleDefinitionId,
                CriteriaFieldValues = new Dictionary<string, object>() { }
            };
            tariffRuleGenericRuleQuery.CriteriaFieldValues.Add(accountFilter.Key, accountFilter.Value);
            tariffRuleGenericRuleQuery.CriteriaFieldValues.Add(chargingPolicyFilter.Key, chargingPolicyFilter.Value);
            tariffRuleGenericRuleQuery.CriteriaFieldValues.Add(serviceTypeFilter.Key, serviceTypeFilter.Value);

            List<TariffRule> tariffRules = new TariffRuleManager().GetApplicableFilteredRules(tariffRuleDefinitionId, tariffRuleGenericRuleQuery);
            List<string> exportTariffRuleDataHeaders;
            List<string[]> exportTariffRuleDataList;

            ExtractDataFromRule(tariffRuleDefinitionId, tariffRules != null ? tariffRules.Select(itm => itm as GenericRule).ToList() : null, out exportTariffRuleDataHeaders, out exportTariffRuleDataList);
            context.TariffRuleData = new ExportRuleData() { Headers = exportTariffRuleDataHeaders, Data = exportTariffRuleDataList };
        }

        private void ExtractDataFromRule(Guid genericRuleDefinitionId, List<GenericRule> genericRules, out List<string> ruleDataHeaders, out List<string[]> exportRuleDataList)
        {
            GenericRuleDefinition genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            int genericRuleDefinitionFieldsCount = genericRuleDefinition.CriteriaDefinition.Fields.Count;

            ruleDataHeaders = genericRuleDefinition.CriteriaDefinition.Fields.Select(itm => itm.FieldName).ToList();
            ruleDataHeaders.Add("Settings");
            exportRuleDataList = null;

            if (genericRules != null && genericRules.Count > 0)
            {
                exportRuleDataList = new List<string[]>();
                foreach (GenericRule genericRule in genericRules)
                {
                    string[] rowData = new string[genericRuleDefinitionFieldsCount + 1];//+1 for settings column

                    if (genericRule.Criteria != null && genericRule.Criteria.FieldsValues != null && genericRule.Criteria.FieldsValues.Count > 0)
                    {
                        int columnIndex = 0;
                        foreach (GenericRuleDefinitionCriteriaField field in genericRuleDefinition.CriteriaDefinition.Fields)
                        {
                            GenericRuleCriteriaFieldValues fieldValue = genericRule.Criteria.FieldsValues.GetRecord(field.FieldName);
                            if (fieldValue == null)
                            {
                                columnIndex++;
                                continue;
                            }

                            string value = string.Empty;
                            IEnumerable<object> objectValues = fieldValue.GetValues();
                            if (objectValues != null)
                            {
                                List<string> objectDescriptionList = new List<string>();
                                foreach (object objectValue in objectValues)
                                {
                                    objectDescriptionList.Add(field.FieldType.GetDescription(objectValue));
                                }
                                value = string.Join(",", objectDescriptionList);
                            }
                            rowData[columnIndex] = value;
                            columnIndex++;
                        }
                    }

                    rowData[genericRuleDefinitionFieldsCount] = genericRule.GetSettingsDescription(new GenericRuleSettingsDescriptionContext() { RuleDefinitionSettings = genericRuleDefinition.SettingsDefinition });
                    exportRuleDataList.Add(rowData);
                }
            }
        }

        public override void ApplyChargingPolicyToVoiceEvent(IVoiceChargingPolicyEvaluatorContext context)
        {
            VoiceEventPricingInfo voiceEventPricingInfo = new VoiceEventPricingInfo();
            voiceEventPricingInfo.ChargingPolicyId = context.ChargingPolicyId;
            context.EventPricingInfo = voiceEventPricingInfo;

            GenericRuleTarget genericRuleTarget = BuildingGenericRuleTarget(context);

            //Rate Value Rules
            RateValueRuleContext rateValueRuleContext = new RateValueRuleContext();
            ApplyRateValueRule(context, rateValueRuleContext, genericRuleTarget);

            if (rateValueRuleContext.Rule != null)
            {
                RateValueRule rateValueRule = rateValueRuleContext.Rule.CastWithValidate<RateValueRule>("rateValueRule", rateValueRuleContext.Rule);
                voiceEventPricingInfo.SaleRateValueRuleId = rateValueRule.RuleId;
                decimal rate = rateValueRuleContext.NormalRate;

                //Rate Type Rules
                RateTypeRuleContext rateTypeRuleContext = new RateTypeRuleContext();
                ApplyRateTypeRule(context, rateTypeRuleContext, genericRuleTarget, rateValueRuleContext.RatesByRateType);

                if (rateTypeRuleContext.Rule != null)
                {
                    RateTypeRule rateTypeRule = rateTypeRuleContext.Rule.CastWithValidate<RateTypeRule>("rateTypeRule", rateTypeRuleContext.Rule);
                    voiceEventPricingInfo.SaleRateTypeRuleId = rateTypeRule.RuleId;

                    if (rateTypeRuleContext.RateTypeId.HasValue && !rateValueRuleContext.RatesByRateType.TryGetValue(rateTypeRuleContext.RateTypeId.Value, out rate))
                        throw new Exception(string.Format("rate of rateTypeId: {0} not found at rateValueRuleContext.RatesByRateType", rateTypeRuleContext.RateTypeId.Value));

                    voiceEventPricingInfo.SaleRateTypeId = rateTypeRuleContext.RateTypeId;
                }

                voiceEventPricingInfo.SaleRate = rate;

                //Tariff Rules 
                TariffRuleContext tariffRuleContext = new TariffRuleContext();
                ApplyTariffRule(context, tariffRuleContext, genericRuleTarget, rateValueRuleContext.CurrencyId != 0 ? rateValueRuleContext.CurrencyId : default(int?), rate);

                if (tariffRuleContext.Rule != null)
                {
                    TariffRule tariffRule = tariffRuleContext.Rule.CastWithValidate<TariffRule>("tariffRule", tariffRuleContext.Rule);
                    voiceEventPricingInfo.SaleTariffRuleId = tariffRule.RuleId;

                    voiceEventPricingInfo.SaleAmount = tariffRuleContext.TotalAmount;
                    voiceEventPricingInfo.SaleCurrencyId = tariffRuleContext.DestinationCurrencyId.HasValue ? tariffRuleContext.DestinationCurrencyId.Value : tariffRuleContext.SourceCurrencyId;
                    voiceEventPricingInfo.SaleDurationInSeconds = tariffRuleContext.EffectiveDurationInSeconds;
                }
            }
        }

        private GenericRuleTarget BuildingGenericRuleTarget(IVoiceChargingPolicyEvaluatorContext context)
        {
            Account packageAccount = new AccountBEManager().GetAccount(context.AccountBEDefinitionId, context.PackageAccountId);

            GenericRuleTarget genericRuleTarget = new GenericRuleTarget();
            genericRuleTarget.EffectiveOn = context.EventTime;
            genericRuleTarget.TargetFieldValues = new Dictionary<string, object>();
            genericRuleTarget.TargetFieldValues.Add("ChargingPolicy", context.ChargingPolicyId);
            genericRuleTarget.Objects = new Dictionary<string, dynamic>();
            genericRuleTarget.Objects.Add("RawCDR", context.RawCDR);
            genericRuleTarget.Objects.Add("MappedCDR", context.MappedCDR);
            genericRuleTarget.Objects.Add("Account", packageAccount);

            return genericRuleTarget;
        }

        private Guid GetRateValueRuleDefinitionId(Guid serviceTypeId)
        {
            Guid? rateValueRuleDefinitionId = RateValueRuleDefinitionId;
            if (!rateValueRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition rateValueRuleDefinition = GetChargingPolicyRuleDefinition(serviceTypeId, RateValueRuleDefinitionSettings.CONFIG_ID);
                rateValueRuleDefinitionId = rateValueRuleDefinition.RuleDefinitionId;
            }
            return rateValueRuleDefinitionId.Value;
        }

        private void ApplyRateValueRule(IVoiceChargingPolicyEvaluatorContext context, RateValueRuleContext rateValueRuleContext, GenericRuleTarget genericRuleTarget)
        {
            Guid rateValueRuleDefinitionId = GetRateValueRuleDefinitionId(context.ServiceTypeId);
            var rateValueRuleManager = new Vanrise.GenericData.Pricing.RateValueRuleManager();
            rateValueRuleManager.ApplyRateValueRule(rateValueRuleContext, rateValueRuleDefinitionId, genericRuleTarget);
        }

        private Guid GetRateTypeRuleDefinitionId(Guid serviceTypeId)
        {
            Guid? rateTypeRuleDefinitionId = RateTypeRuleDefinitionId;
            if (!rateTypeRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition rateValueRuleDefinition = GetChargingPolicyRuleDefinition(serviceTypeId, RateTypeRuleDefinitionSettings.CONFIG_ID);
                RateTypeRuleDefinitionId = rateValueRuleDefinition.RuleDefinitionId;
            }
            return rateTypeRuleDefinitionId.Value;
        }

        private void ApplyRateTypeRule(IVoiceChargingPolicyEvaluatorContext context, RateTypeRuleContext rateTypeRuleContext, GenericRuleTarget genericRuleTarget, Dictionary<int, decimal> ratesByRateType)
        {
            if (ratesByRateType == null)
                return;

            rateTypeRuleContext.TargetTime = context.EventTime;
            rateTypeRuleContext.RateTypes = ratesByRateType.Keys.ToList();

            Guid rateTypeRuleDefinitionId = GetRateTypeRuleDefinitionId(context.ServiceTypeId);

            var rateTypeRuleManager = new Vanrise.GenericData.Pricing.RateTypeRuleManager();
            rateTypeRuleManager.ApplyRateTypeRule(rateTypeRuleContext, rateTypeRuleDefinitionId, genericRuleTarget);
        }

        private Guid GetTariffRuleDefinitionId(Guid serviceTypeId)
        {
            Guid? tariffRuleDefinitionId = TariffRuleDefinitionId;
            if (!tariffRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition tariffRuleDefinition = GetChargingPolicyRuleDefinition(serviceTypeId, TariffRuleDefinitionSettings.CONFIG_ID);
                tariffRuleDefinitionId = tariffRuleDefinition.RuleDefinitionId;
            }
            return tariffRuleDefinitionId.Value;
        }

        private void ApplyTariffRule(IVoiceChargingPolicyEvaluatorContext context, TariffRuleContext tariffRuleContext, GenericRuleTarget genericRuleTarget, int? destinationCurrencyId, decimal rate)
        {
            tariffRuleContext.TargetTime = context.EventTime;
            tariffRuleContext.DurationInSeconds = context.Duration;
            tariffRuleContext.DestinationCurrencyId = destinationCurrencyId;
            tariffRuleContext.Rate = rate;

            Guid tariffRuleDefinitionId = GetTariffRuleDefinitionId(context.ServiceTypeId);

            var tariffRuleManager = new Vanrise.GenericData.Pricing.TariffRuleManager();
            tariffRuleManager.ApplyTariffRule(tariffRuleContext, tariffRuleDefinitionId, genericRuleTarget);
        }

        private ChargingPolicyRuleDefinition GetChargingPolicyRuleDefinition(Guid serviceTypeId, Guid genericRuleDefinitionSettingsCongigId)
        {
            ChargingPolicyRuleDefinition chargingPolicyRuleDefinition = null;
            GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();

            ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
            ChargingPolicyDefinitionSettings chargingPolicyDefinitionSettings = serviceTypeManager.GetChargingPolicyDefinitionSettings(serviceTypeId);
            if (chargingPolicyDefinitionSettings == null)
                throw new NullReferenceException(string.Format("serviceType.Settings.ChargingPolicyDefinitionSettings of serviceTypeId {0}", serviceTypeId));

            List<ChargingPolicyRuleDefinition> ruleDefinitions = chargingPolicyDefinitionSettings.RuleDefinitions;
            if (ruleDefinitions == null)
                throw new NullReferenceException(string.Format("serviceType.Settings.ChargingPolicyDefinitionSettings.ruleDefinitions of serviceTypeId {0}", serviceTypeId));

            foreach (var itm in ruleDefinitions)
            {
                GenericRuleDefinition genericRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(itm.RuleDefinitionId); // needs caching

                if (genericRuleDefinition.SettingsDefinition.ConfigId == genericRuleDefinitionSettingsCongigId)
                {
                    chargingPolicyRuleDefinition = itm;
                    break;
                }
            }

            if (chargingPolicyRuleDefinition == null)
                throw new Exception(string.Format("No chargingPolicyRuleDefinition of TypeId {0} exists for serviceTypeId {1}", genericRuleDefinitionSettingsCongigId, serviceTypeId));

            return chargingPolicyRuleDefinition;
        }
    }
}