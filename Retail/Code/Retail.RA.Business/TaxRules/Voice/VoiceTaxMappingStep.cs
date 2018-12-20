using System;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class VoiceTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("5DB02AB2-4B23-42CC-92E5-64145E75A57E"); } }

        public string DurationInSeconds { get; set; }

        public string TotalAmount { get; set; }

        public string TotalTaxValue { set; get; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.VoiceTaxRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.VoiceTaxRuleManager();", ruleManagerVariableName);

            if (!string.IsNullOrEmpty(this.DurationInSeconds))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.DurationInSeconds = {1};", ruleContextVariableName, this.DurationInSeconds);

            if (!string.IsNullOrEmpty(this.TotalAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.TotalAmount = {1};", ruleContextVariableName, this.TotalAmount);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTaxRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) ", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{ ");

            if (!string.IsNullOrEmpty(this.TotalTaxValue))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.TotalTaxValue;", this.TotalTaxValue, ruleContextVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("} ");

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}