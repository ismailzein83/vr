using System.ComponentModel;

namespace TOne.WhS.DBSync.Entities
{
    public enum DBTableName
    {
        [Description("CurrencyExchangeRate")]
        CurrencyExchangeRate,

        [Description("Currency")]
        Currency,

        [Description("Switch")]
        Switch,

        [Description("CarrierProfile")]
        CarrierProfile,

        [Description("CarrierAccount")]
        CarrierAccount,

        [Description("SupplierZoneService")]
        SupplierZoneService,

        [Description("Country")]
        Country,

        [Description("CodeGroup")]
        CodeGroup,

        [Description("SaleZone")]
        SaleZone,

        [Description("SupplierZone")]
        SupplierZone,

        [Description("SaleRate")]
        SaleRate ,

        [Description("SupplierRate")]
        SupplierRate,

        [Description("SaleCode")]
        SaleCode,

        [Description("SupplierCode")]
        SupplierCode,

        [Description("SalePriceList")]
        SalePriceList,

        [Description("SupplierPriceList")]
        SupplierPriceList,
    }
}
