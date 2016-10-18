using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.Routing.Business.TransformationSteps
{
    public class SupplierCodeMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("98a78f7e-ca73-471d-b022-d90ec4406871"); } }

        public string Number { get; set; }
        public string SupplierId { get; set; }
        public string EffectiveOn { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierZoneId { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var codeMatchManagerName = context.GenerateUniqueMemberName("codeMatchBuilder");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.Routing.Business.CodeMatchBuilder();", codeMatchManagerName);
             var supplierCodeMatch = context.GenerateUniqueMemberName("supplierCodeMatch");
             context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetSupplierCodeMatch({2}, {3},{4});", supplierCodeMatch, codeMatchManagerName, this.Number, this.SupplierId, this.EffectiveOn);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)",supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierCode;",this.SupplierCode,supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierZoneId;",this.SupplierZoneId,supplierCodeMatch);
            context.AddCodeToCurrentInstanceExecutionBlock("}");     
        }
    }
}
