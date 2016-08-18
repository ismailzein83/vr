//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.GenericData.Entities;

//namespace Vanrise.GenericData.Pricing
//{
//    public class PricingManager
//    {
//        RateTypeRuleManager _rateTypeRuleManager = new RateTypeRuleManager();
//        TariffRuleManager _tariffRuleManager = new TariffRuleManager();
//        ExtraChargeRuleManager _extraChargeRuleManager = new ExtraChargeRuleManager();

//        public void ApplyPricingRules(IPricingRulesContext context, GenericRuleTarget target)
//        {
//            Decimal effectiveRate;
//            ApplyRateTypeRule(context, target, out effectiveRate);

//            ApplyExtraChargeRate(context, target, ref effectiveRate);

//            ApplyTariffRate(context, target, ref effectiveRate);
//        }

//        private void ApplyRateTypeRule(IPricingRulesContext context, GenericRuleTarget target, out Decimal effectiveRate)
//        {
//            RateTypeRuleContext rateTypeRuleContext = new RateTypeRuleContext
//            {
//                NormalRate = context.NormalRate,
//                RatesByRateType = context.RatesByRateType,
//                TargetTime = context.TargetTime
//            };
//            int rateTypeRuleDefinitionId = 0;
//            _rateTypeRuleManager.ApplyRateTypeRule(rateTypeRuleContext, rateTypeRuleDefinitionId, target);
//            effectiveRate = rateTypeRuleContext.EffectiveRate;
//            context.EffectiveRate = effectiveRate;
//            context.RateTypeId = rateTypeRuleContext.RateTypeId;
//        }

//        private void ApplyExtraChargeRate(IPricingRulesContext context, GenericRuleTarget target, ref Decimal effectiveRate)
//        {
//            ExtraChargeRuleContext extraChargeRuleContext = new ExtraChargeRuleContext
//            {
//                Rate = effectiveRate,
//                TargetTime = context.TargetTime
//            };
//            int extraChargeRuleDefinitionId = 0;
//            _extraChargeRuleManager.ApplyExtraChargeRule(extraChargeRuleContext, extraChargeRuleDefinitionId, target);
//            effectiveRate = extraChargeRuleContext.Rate;
//            context.EffectiveRate = effectiveRate;
//            context.ExtraChargeRate = extraChargeRuleContext.ExtraChargeRate;
//        }

//        private void ApplyTariffRate(IPricingRulesContext context, GenericRuleTarget target, ref Decimal effectiveRate)
//        {
//            TariffRuleContext tariffRuleContext = new TariffRuleContext
//            {
//                Rate = effectiveRate,
//                DurationInSeconds = context.DurationInSeconds,
//                TargetTime = context.TargetTime
//            };
//            int tariffRuleDefinitionId = 0;
//            _tariffRuleManager.ApplyTariffRule(tariffRuleContext, tariffRuleDefinitionId, target);
//            effectiveRate = tariffRuleContext.EffectiveRate;
//            context.EffectiveRate = effectiveRate;
//            context.TotalAmount = tariffRuleContext.TotalAmount;
//            context.EffectiveDurationInSeconds = tariffRuleContext.EffectiveDurationInSeconds;
//        }
//    }
//}
