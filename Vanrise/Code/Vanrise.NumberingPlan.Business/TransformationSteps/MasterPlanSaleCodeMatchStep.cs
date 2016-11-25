using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.NumberingPlan.Business.TransformationSteps
{
    public class MasterPlanSaleCodeMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("073E5902-D609-440A-B849-DF0A5E20F8F4"); } }

        public string Number { get; set; }
        public string EffectiveOn { get; set; }
        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
        public string SellingNumberPlanId { get; set; }
         

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchBuilderVariableName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.NumberingPlan.Business.CodeMatchBuilder();", codeMatchBuilderVariableName);
            var saleCodeMatchVariableName = context.GenerateUniqueMemberName("saleCodeMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("Vanrise.NumberingPlan.Entities.SaleCodeMatch {0} = {1}.GetMasterPlanSaleCodeMatch({2},{3});",
                saleCodeMatchVariableName, codeMatchBuilderVariableName, this.Number, this.EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", saleCodeMatchVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            if (!String.IsNullOrEmpty(this.SaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCode;", this.SaleCode, saleCodeMatchVariableName);
            if (!String.IsNullOrEmpty(this.SaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleZoneId;", this.SaleZoneId, saleCodeMatchVariableName);
            if (!String.IsNullOrEmpty(this.SellingNumberPlanId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SellingNumberPlanId;", this.SellingNumberPlanId, saleCodeMatchVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
