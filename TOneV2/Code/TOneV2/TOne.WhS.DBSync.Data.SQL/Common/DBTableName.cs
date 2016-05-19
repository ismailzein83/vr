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
        CurrencyExchangeRate = 0,

        [Description("Currency")]
        Currency = 1,

        [Description("Switch")]
        Switch = 2,

        [Description("CarrierProfile")]
        CarrierProfile = 3,

        [Description("CarrierAccount")]
        CarrierAccount = 4,

        [Description("CustomerZone")]
        CustomerZone = 5,

        [Description("SupplierZone")]
        SupplierZone = 6,

        [Description("SupplierRate")]
        SupplierRate = 7,

        [Description("SupplierCode")]
        SupplierCode = 8,

        [Description("SupplierZoneService")]
        SupplierZoneService = 9,

        [Description("Country")]
        Country = 10,

        [Description("CodeGroup")]
        CodeGroup = 11,
    }
}
