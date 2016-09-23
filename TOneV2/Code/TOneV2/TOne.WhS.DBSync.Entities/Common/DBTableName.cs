﻿using System.ComponentModel;

namespace TOne.WhS.DBSync.Entities
{
    public enum DBTableName
    {
        [Description("Switch")]
        Switch,

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

        [Description("CarrierAccount")]
        CarrierAccount,

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

        [Description("VRTimeZone")]
        VRTimeZone,

        [Description("Rule")]
        Rule,

       
    }
}
