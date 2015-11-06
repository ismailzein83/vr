using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning.Input
{
    public class ZoneItemInput
    {
        public ZoneItemFilter Filter { get; set; }

        public int FromRow { get; set; }

        public int ToRow { get; set; }
    }

    public class ZoneItemFilter
    {
        public RatePlanOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public char ZoneLetter { get; set; }
    }
}
