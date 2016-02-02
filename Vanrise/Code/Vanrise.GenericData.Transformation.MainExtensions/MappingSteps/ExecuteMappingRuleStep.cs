using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ExecuteMappingRuleStep : BaseGenericRuleMappingStep
    {
        public string TargetRecordName { get; set; }

        public string TargetFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            GenericRuleTarget ruleTarget = CreateGenericRuleTarget(context);

            MappingRuleManager ruleManager = new MappingRuleManager();
            var rule = ruleManager.GetMatchRule(this.RuleDefinitionId, ruleTarget);
            if (rule != null)
            {
                var targetRecord = context.GetDataRecord(this.TargetFieldName);
                targetRecord[this.TargetFieldName] = rule.Settings.Value;
            }
        }

        public override void GenerateExecutionCode(IDataTransformationCodeContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName();
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Transformation.MappingRuleManager();", ruleManagerVariableName);
            var ruleVariableName = context.GenerateUniqueMemberName();
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetMatchRule({2}, {3});", ruleVariableName, ruleManagerVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null", ruleVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.{1}.{2} = {3}.Settings.Value;", context.DataRecordsVariableName, this.TargetRecordName, this.TargetFieldName, ruleVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
