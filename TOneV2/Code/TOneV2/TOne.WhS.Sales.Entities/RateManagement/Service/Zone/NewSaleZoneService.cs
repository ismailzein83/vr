using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class NewSaleZoneService
    {
        public long SaleEntityServiceId { get; set; }
        public long SaleZoneId { get; set; }
        public List<ZoneService> Services { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
