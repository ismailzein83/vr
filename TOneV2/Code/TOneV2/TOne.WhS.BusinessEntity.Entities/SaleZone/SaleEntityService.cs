using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityService
    {
        public int SaleEntityServiceId { get; set; }
        public int PriceListId { get; set; }
        public long? ZoneId { get; set; }
        public List<ZoneService> Services { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public class ZoneService
    {
        public int ServiceId { get; set; }
    }
}
