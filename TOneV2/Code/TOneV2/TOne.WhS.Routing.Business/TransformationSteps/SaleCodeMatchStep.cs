using System;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class SaleCodeMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("8d2f79d5-ea1e-42b5-8b9e-d26b6a95dc2f"); } }

        public string Number { get; set; }
        public string CustomerId { get; set; }
        public string EffectiveOn { get; set; }

        public string CustomerSellingNumberPlanId { get; set; }
        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
        public string MasterSaleCode { get; set; }
        public string MasterSaleZoneId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);

            var saleCodeMatch = context.GenerateUniqueMemberName("saleCodeMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.Routing.Entities.SaleCodeMatchWithMaster {0} = {1}.GetSaleCodeMatchWithMaster({2},{3},{4});",
                saleCodeMatch, codeMatchManagerName, this.Number, !String.IsNullOrEmpty(this.CustomerId) ? this.CustomerId : "null", this.EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            if (!String.IsNullOrEmpty(this.CustomerSellingNumberPlanId))
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.CustomerSellingNumberPlanId.HasValue)", saleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("{");
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CustomerSellingNumberPlanId.Value;", this.CustomerSellingNumberPlanId, saleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("}");
            }

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatch != null)", saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (!String.IsNullOrEmpty(this.SaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatch.SaleCode;", this.SaleCode, saleCodeMatch);
            if (!String.IsNullOrEmpty(this.SaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatch.SaleZoneId;", this.SaleZoneId, saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.MasterPlanCodeMatch != null)", saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (!String.IsNullOrEmpty(this.MasterSaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.MasterPlanCodeMatch.SaleCode;", this.MasterSaleCode, saleCodeMatch);
            if (!String.IsNullOrEmpty(this.MasterSaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.MasterPlanCodeMatch.SaleZoneId;", this.MasterSaleZoneId, saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
