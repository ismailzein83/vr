using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerZones
    {
        public int CustomerId { get; set; }

        public List<CustomerZone> Zones { get; set; }

        public DateTime StartEffectiveTime { get; set; }
    }

    public class CustomerZone
    {
        public long ZoneId { get; set; }

        public short? ServicesFlag { get; set; }
    }
}
