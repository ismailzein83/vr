using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts
{
    public static class Helper
    {
        internal static RuleTree GetRuleTree(PricingEntity pricingEntity, long pricingEntityId, int serviceTypeId, ChargingPolicyPartType pricingPolicyPartType, IEnumerable<Vanrise.GenericData.Entities.GenericRule> rules)
        {
            string cacheName = String.Format("Helper_GetRuleTree_{0}_{1}_{2}", pricingEntityId, serviceTypeId, pricingPolicyPartType);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<PricingEntityCacheManager>().GetOrCreateObject(cacheName, pricingEntity,
                () =>
                {
                    var criteriaDefinition = (new Retail.BusinessEntity.Business.ConfigurationManager()).GetCriteriaDefinition(serviceTypeId, pricingPolicyPartType);
                    if (criteriaDefinition == null)
                        throw new NullReferenceException(String.Format("criteriaDefinition. serviceTypeId '{0}'", serviceTypeId));
                    return new Vanrise.GenericData.Business.GenericRuleManager<Vanrise.GenericData.Entities.GenericRule>().BuildRuleTree(criteriaDefinition, rules);
                });
        }

        internal static Vanrise.GenericData.Pricing.RateValueRuleContext CreateRateValueRuleContext(IChargingPolicyRateValueContext voiceRateValueContext)
        {
            return new Vanrise.GenericData.Pricing.RateValueRuleContext();
        }

        internal static void UpdateVoiceRateValueContext(IChargingPolicyRateValueContext voiceRateValueContext, Vanrise.GenericData.Pricing.RateValueRuleContext rateValueRuleContext)
        {
            voiceRateValueContext.NormalRate = rateValueRuleContext.NormalRate;
            voiceRateValueContext.RatesByRateType = rateValueRuleContext.RatesByRateType;
        }

        internal static Vanrise.GenericData.Pricing.RateTypeRuleContext CreateRateTypeRuleContext(IChargingPolicyRateTypeContext voiceRateTypeContext)
        {
            return new Vanrise.GenericData.Pricing.RateTypeRuleContext
            {
                NormalRate = voiceRateTypeContext.NormalRate,
                RatesByRateType = voiceRateTypeContext.RatesByRateType,
                TargetTime = voiceRateTypeContext.TargetTime
            };
        }

        internal static void UpdateVoiceRateTypeContext(IChargingPolicyRateTypeContext voiceRateTypeContext, Vanrise.GenericData.Pricing.RateTypeRuleContext rateTypeRuleContext)
        {
            voiceRateTypeContext.EffectiveRate = rateTypeRuleContext.EffectiveRate;
            voiceRateTypeContext.RateTypeId = rateTypeRuleContext.RateTypeId;
        }


        internal static Vanrise.GenericData.Pricing.TariffRuleContext CreateTariffRuleContext(IChargingPolicyDurationTariffContext voiceTariffContext)
        {
            return new Vanrise.GenericData.Pricing.TariffRuleContext
            {
                Rate = voiceTariffContext.Rate,
                DurationInSeconds = voiceTariffContext.DurationInSeconds,
                TargetTime = voiceTariffContext.TargetTime
            };
        }

        internal static void UpdateVoiceTariffContext(IChargingPolicyDurationTariffContext voiceTariffContext, Vanrise.GenericData.Pricing.TariffRuleContext tariffRuleContext)
        {
            voiceTariffContext.EffectiveRate = tariffRuleContext.EffectiveRate;
            voiceTariffContext.TotalAmount = tariffRuleContext.TotalAmount;
            voiceTariffContext.EffectiveDurationInSeconds = tariffRuleContext.EffectiveDurationInSeconds;
        }
    }
}
