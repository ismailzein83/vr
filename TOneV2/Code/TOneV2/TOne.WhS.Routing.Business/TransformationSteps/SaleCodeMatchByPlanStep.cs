using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class SaleCodeMatchByPlanStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("8726EFB9-7C3A-4EDE-9430-1F4B33397BB8"); } }
        public string EffectiveOn { get; set; }
        public string SellingNumberPlanId { get; set; }
        public string CDPN { get; set; }
        public string CGPN { get; set; }

        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
        public string OriginatingSaleCode { get; set; }
        public string OriginatingSaleZoneId { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);

            var saleCodeMatch = context.GenerateUniqueMemberName("saleCodeMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.Routing.Entities.SaleCodeMatch {0} = {1}.GetSaleCodeMatchByPlan({2},{3},{4});",
                saleCodeMatch, codeMatchManagerName, this.CDPN, this.SellingNumberPlanId, this.EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            if (!String.IsNullOrEmpty(this.SaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCode;", this.SaleCode, saleCodeMatch);
            if (!String.IsNullOrEmpty(this.SaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleZoneId;", this.SaleZoneId, saleCodeMatch);

            context.AddCodeToCurrentInstanceExecutionBlock("}");

            var originatingSaleCodeMatch = context.GenerateUniqueMemberName("originatingSaleCodeMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.Routing.Entities.SaleCodeMatch {0} = {1}.GetSaleCodeMatchByPlan({2},{3},{4});",
                originatingSaleCodeMatch, codeMatchManagerName, this.CGPN, this.SellingNumberPlanId, this.EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", originatingSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            if (!String.IsNullOrEmpty(this.OriginatingSaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCode;", this.OriginatingSaleCode, originatingSaleCodeMatch);
            if (!String.IsNullOrEmpty(this.OriginatingSaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleZoneId;", this.OriginatingSaleZoneId, originatingSaleCodeMatch);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}