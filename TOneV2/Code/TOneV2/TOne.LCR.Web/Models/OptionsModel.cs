using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.LCR.Web.Models
{
    public class OptionsModel
    {
        public string SupplierId { get; set; }
        public string Supplier { get; set; }
        public decimal Rate { get; set; }
        public short ServicesFlag { get; set; }
        public int Priority { get; set; }
        public bool IsBlocked { get; set; }
        public short Percentage { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
    }
}