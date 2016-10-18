using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.TransformationSteps
{
    public class CustomerZoneRateStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("63cfd19f-d742-4f37-87ae-2b79c140036e"); } }

        public string CustomerId { get; set; }

        public string SaleZoneId { get; set; }

        public string EffectiveOn { get; set; }

        public string CustomerZoneRate { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var saleRateManagerVariableName = context.GenerateUniqueMemberName("saleRateManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.BusinessEntity.Business.SaleRateManager();", saleRateManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.GetCachedCustomerZoneRate({2}, {3}, {4});",
                this.CustomerZoneRate, saleRateManagerVariableName, this.CustomerId, this.SaleZoneId, this.EffectiveOn);
        }
    }
}
