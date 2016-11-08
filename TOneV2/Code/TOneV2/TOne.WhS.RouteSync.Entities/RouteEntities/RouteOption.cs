using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteOption
    {
        public string SupplierId { get; set; }
        public decimal? SupplierRate { get; set; }
        public Decimal? Percentage { get; set; }
    }
}
