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
        public override Guid ConfigId { get { return new Guid("3fb4b968-b4b1-4072-908f-e6f19eb87be0"); } }

        public string NormalRate { get; set; }

        public string RatesByRateType { get; set; }

        public string CurrencyId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateValueRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.RateValueRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyRateValueRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) ", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{ ");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.NormalRate;", this.NormalRate, ruleContextVariableName);
            if (this.CurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CurrencyId;", this.CurrencyId, ruleContextVariableName);
            if (this.RatesByRateType != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RatesByRateType;", this.RatesByRateType, ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("} ");


            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
