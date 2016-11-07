using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.MappingSteps
{
    class GetCDPNsForIdentificationStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("EE9BDD41-2DEB-4E0A-A2BD-871E5A30E03C"); } }

        public int SwitchId { get; set; }
        public int InputCDPN { get; set; }
        public int CDPNIn { get; set; }
        public int CDPNOut { get; set; }

        public int CustomerCDPN { get; set; }
        public int SupplierCDPN { get; set; }
        public int OutputCDPN { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            //context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1};",
            //    this.Target, this.Source);
        } 
    }
}
