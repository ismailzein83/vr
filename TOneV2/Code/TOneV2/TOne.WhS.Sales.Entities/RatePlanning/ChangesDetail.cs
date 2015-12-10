using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ChangesDetail
    {
        public DefaultChangesDetail DefaultChanges { get; set; }
        public IEnumerable<ZoneChangesDetail> ZoneChanges { get; set; }
    }

    public class DefaultChangesDetail
    {
        public DefaultChanges Entity { get; set; }
        public string DefaultRoutingProductName { get; set; }
    }

    public class ZoneChangesDetail
    {
        public ZoneChanges Entity { get; set; }
        public string ZoneName { get; set; }
        public string ZoneRoutingProductName { get; set; }
    }
}
