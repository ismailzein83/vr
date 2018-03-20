using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class MatchingSupplierCodeMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("97FB723C-FD0B-4DE9-B5B5-AA32CA59EE65"); } }

        public string Number { get; set; }
        public string SupplierId { get; set; }
        public string SupplierANumberId { get; set; }
        public string EffectiveOn { get; set; }

        public string MatchingSupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierZoneId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);
            var supplierCodeMatch = context.GenerateUniqueMemberName("supplierCodeMatch");
            
            var matchingSupplierId = context.GenerateUniqueMemberName("matchingSupplierId");
            context.AddCodeToCurrentInstanceExecutionBlock("int? {0};", matchingSupplierId);

            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.Routing.Entities.SupplierCodeMatch {0} = {1}.GetSupplierCodeMatch({2}, {3}, {4}, {5}, out {6});", supplierCodeMatch, codeMatchManagerName, this.Number, this.SupplierId, this.SupplierANumberId, this.EffectiveOn, matchingSupplierId);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierCode;", this.SupplierCode, supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierZoneId;", this.SupplierZoneId, supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1};", this.MatchingSupplierId, matchingSupplierId);
        }
    }
}