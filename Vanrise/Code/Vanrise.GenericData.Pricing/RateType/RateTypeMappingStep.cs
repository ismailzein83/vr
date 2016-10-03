using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeMappingStep : BaseGenericRuleMappingStep
    {
        public string RateTypes { get; set; }

        public string RateTypeId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateTypeRuleContext();", ruleContextVariableName);
            if (this.RateTypes != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.RateTypes = {1};", ruleContextVariableName, this.RateTypes);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, base.EffectiveTime);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateTypeRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyRateTypeRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);

            if (this.RateTypeId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RateTypeId;", this.RateTypeId, ruleContextVariableName);

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
