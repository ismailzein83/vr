using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CDPNIdentification { CDPN = 0, CDPNIn = 1, CDPNOut = 2 }

    public class CDRImportSettings : SettingData
    {
        public SwitchCDRProcessConfiguration SwitchCDRProcessConfiguration { get; set; }
    }

    public class SwitchCDRProcessConfiguration
    {
        public GeneralIdentification GeneralIdentification { get; set; }

        public CustomerIdentification CustomerIdentification { get; set; }

        public SupplierIdentification SupplierIdentification { get; set; }

        public SaleZoneIdentification SaleZoneIdentification { get; set; }

        public SupplierZoneIdentification SupplierZoneIdentification { get; set; }
    }

    public class GeneralIdentification : ICDPNIdentification
    {
        public CDPNIdentification? CDPNIdentification { get; set; }
    }
    public class CustomerIdentification : ICDPNIdentification
    {
        public CDPNIdentification? CDPNIdentification { get; set; }
    }
    public class SupplierIdentification : ICDPNIdentification
    {
        public CDPNIdentification? CDPNIdentification { get; set; }
    }
    public class SaleZoneIdentification : ICDPNIdentification
    {
        public CDPNIdentification? CDPNIdentification { get; set; }
    }
    public class SupplierZoneIdentification : ICDPNIdentification
    {
        public CDPNIdentification? CDPNIdentification { get; set; }
    }

    public interface ICDPNIdentification
    {
        CDPNIdentification? CDPNIdentification { get; set; }
    }
}
