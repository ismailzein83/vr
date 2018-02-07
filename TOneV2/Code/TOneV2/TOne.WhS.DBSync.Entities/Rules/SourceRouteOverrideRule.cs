using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRouteOverrideRule : SourceBaseRule
    {
        public int? SaleZoneId { get; set; }
        public string CustomerId { get; set; }
        public IEnumerable<SupplierOption> SupplierOptions { get; set; }
        public string SupplierOptionsString { get; set; }
        public IEnumerable<BlockedOption> BlockedOptions { get; set; }
        public string BlockedOptionsString { get; set; }
    }
}