using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizationRuleStep : BaseGenericRuleMappingStep
    {
        public string Value { get; set; }

        public string NormalizedValue { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Normalization.NormalizeRuleContext();", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Value = {1};", ruleContextVariableName, this.Value);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Normalization.NormalizationRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyNormalizationRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.NormalizedValue;", this.NormalizedValue, ruleContextVariableName);

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
