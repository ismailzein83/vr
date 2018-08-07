using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NewZoneService : Vanrise.Entities.IDateEffectiveSettings
    {
        public long ZoneServiceId { get; set; }

        public int SupplierId { get; set; }
        public IZone Zone { get; set; }

        public List<ZoneService> ZoneServices { get; set; }

        public DateTime BED { get; set; }
        public bool IsExcluded { get; set; }
        public DateTime? EED { get; set; }
    }
}
