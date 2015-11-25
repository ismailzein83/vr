using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class NewService
    {
        public long ZoneId { get; set; }

        public short ServicesFlag { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ServiceChange
    {
        public long ZoneServiceId { get; set; }

        public DateTime? EED { get; set; }
    }
}
