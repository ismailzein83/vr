using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators
{
    public class StandardPolicyEvaluator : VoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("B0B5BC1F-E899-4AE5-AEFB-4FCD5D1BA140"); }
        }

        public Guid? TariffRuleDefinitionId { get; set; }
        public Guid? RateValueRuleDefinitionId { get; set; }
        public Guid? RateTypeRuleDefinitionId { get; set; }
        public Guid? ExtraChargeRuleDefinitionId { get; set; }


        public override void ApplyChargingPolicyToVoiceEvent(IVoiceChargingPolicyEvaluatorContext context)
        {
            throw new NotImplementedException();
        }

        private void ApplyTariffRule(IVoiceChargingPolicyEvaluatorContext context)
        {
            //var ruleContext = new Vanrise.GenericData.Pricing.TariffRuleContext();
            //ruleContext.TargetTime = context.EventTime;
            //ruleContext.DurationInSeconds = context.Duration;

            //GenericRuleTarget genericRuleTarget = new GenericRuleTarget();
            //genericRuleTarget.Objects = new Dictionary<string,dynamic>();
            //genericRuleTarget.Objects.Add("RawCDR", context.RawCDR);
            //genericRuleTarget.Objects.Add("MappedCDR", context.MappedCDR);

            //var ruleManagerVariableName = new Vanrise.GenericData.Pricing.TariffRuleManager();
            //ruleManagerVariableName.ApplyTariffRule(ruleContext, TariffRuleDefinitionId.Value, genericRuleTarget);

            //VoiceEventPricingInfo voiceEventPricingInfo = new VoiceEventPricingInfo();
            //voiceEventPricingInfo.Rate = ruleContext.Rate;
            //voiceEventPricingInfo.Amount = ruleContext.TotalAmount.HasValue ? ruleContext.TotalAmount.Value : default(decimal);
            //voiceEventPricingInfo.RateTypeId = null;
            //voiceEventPricingInfo.CurrencyId = default(int);

            //context.EventPricingInfo = voiceEventPricingInfo;



            //context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, this.EffectiveTime);
            //context.AddCodeToCurrentInstanceExecutionBlock("{0}.Rate = {1};", ruleContextVariableName, this.InitialRate);

            //if (this.DurationInSeconds != null)
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0}.DurationInSeconds = {1};", ruleContextVariableName, this.DurationInSeconds);

            //var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            //context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.TariffRuleManager();", ruleManagerVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTariffRule({1}, new Guid(\"{2}\"), {3});",
            //    ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);

            //if (this.EffectiveRate != null)
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.EffectiveRate;", this.EffectiveRate, ruleContextVariableName);

            //if (this.EffectiveDurationInSeconds != null)
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.EffectiveDurationInSeconds.Value;", this.EffectiveDurationInSeconds, ruleContextVariableName);

            //if (this.TotalAmount != null)
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.TotalAmount.Value;", this.TotalAmount, ruleContextVariableName);

            //if (this.ExtraChargeValue != null)
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.ExtraChargeValue;", this.ExtraChargeValue, ruleContextVariableName);
        }
    }
}
