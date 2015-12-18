using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum ZoneChangeType { New = 0, Closed = 1, Deleted = 2 };

    public class ZonePreview
    {
        public string ZoneName { get; set; }

        public ZoneChangeType ChangeType { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
