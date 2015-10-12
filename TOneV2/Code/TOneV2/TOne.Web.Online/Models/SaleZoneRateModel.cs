using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.Web.Online.Models
{
    public class SaleZoneRateModel
    {
        public int ZoneId { get; set; }
        public int PriceListId { get; set; }
        public string ZoneName { get; set; }
        public int FlaggedSericeId { get; set; }
        public string Symbol { get; set; }
        public string ServiceColor { get; set; }
        public string CurrencyId { get; set; }
        public decimal SaleRate { get; set; }
        public List<SupplierZoneRateModel> SupplierRates { get; set; }
    }
}