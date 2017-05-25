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

namespace Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators
{
    public class StandardPolicyEvaluator : VoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("B0B5BC1F-E899-4AE5-AEFB-4FCD5D1BA140"); } }

        public Guid? RateValueRuleDefinitionId { get; set; }
        public Guid? RateTypeRuleDefinitionId { get; set; }
        public Guid? ExtraChargeRuleDefinitionId { get; set; }
        public Guid? TariffRuleDefinitionId { get; set; }


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

                if(rateTypeRuleContext.Rule != null)
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

        private void ApplyRateValueRule(IVoiceChargingPolicyEvaluatorContext context, RateValueRuleContext rateValueRuleContext, GenericRuleTarget genericRuleTarget)
        {
            if (!RateValueRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition rateValueRuleDefinition = GetChargingPolicyRuleDefinition(context.ServiceTypeId, RateValueRuleDefinitionSettings.CONFIG_ID);
                RateValueRuleDefinitionId = rateValueRuleDefinition.RuleDefinitionId;
            }
            var rateValueRuleManager = new Vanrise.GenericData.Pricing.RateValueRuleManager();
            rateValueRuleManager.ApplyRateValueRule(rateValueRuleContext, RateValueRuleDefinitionId.Value, genericRuleTarget);
        }
        private void ApplyRateTypeRule(IVoiceChargingPolicyEvaluatorContext context, RateTypeRuleContext rateTypeRuleContext, GenericRuleTarget genericRuleTarget, Dictionary<int, decimal> ratesByRateType)
        {
            if (ratesByRateType == null)
                return;

            rateTypeRuleContext.TargetTime = context.EventTime;
            rateTypeRuleContext.RateTypes = ratesByRateType.Keys.ToList();

            if (!RateTypeRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition rateValueRuleDefinition = GetChargingPolicyRuleDefinition(context.ServiceTypeId, RateTypeRuleDefinitionSettings.CONFIG_ID);
                RateTypeRuleDefinitionId = rateValueRuleDefinition.RuleDefinitionId;
            }
            var rateTypeRuleManager = new Vanrise.GenericData.Pricing.RateTypeRuleManager();
            rateTypeRuleManager.ApplyRateTypeRule(rateTypeRuleContext, RateTypeRuleDefinitionId.Value, genericRuleTarget);
        }
        private void ApplyTariffRule(IVoiceChargingPolicyEvaluatorContext context, TariffRuleContext tariffRuleContext, GenericRuleTarget genericRuleTarget, int? destinationCurrencyId, decimal rate)
        {
            tariffRuleContext.TargetTime = context.EventTime;
            tariffRuleContext.DurationInSeconds = context.Duration;
            tariffRuleContext.DestinationCurrencyId = destinationCurrencyId;
            tariffRuleContext.Rate = rate;

            if (!TariffRuleDefinitionId.HasValue)
            {
                ChargingPolicyRuleDefinition tariffRuleDefinition = GetChargingPolicyRuleDefinition(context.ServiceTypeId, TariffRuleDefinitionSettings.CONFIG_ID);
                TariffRuleDefinitionId = tariffRuleDefinition.RuleDefinitionId;
            }
            var tariffRuleManager = new Vanrise.GenericData.Pricing.TariffRuleManager();
            tariffRuleManager.ApplyTariffRule(tariffRuleContext, TariffRuleDefinitionId.Value, genericRuleTarget);
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