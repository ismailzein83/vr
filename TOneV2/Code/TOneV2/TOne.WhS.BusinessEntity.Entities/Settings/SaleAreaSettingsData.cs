using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleAreaSettingsData : SettingData
    {
        public IEnumerable<string> FixedKeywords { get; set; }

        public IEnumerable<string> MobileKeywords { get; set; }

        public PrimarySaleEntity PrimarySaleEntity { get; set; }

        public PricelistSettings PricelistSettings { get; set; }

        public PricingSettings PricingSettings { get; set; }
    }

    public enum PrimarySaleEntity { SellingProduct = 0, Customer = 1 }

    public enum SaleAreaSettingsContextEnum { System = 0, SellingProduct = 1, Customer = 2 }
}
