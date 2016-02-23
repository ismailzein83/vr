using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class SaleCodeMatchStep : MappingStep
    {
        public string Number { get; set; }
        public string CustomerId { get; set; }
        public string EffectiveOn { get; set; }
        public string SaleCode { get; set; }
        public string SaleZoneId { get; set; }
   
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);
             var saleCodeMatch = context.GenerateUniqueMemberName("saleCodeMatch");
             context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetSaleCodeMatch({2}, {3},{4});", saleCodeMatch, codeMatchManagerName, this.Number, this.CustomerId, this.EffectiveOn);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)",saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCode;",this.SaleCode,saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleZoneId;",this.SaleZoneId,saleCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");  
        }
    }
}
