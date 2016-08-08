using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public abstract class BaseGenericRuleMappingStep : MappingStep
    {
        public int RuleDefinitionId { get; set; }

        public string EffectiveTime { get; set; }

        public string RuleId { get; set; }
        public List<GenericRuleCriteriaFieldMapping> RuleFieldsMappings { get; set; }
        public List<GenericRuleObjectMapping> RuleObjectsMappings { get; set; }
        protected void GenerateRuleTargetExecutionCode<T>(IDataTransformationCodeGenerationContext context, out string ruleTargetVariableName)
            where T : GenericRuleTarget
        {
            ruleTargetVariableName = context.GenerateUniqueMemberName("ruleTarget");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new {1}();", ruleTargetVariableName, CSharpCompiler.TypeToString(typeof(T)));
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.EffectiveOn = {1};", ruleTargetVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetFieldValues = new Dictionary<string, object>();", ruleTargetVariableName);

            if (this.RuleFieldsMappings != null)
            {
                foreach (var ruleFieldMapping in this.RuleFieldsMappings)
                {
                    context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null) {1}.TargetFieldValues.Add({3}{2}{3}, {0});",
                        ruleFieldMapping.Value, ruleTargetVariableName, ruleFieldMapping.RuleCriteriaFieldName, "\"");
                }
            }
            if (this.RuleObjectsMappings != null)
            {
                foreach (var ruleObjectMapping in this.RuleObjectsMappings)
                {
                    context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null) {1}.Objects.Add({3}{2}{3}, {0});",
                        ruleObjectMapping.Value, ruleTargetVariableName, ruleObjectMapping.RuleObjectName, "\"");
                }
            }
        }

        protected void SetIdRuleMatched(IDataTransformationCodeGenerationContext context, string ruleContextVariableName)
        {
            if (!string.IsNullOrEmpty(this.RuleId))
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null)", ruleContextVariableName);
                context.AddCodeToCurrentInstanceExecutionBlock("{");
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.Rule.RuleId;", this.RuleId, ruleContextVariableName);
                context.AddCodeToCurrentInstanceExecutionBlock("}");
            }
        }
    }
}
