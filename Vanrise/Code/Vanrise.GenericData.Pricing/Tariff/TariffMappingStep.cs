using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class TariffMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("3fc597f0-f5ce-4b49-9aea-24795c81fe80"); } }

        public string InitialRate { get; set; }
        public string DurationInSeconds { get; set; }
        public string EffectiveRate { get; set; }
        public string EffectiveDurationInSeconds { get; set; }
        public string TotalAmount { get; set; }
        public string ExtraChargeRate { get; set; }
        public string ExtraChargeValue { get; set; }
        public string CurrencyId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.TariffRuleContext();", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Rate = {1};", ruleContextVariableName, this.InitialRate);

            if (this.ExtraChargeRate != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.ExtraChargeRate = {1};", ruleContextVariableName, this.ExtraChargeRate);

            if (this.CurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.DestinationCurrencyId = {1};", ruleContextVariableName, this.CurrencyId);

            if (this.DurationInSeconds != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.DurationInSeconds = {1};", ruleContextVariableName, this.DurationInSeconds);

            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.TariffRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTariffRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);

            if (this.EffectiveRate != null)
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) {1} = {2}.EffectiveRate;", ruleContextVariableName, this.EffectiveRate, ruleContextVariableName);

            if (this.EffectiveDurationInSeconds != null)
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.EffectiveDurationInSeconds.HasValue){1} = {0}.EffectiveDurationInSeconds.Value;", ruleContextVariableName, this.EffectiveDurationInSeconds);

            if (this.TotalAmount != null)
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.TotalAmount.HasValue){1} = {0}.TotalAmount.Value;", ruleContextVariableName, this.TotalAmount);

            if (this.ExtraChargeValue != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.ExtraChargeValue;", this.ExtraChargeValue, ruleContextVariableName);

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
