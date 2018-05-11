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
using Vanrise.Common.Business;

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

            CurrencyManager currencyManager = new CurrencyManager();

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
            List<object[]> exportRateValueRuleDataList;
            List<string> rateValueRuleSettingHeaders = new List<string>();
            GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
            var rateValueRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(rateValueRuleDefinitionId);
            rateValueRuleDefinition.ThrowIfNull("rateValueRuleDefinition", rateValueRuleDefinitionId);
            rateValueRuleDefinition.SettingsDefinition.ThrowIfNull("rateValueRuleDefinition.SettingsDefinition");
            var rateValueFieldNames = rateValueRuleDefinition.SettingsDefinition.GetFieldNames();
            if (rateValueFieldNames != null && rateValueFieldNames.Count != 0)
            {
                foreach (var rateName in rateValueFieldNames)
                {
                    rateValueRuleSettingHeaders.Add(rateName);
                }
            }
            ExtractDataFromRule(rateValueRuleDefinitionId, rateValueRules != null ? rateValueRules.Select(itm => itm as GenericRule).ToList() : null, rateValueRuleSettingHeaders,
                (genericRuleDefinition, genericRule) =>
                {
                    RateValueRule rateValueRule = genericRule as RateValueRule;
                    List<object> settingData = new List<object>();
                    settingData.Add(currencyManager.GetCurrencySymbol(rateValueRule.Settings.CurrencyId));
                    var rateNamesAndValues = genericRule.GetSettingsValuesByName();
                    if (rateNamesAndValues != null && rateNamesAndValues.Count != 0 && rateValueFieldNames!=null && rateValueFieldNames.Count!=0)
                    {
                        foreach (var rateName in rateValueFieldNames)
                        {
                            if(!rateName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var data = rateNamesAndValues.GetRecord(rateName);
                                settingData.Add(data);
                            }
                        }
                    }
                   
                    return settingData;
                }
                , out exportRateValueRuleDataHeaders, out exportRateValueRuleDataList);
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
            List<object[]> exportTariffRuleDataList;
            List<string> tariffRuleSettingHeaders = new List<string>() { "Currency", "Settings" };

            var tarrifRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(tariffRuleDefinitionId);
            tarrifRuleDefinition.ThrowIfNull("tarrifRuleDefinition", tarrifRuleDefinition);
            tarrifRuleDefinition.SettingsDefinition.ThrowIfNull("tarrifRuleDefinition.SettingsDefinition");
         
            ExtractDataFromRule(tariffRuleDefinitionId, tariffRules != null ? tariffRules.Select(itm => itm as GenericRule).ToList() : null, tariffRuleSettingHeaders,
                (genericRuleDefinition, genericRule) =>
                {
                    TariffRule tariffRule = genericRule as TariffRule;
                    List<object> settingData = new List<object>();
                    tariffRule.Settings.ThrowIfNull("tariffRule.Settings");
                    settingData.Add(currencyManager.GetCurrencySymbol(tariffRule.Settings.CurrencyId));
                    settingData.Add(tariffRule.Settings.GetPricingDescription());
                    return settingData;
                }
                , out exportTariffRuleDataHeaders, out exportTariffRuleDataList);
            context.TariffRuleData = new ExportRuleData() { Headers = exportTariffRuleDataHeaders, Data = exportTariffRuleDataList };
        }

        private void ExtractDataFromRule(Guid genericRuleDefinitionId, List<GenericRule> genericRules, List<string> settingHeaders, Func<GenericRuleDefinition, GenericRule, List<object>> getSettingDataList, out List<string> ruleDataHeaders, out List<object[]> exportRuleDataList)
        {
            GenericRuleDefinition genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            List<string> filteredCriteriaHeaders = new List<string>();
            List<GenericRuleDefinitionCriteriaField> filteredCriteria = new List<GenericRuleDefinitionCriteriaField>();
            List<string> excludedCriteria = new List<string> { "Account", "ServiceType", "ChargingPolicy", "Package" };
            foreach (var criteriaField in genericRuleDefinition.CriteriaDefinition.Fields)
            {
                if (!excludedCriteria.Contains(criteriaField.FieldName))
                {
                    filteredCriteriaHeaders.Add(criteriaField.Title);
                    filteredCriteria.Add(criteriaField);
                }
            }
            int genericRuleDefinitionFieldsCount = filteredCriteria.Count;
            ruleDataHeaders = filteredCriteriaHeaders;

            if (settingHeaders != null)
                ruleDataHeaders.AddRange(settingHeaders);

            ruleDataHeaders.Add("Effective On");
            exportRuleDataList = null;

            if (genericRules != null && genericRules.Count > 0)
            {
                exportRuleDataList = new List<object[]>();
                List<object[]> exportRuleDataListEdited = new List<object[]>();
                Dictionary<int, bool> hasValueByColumn = new Dictionary<int, bool>();
                int totalColumns = 0;

                foreach (GenericRule genericRule in genericRules)
                {
                    int columnIndex = 0;
                    object[] rowData = new object[ruleDataHeaders.Count];

                    if (genericRule.Criteria != null && genericRule.Criteria.FieldsValues != null && genericRule.Criteria.FieldsValues.Count > 0)
                    {
                        foreach (GenericRuleDefinitionCriteriaField field in genericRuleDefinition.CriteriaDefinition.Fields)
                        {
                            if (excludedCriteria.Contains(field.FieldName))
                            {
                                continue;
                            }
                            GenericRuleCriteriaFieldValues fieldValue = genericRule.Criteria.FieldsValues.GetRecord(field.FieldName);
                            if (fieldValue == null)
                            {
                                bool isValueExists;
                                if (hasValueByColumn.TryGetValue(columnIndex, out isValueExists))
                                {
                                    if (!isValueExists)
                                        hasValueByColumn[columnIndex] = false;
                                }
                                else
                                {
                                    hasValueByColumn.Add(columnIndex, false);
                                }
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

                            bool valueExists;
                            if (hasValueByColumn.TryGetValue(columnIndex, out valueExists))
                            {
                                if (!valueExists && value != null)
                                    hasValueByColumn[columnIndex] = true;
                            }
                            else
                            {
                                hasValueByColumn.Add(columnIndex, value != null);
                            }
                            columnIndex++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < genericRuleDefinitionFieldsCount; i++)
                        {
                            bool isValueExists;
                            if (hasValueByColumn.TryGetValue(i, out isValueExists))
                            {
                                if (!isValueExists)
                                    hasValueByColumn[i] = false;
                            }
                            else
                            {
                                hasValueByColumn.Add(i, false);
                            }
                        }
                    }
                    if (settingHeaders != null)
                    {
                        List<object> settingData = getSettingDataList(genericRuleDefinition, genericRule);
                        settingData.ThrowIfNull("settingData", genericRule.RuleId);

                        columnIndex = genericRuleDefinitionFieldsCount;
                        foreach (var settingValue in settingData)
                        {
                            rowData[columnIndex] = settingValue;
                          
                            bool valueExists;
                            if (hasValueByColumn.TryGetValue(columnIndex, out valueExists))
                            {
                                if (!valueExists && settingValue != null)
                                    hasValueByColumn[columnIndex] = true;
                            }
                            else
                            {
                                hasValueByColumn.Add(columnIndex, settingValue != null);
                            }

                            columnIndex++;
                        }
                    }
                    int effectiveOnIndex = ruleDataHeaders.Count-1;
                    if (genericRule.BeginEffectiveTime != null)
                        rowData[effectiveOnIndex] = genericRule.BeginEffectiveTime.ToString();

                    bool isFilled;
                    if (hasValueByColumn.TryGetValue(effectiveOnIndex, out isFilled))
                    {
                        if (!isFilled && genericRule.BeginEffectiveTime != null)
                            hasValueByColumn[effectiveOnIndex] = true;
                    }
                    else
                    {
                        hasValueByColumn.Add(effectiveOnIndex, genericRule.BeginEffectiveTime != null);
                    }

                    exportRuleDataList.Add(rowData);
                    totalColumns = rowData.Length;
                }
                bool ruleDataHeadersEdited = false;
                foreach (var exportedRule in exportRuleDataList)
                {
                    List<object> rowDataEditedList = new List<object>();
                    for (int i = 0; i < exportedRule.Length; i++)
                    {
                        if(!hasValueByColumn[i])
                        {
                            if (!ruleDataHeadersEdited)
                            {
                                int diff = totalColumns - ruleDataHeaders.Count;
                                int result = i - diff;
                                if (result >= 0)
                                    ruleDataHeaders.RemoveAt(result);
                                else
                                    ruleDataHeaders.RemoveAt(i);
                            }
                        }
                        else
                        {
                            rowDataEditedList.Add(exportedRule[i]);
                        }
                    }
                    ruleDataHeadersEdited = true;
                    int newLength = rowDataEditedList.Count;
                    object[] rowDataEditedArray = new object[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        rowDataEditedArray[i] = rowDataEditedList[i];
                    }
                    exportRuleDataListEdited.Add(rowDataEditedArray);
                }
                exportRuleDataList = exportRuleDataListEdited;
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