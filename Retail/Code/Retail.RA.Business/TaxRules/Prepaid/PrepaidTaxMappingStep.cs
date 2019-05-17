using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class PrepaidTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("9DCA9F0E-1112-4B44-9EAB-2EFD093B6C04"); } }
        public string TotalResidualAmount { get; set; }
        public string TotalTopUpsAmount { get; set; }
        public string TotalTaxValue { set; get; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PrepaidTaxRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PrepaidTaxRuleManager();", ruleManagerVariableName);

            if (!string.IsNullOrEmpty(this.TotalResidualAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.TotalResidualAmount = {1};", ruleContextVariableName, this.TotalResidualAmount);

            if (!string.IsNullOrEmpty(this.TotalTopUpsAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.TotalTopUpsAmount = {1};", ruleContextVariableName, this.TotalTopUpsAmount);

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
