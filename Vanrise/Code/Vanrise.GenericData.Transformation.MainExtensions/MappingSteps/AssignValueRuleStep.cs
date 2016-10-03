using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class AssignValueRuleStep : BaseGenericRuleMappingStep
    {
        public string Target { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var genericRuleDefinitionManager = new GenericRuleDefinitionManager();
            var genericRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(base.RuleDefinitionId);
            if (genericRuleDefinition == null)
                throw new NullReferenceException("genericRuleDefinition");
            var mappingRuleSettingsDefinition = genericRuleDefinition.SettingsDefinition as MappingRuleDefinitionSettings;
            if (mappingRuleSettingsDefinition == null)
                throw new NullReferenceException("mappingSettings");

            Type ruleValueRuntimeType = mappingRuleSettingsDefinition.FieldType.GetNonNullableRuntimeType();

            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Transformation.MappingRuleManager();", ruleManagerVariableName);
            var ruleVariableName = context.GenerateUniqueMemberName("rule");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetMatchRule(new Guid(\"{2}\"), {3});",
                ruleVariableName, ruleManagerVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", ruleVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = ({1})Convert.ChangeType({2}.Settings.Value, typeof({1}));",
                this.Target, CSharpCompiler.TypeToString(ruleValueRuntimeType), ruleVariableName);
            if (!string.IsNullOrEmpty(this.RuleId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RuleId;", this.RuleId, ruleVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
