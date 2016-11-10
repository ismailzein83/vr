using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.MappingSteps
{
    public class GetCDPNsForIdentificationStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("EE9BDD41-2DEB-4E0A-A2BD-871E5A30E03C"); } }

        public string SwitchId { get; set; }
        public string InputCDPN { get; set; }
        public string CDPNIn { get; set; }
        public string CDPNOut { get; set; }

        public string CustomerCDPN { get; set; }
        public string SupplierCDPN { get; set; }
        public string OutputCDPN { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            //var switchManagerVariableName = context.GenerateUniqueMemberName("switchManager");
            //context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.BusinessEntity.Business.SwitchManager();", switchManagerVariableName);

            //var switchCDPNsForIdentificationVariableName = context.GenerateUniqueMemberName("switchCDPNsForIdentification");
            //context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.BusinessEntity.Business.SwitchCDPNsForIdentification {0} = {1}.GetSwitchCDPNsForIdentification({2},{3},{4},{5});", switchCDPNsForIdentificationVariableName, switchManagerVariableName,
            //    this.SwitchId, this.InputCDPN, this.CDPNIn, this.CDPNOut);

            //context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CustomerCDPN;", this.CustomerCDPN, switchCDPNsForIdentificationVariableName);

            //context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierCDPN;", this.SupplierCDPN, switchCDPNsForIdentificationVariableName);

            //context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.OutputCDPN;", this.OutputCDPN, switchCDPNsForIdentificationVariableName);
        }
    }
}