using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class PriceListZoneItem
    {
        public List<PriceListCodeItem> Codes { get; set; }
        public PriceListRateItem Rate { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

    }
}
