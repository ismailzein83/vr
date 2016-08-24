using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class PriceListZoneService
    {
        public string ZoneName { get; set; }
        public int FlaggedServiceId { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
