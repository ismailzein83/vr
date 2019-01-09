using System;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class SMSTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("A64D3211-45AD-4B1B-8C76-BCE2AF7D03DE"); } }

        public string NumberOfSMSs { get; set; }

        public string TotalAmount { get; set; }

        public string TotalTaxValue { set; get; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.SMSTaxRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.SMSTaxRuleManager();", ruleManagerVariableName);

            if (!string.IsNullOrEmpty(this.NumberOfSMSs))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.NumberOfSMSs = {1};", ruleContextVariableName, this.NumberOfSMSs);

            if (!string.IsNullOrEmpty(this.TotalAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.TotalAmount = {1};", ruleContextVariableName, this.TotalAmount);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplySMSTaxRule({1}, new Guid(\"{2}\"), {3});",
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