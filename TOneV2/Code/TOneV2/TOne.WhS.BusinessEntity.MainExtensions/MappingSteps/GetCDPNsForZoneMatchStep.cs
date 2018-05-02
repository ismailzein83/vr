using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.MappingSteps
{
    public class GetCDPNsForZoneMatchStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("C29BD6EC-35FB-4EDA-8CF0-E889E6F4C000"); } }

        public Guid NormalizationRuleDefinitionId { get; set; }
        public string EffectiveTime { get; set; }
        public string SwitchId { get; set; }
        public string CDPN { get; set; }
        public string CDPNIn { get; set; }
        public string CDPNOut { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }

        public string SaleZoneCDPN { get; set; }
        public string SupplierZoneCDPN { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var switchManagerVariableName = context.GenerateUniqueMemberName("switchManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new TOne.WhS.BusinessEntity.Business.SwitchManager();", switchManagerVariableName);

            var switchCDPNsForZoneMatchVariableName = context.GenerateUniqueMemberName("switchCDPNsForZoneMatch");
            context.AddCodeToCurrentInstanceExecutionBlock("TOne.WhS.BusinessEntity.Entities.SwitchCDPNsForZoneMatch {0} = {1}.GetSwitchCDPNsForZoneMatch({2},{3},{4},new Guid(\"{5}\"),{6},{7},{8},{9});",
                switchCDPNsForZoneMatchVariableName, switchManagerVariableName, this.CDPN, this.CDPNIn, this.CDPNOut, this.NormalizationRuleDefinitionId, this.EffectiveTime, 
                this.SwitchId, this.CustomerId, this.SupplierId);

            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleZoneCDPN;", this.SaleZoneCDPN, switchCDPNsForZoneMatchVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SupplierZoneCDPN;", this.SupplierZoneCDPN, switchCDPNsForZoneMatchVariableName);
        }
    }
}
