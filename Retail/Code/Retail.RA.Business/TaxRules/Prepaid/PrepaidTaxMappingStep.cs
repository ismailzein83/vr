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
        public override Guid ConfigId { get { return new Guid("C8AE398C-9B0D-433B-88FD-57B6EBF3482B"); } }

        #region TopUp
        public string TotalTopUpAmount { get; set; }
        public string TotalTopUpTaxValue { set; get; }
        #endregion

        #region Residual 
        #endregion

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            //string ruleTargetVariableName;
            //base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            //var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            //StringBuilder contextInitialization = new StringBuilder(@"var #VARIABLENAME# = new Retail.RA.Business.PrepaidTaxRuleContext(){
            //                                                            TopUpContext = new Retail.RA.Business.TransactionTaxRuleContext(),
            //                                                            ResidualContext = new Retail.RA.Business.PrepaidResidualTaxRuleContext()
            //                                                         };");
            //contextInitialization.Replace("#VARIABLENAME#", ruleContextVariableName);

            //context.AddCodeToCurrentInstanceExecutionBlock(contextInitialization.ToString());
            //var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            //context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PrepaidTaxRuleManager();", ruleManagerVariableName);

            //if (!string.IsNullOrEmpty(this.TotalTopUpAmount))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0}.TopUpContext.TotalAmount = {1};", ruleContextVariableName, this.TotalTopUpAmount);

            //context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTaxRule({1}, new Guid(\"{2}\"), {3});",
            //  ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) ", ruleContextVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("{ ");

            //if (!string.IsNullOrEmpty(this.TotalTopUpTaxValue))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.TopUpContext.TotalTaxValue;", this.TotalTopUpTaxValue, ruleContextVariableName);

            //context.AddCodeToCurrentInstanceExecutionBlock("} ");

            //base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
