using System.ComponentModel;

namespace TOne.WhS.DBSync.Entities
{
    public enum DBTableName
    {
        [Description("Currency")]
        Currency = 0,

        [Description("CurrencyExchangeRate")]
        CurrencyExchangeRate = 1,

        [Description("Country")]
        Country = 2,

        [Description("CodeGroup")]
        CodeGroup = 3,

        [Description("VRTimeZone")]
        VRTimeZone = 4,

        [Description("ZoneServiceConfig")]
        ZoneServiceConfig = 5,

        [Description("CarrierProfile")]
        CarrierProfile = 6,

        [Description("CarrierAccount")]
        CarrierAccount = 7,

        [Description("Switch")]
        Switch = 8,

        [Description("SaleZone")]
        SaleZone = 9,

        [Description("SupplierZone")]
        SupplierZone = 10,


        [Description("SaleCode")]
        SaleCode = 11,

        [Description("SupplierCode")]
        SupplierCode = 12,


        [Description("SalePriceList")]
        SalePriceList = 13,

        [Description("SupplierPriceList")]
        SupplierPriceList = 14,

        [Description("SaleRate")]
        SaleRate = 15,

        [Description("SaleEntityRoutingProduct")]
        SaleEntityRoutingProduct = 16,

        [Description("SupplierRate")]
        SupplierRate = 17,

        [Description("CustomerCountry")]
        CustomerCountry = 18,

        [Description("File")]
        File = 19,

        [Description("SwitchConnectivity")]
        SwitchConnectivity = 20,

        [Description("SupplierZoneService")]
        SupplierZoneService = 21,

        [Description("SaleEntityService")]
        SaleEntityService = 22,

        [Description("Rule")]
        Rule = 23,

        [Description("CarrierAccountStatusHistory")]
        CarrierAccountStatusHistory = 24
    }
}
