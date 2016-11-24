using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.NumberingPlan.Business.TransformationSteps
{
    public class MasterPlanSaleCodeStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("8d2f79d5-ea1e-42b5-8b9e-d26b6a95dc2f"); } }

        public string Number { get; set; }

        //public string CustomerId { get; set; }
        public string EffectiveOn { get; set; }
        public string NumberingPlanId { get; set; }
        
        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
        public string MasterSaleCode { get; set; }
        public string MasterSaleZoneId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            //var codeMatchManagerVariableName = context.GenerateUniqueMemberName("codeMatchBuilder");
            //context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.NumberingPlan.Business.CodeMatchBuilder();", codeMatchManagerVariableName);
            //var saleCodeMatchWithMasterVariableName = context.GenerateUniqueMemberName("saleCodeMatch");
            //context.AddCodeToCurrentInstanceExecutionBlock("Vanrise.NumberingPlan.Entities.SaleCodeMatchWithMaster {0} = {1}.GetMatchPlanSaleCode({2},{3});",
            //    saleCodeMatchWithMasterVariableName, codeMatchManagerVariableName, this.Number, this.EffectiveOn);
            
            //context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", saleCodeMatchWithMasterVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("{");

            //if (!String.IsNullOrEmpty(this.NumberingPlanId))
            //{
            //    context.AddCodeToCurrentInstanceExecutionBlock("if({0}.CustomerSellingNumberPlanId.HasValue)", saleCodeMatchWithMasterVariableName);
            //    context.AddCodeToCurrentInstanceExecutionBlock("{");
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CustomerSellingNumberPlanId.Value;", this.NumberingPlanId, saleCodeMatchWithMasterVariableName);
            //    context.AddCodeToCurrentInstanceExecutionBlock("}");
            //}

            //context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatch != null)", saleCodeMatchWithMasterVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("{");
            //if (!String.IsNullOrEmpty(this.SaleCode))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatch.SaleCode;", this.SaleCode, saleCodeMatchWithMasterVariableName);
            //if (!String.IsNullOrEmpty(this.SaleZoneId))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatch.SaleZoneId;", this.SaleZoneId, saleCodeMatchWithMasterVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("}");

            //context.AddCodeToCurrentInstanceExecutionBlock("if({0}.MasterPlanCodeMatch != null)", saleCodeMatchWithMasterVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("{");
            //if (!String.IsNullOrEmpty(this.MasterSaleCode))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.MasterPlanCodeMatch.SaleCode;", this.MasterSaleCode, saleCodeMatchWithMasterVariableName);
            //if (!String.IsNullOrEmpty(this.MasterSaleZoneId))
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.MasterPlanCodeMatch.SaleZoneId;", this.MasterSaleZoneId, saleCodeMatchWithMasterVariableName);
            //context.AddCodeToCurrentInstanceExecutionBlock("}");

            //context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
