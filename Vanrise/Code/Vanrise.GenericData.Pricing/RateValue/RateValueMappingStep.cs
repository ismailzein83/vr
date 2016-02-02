using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueMappingStep : BaseGenericRuleMappingStep
    {
        public string NormalRate { get; set; }

        public string RatesByRateType { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            //var ruleTarget = CreateGenericRuleTarget(context);
            //var ruleContext = new RateValueRuleContext();
            //var ruleManager = new RateValueRuleManager();
            //ruleManager.ApplyRateValueRule(ruleContext, this.RuleDefinitionId, ruleTarget);
            //var targetRecord = context.GetDataRecord(this.TargetRecordName);
            //targetRecord[this.NormalRateFieldName] = ruleContext.NormalRate;
            //targetRecord[this.RatesByRateTypeFieldName] = ruleContext.RatesByRateType;
        }

        public override void GenerateExecutionCode(IDataTransformationCodeContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName();
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateValueRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName();
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateValueRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyRateValueRule({1}, {2}, {3});", 
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.NormalRate;", this.NormalRate, ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RatesByRateType;", this.RatesByRateType, ruleContextVariableName);
        }
    }
}
