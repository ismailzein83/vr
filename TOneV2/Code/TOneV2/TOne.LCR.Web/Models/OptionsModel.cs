using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.LCR.Web.Models
{
    public class OptionsModel
    {
        public string Supplier { get; set; }
        public decimal Rate { get; set; }
        public short ServicesFlag { get; set; }
        public int Priority { get; set; }
        public bool IsBlocked { get; set; }
        public short Percentage { get; set; }
    }
}