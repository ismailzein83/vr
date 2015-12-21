using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class RatePreview
    {
        public string ZoneName { get; set; }

        public RateChangeType ChangeType { get; set; }

        public Decimal? RecentRate { get; set; }

        public Decimal NewRate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
