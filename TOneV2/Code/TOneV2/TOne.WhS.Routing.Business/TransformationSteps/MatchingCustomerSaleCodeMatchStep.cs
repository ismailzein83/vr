using System;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class MatchingCustomerSaleCodeMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("A9356E61-AF73-4F23-8B39-DA15B7C678AC"); } }
        public string EffectiveOn { get; set; }
        public string Number { get; set; }
        public string CustomerId { get; set; }
        public string CustomerANumberId { get; set; }

        public string MatchingCustomerId { get; set; }
        public string CustomerSellingNumberPlanId { get; set; }
        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
        public string MasterSaleCode { get; set; }
        public string MasterSaleZoneId { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);

            var customerSaleCodeMatch = context.GenerateUniqueMemberName("customerSaleCodeMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.Routing.Entities.CustomerSaleCodeMatchWithMaster {0} = {1}.GetCustomerSaleCodeMatchWithMaster({2}, {3}, {4}, {5});",
                customerSaleCodeMatch, codeMatchManagerName, this.Number, !String.IsNullOrEmpty(this.CustomerId) ? this.CustomerId : "null",
                !String.IsNullOrEmpty(this.CustomerANumberId) ? this.CustomerANumberId : "null", this.EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", customerSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatchWithMaster != null)", customerSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (!String.IsNullOrEmpty(this.CustomerSellingNumberPlanId))
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatchWithMaster.CustomerSellingNumberPlanId.HasValue)", customerSaleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("{");
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatchWithMaster.CustomerSellingNumberPlanId.Value;", this.CustomerSellingNumberPlanId, customerSaleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("}");
            }

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatchWithMaster.SaleCodeMatch != null)", customerSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (!String.IsNullOrEmpty(this.SaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatchWithMaster.SaleCodeMatch.SaleCode;", this.SaleCode, customerSaleCodeMatch);
            
            if (!String.IsNullOrEmpty(this.SaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatchWithMaster.SaleCodeMatch.SaleZoneId;", this.SaleZoneId, customerSaleCodeMatch);
            
            context.AddCodeToCurrentInstanceExecutionBlock("}");

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.SaleCodeMatchWithMaster.MasterPlanCodeMatch != null)", customerSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (!String.IsNullOrEmpty(this.MasterSaleCode))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatchWithMaster.MasterPlanCodeMatch.SaleCode;", this.MasterSaleCode, customerSaleCodeMatch);
            if (!String.IsNullOrEmpty(this.MasterSaleZoneId))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCodeMatchWithMaster.MasterPlanCodeMatch.SaleZoneId;", this.MasterSaleZoneId, customerSaleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");

            context.AddCodeToCurrentInstanceExecutionBlock("}");

            if (!String.IsNullOrEmpty(this.MatchingCustomerId))
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0}.CustomerId.HasValue)", customerSaleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("{");
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CustomerId.Value;", this.MatchingCustomerId, customerSaleCodeMatch);
                context.AddCodeToCurrentInstanceExecutionBlock("}");
            }
            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}