using System.ComponentModel;

namespace TOne.WhS.DBSync.Entities
{
    public enum DBTableName
    {
        [Description("Currency")]
        Currency,

        [Description("CurrencyExchangeRate")]
        CurrencyExchangeRate,

        [Description("Country")]
        Country,

        [Description("CodeGroup")]
        CodeGroup,

        [Description("CarrierProfile")]
        CarrierProfile,

        [Description("ZoneServiceConfig")]
        ZoneServiceConfig,

        [Description("VRTimeZone")]
        VRTimeZone,

        [Description("CarrierAccount")]
        CarrierAccount,

        [Description("Switch")]
        Switch,

        [Description("SaleZone")]
        SaleZone,

        [Description("SupplierZone")]
        SupplierZone,


        [Description("SaleCode")]
        SaleCode,

        [Description("SupplierCode")]
        SupplierCode,


        [Description("SalePriceList")]
        SalePriceList,

        [Description("SupplierPriceList")]
        SupplierPriceList,

        [Description("SaleRate")]
        SaleRate,

        [Description("SupplierRate")]
        SupplierRate,

        [Description("CustomerZone")]
        CustomerZone,

        [Description("CustomerSellingProduct")]
        CustomerSellingProduct,

        [Description("File")]
        File,

        [Description("SwitchConnectivity")]
        SwitchConnectivity,

        [Description("SupplierZoneService")]
        SupplierZoneService,

        [Description("SaleEntityService")]
        SaleEntityService,

        [Description("Rule")]
        Rule

    }
}
