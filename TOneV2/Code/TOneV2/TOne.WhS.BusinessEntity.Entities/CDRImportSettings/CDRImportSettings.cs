using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CDPNIdentification { CDPN = 0, CDPNIn = 1, CDPNOut = 2, NormalizedCDPN = 3, NormalizedCDPNIn = 4, NormalizedCDPNOut = 5 }

    public class CDRImportSettings : SettingData
    {
        public SwitchCDRMappingConfiguration SwitchCDRMappingConfiguration { get; set; }

        public CDRImportZoneIdentification CDRImportZoneIdentification { get; set; }
    }

    public class SwitchCDRMappingConfiguration
    {
        public CDPNIdentification? GeneralIdentification { get; set; }

        public CDPNIdentification? CustomerIdentification { get; set; }

        public CDPNIdentification? SupplierIdentification { get; set; }

        public CDPNIdentification? SaleZoneIdentification { get; set; }

        public CDPNIdentification? SupplierZoneIdentification { get; set; }
    }

    public class CDRImportZoneIdentification
    {
        public int? SecondarySellingNumberPlanId { get; set; }
    }
}
