using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeMappingStep : BaseGenericRuleMappingStep
    {
        public string NormalRate { get; set; }

        public string RatesByRateType { get; set; }

        public string EffectiveRate { get; set; }

        public string RateTypeId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateTypeRuleContext();", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.NormalRate = {1};", ruleContextVariableName, this.NormalRate);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.RatesByRateType = {1};", ruleContextVariableName, this.RatesByRateType);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, base.EffectiveTime);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateTypeRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyRateTypeRule({1}, {2}, {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.EffectiveRate;", this.EffectiveRate, ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RateTypeId;", this.RateTypeId, ruleContextVariableName);
        }
    }
}
