using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public abstract class BaseGenericRuleMappingStep : MappingStep
    {
        public int RuleDefinitionId { get; set; }

        public string EffectiveTime { get; set; }

        public List<GenericRuleCriteriaFieldMapping> RuleFieldsMappings { get; set; }
        
        protected void GenerateRuleTargetExecutionCode<T>(IDataTransformationCodeGenerationContext context, out string ruleTargetVariableName) 
            where T : GenericRuleTarget
        {
            ruleTargetVariableName = context.GenerateUniqueMemberName("ruleTarget");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new {1}();", ruleTargetVariableName, typeof(T).FullName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.EffectiveOn = {1};", ruleTargetVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetFieldValues = new Dictionary<string, object>();", ruleTargetVariableName);

            foreach (var ruleFieldMapping in this.RuleFieldsMappings)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null) {1}.TargetFieldValues.Add({3}{2}{3}, {0});",
                    ruleFieldMapping.Value, ruleTargetVariableName, ruleFieldMapping.RuleCriteriaFieldName,"\"");
            }            
        }
    }
}
