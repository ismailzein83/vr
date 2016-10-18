using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.TransformationSteps
{
    public class SupplierZoneRateStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("9beb5e90-d35e-4730-8b8f-e039c4fe6c90"); } }

        public string SupplierId { get; set; }

        public string SupplierZoneId { get; set; }

        public string EffectiveOn { get; set; }

        public string SupplierZoneRate { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var supplierRateManagerVariableName = context.GenerateUniqueMemberName("supplierRateManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.BusinessEntity.Business.SupplierRateManager();", supplierRateManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.GetCachedSupplierZoneRate({2}, {3}, {4});",
                this.SupplierZoneRate, supplierRateManagerVariableName, this.SupplierId, this.SupplierZoneId, this.EffectiveOn);
        }
    }
}
