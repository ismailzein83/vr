using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityService
    {
        public List<ZoneService> Services { get; set; }
        public SaleEntityServiceSource Source { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public enum SaleEntityServiceSource : byte { CustomerZone, CustomerDefault, ProductZone, ProductDefault }

    public class SaleEntityDefaultService
    {
        public long SaleEntityServiceId { get; set; }
        public int PriceListId { get; set; }
        public List<ZoneService> Services { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public class SaleEntityZoneService
    {
        public long SaleEntityServiceId { get; set; }
        public int PriceListId { get; set; }
        public long ZoneId { get; set; }
        public List<ZoneService> Services { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        public string  SourceId { get; set; }
    }

    public class ZoneService
    {
        public int ServiceId { get; set; }
    }
}
