using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TOne.WhS.DBSync.Data.SQL.Common
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
        SaleRate = 14,

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
