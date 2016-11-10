namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SwitchCDPN { CDPN = 0, CustomerCDPN = 1, SupplierCDPN = 2, SaleZoneCDPN = 3, SupplierZoneCDPN = 4 }

    public class SwitchCDPNsForIdentification
    {
        public string CustomerCDPN { get; set; }
        public string SupplierCDPN { get; set; }
        public string OutputCDPN { get; set; }
    }

    public class SwitchCDPNsForZoneMatch
    {
        public string SaleZoneCDPN { get; set; }
        public string SupplierZoneCDPN { get; set; }
    }
}