using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionDetail
    {
        public RPRouteOption Entity { get; set; }
        public string SupplierName { get; set; }
        public decimal ConvertedSupplierRate { get; set; }
        public string CurrencySymbol { get; set; }
        public int OptionOrder { get; set; }
        public decimal? ACD { get; set; }
        public decimal? ASR { get; set; }
        public decimal? Duration { get; set; }
    }
}
