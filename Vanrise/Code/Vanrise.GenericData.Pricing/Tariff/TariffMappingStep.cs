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
        public string  InitialRate { get; set; }
        public string DurationInSeconds { get; set; }
        public string EffectiveRate { get; set; }
        public string EffectiveDurationInSeconds { get; set; }
        public string TotalAmount { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.TariffRuleContext();", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Rate = {1};", ruleContextVariableName, this.InitialRate);
            if (this.DurationInSeconds != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.DurationInSeconds = {1};", ruleContextVariableName, this.DurationInSeconds);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.TariffRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTariffRule({1}, {2}, {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.EffectiveRate;", this.EffectiveRate, ruleContextVariableName);
            if (this.EffectiveDurationInSeconds != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.EffectiveDurationInSeconds;", this.EffectiveDurationInSeconds, ruleContextVariableName);
            if (this.TotalAmount != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.TotalAmount;", this.TotalAmount, ruleContextVariableName);
        }
    }
}
