using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleZoneService
    {
        public long SaleZoneServiceId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public short ServiceFlag { get; set; }
    }
}
