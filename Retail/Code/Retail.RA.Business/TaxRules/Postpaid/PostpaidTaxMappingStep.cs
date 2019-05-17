using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class PostpaidTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("C96E67E1-F002-484A-9ED6-C876982DB642"); } }
        public string TotalAmount { get; set; }
        public string TotalTaxValue { set; get; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PostpaidTaxRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PostpaidTaxRuleManager();", ruleManagerVariableName);

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
