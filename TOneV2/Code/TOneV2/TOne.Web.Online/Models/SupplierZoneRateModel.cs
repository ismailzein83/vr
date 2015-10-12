using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.Web.Online.Models
{
    public class SupplierZoneRateModel
    {
        public string ZoneName { get; set; }
        public string SupplierName { get; set; }
        public decimal SupplierRate { get; set; }
        public int FlaggedServiceId { get; set; }
        public string Symbol { get; set; }
        public string ServiceColor { get; set; }
    }
}