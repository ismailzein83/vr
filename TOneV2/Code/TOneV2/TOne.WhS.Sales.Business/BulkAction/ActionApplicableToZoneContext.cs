using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ActionApplicableToZoneContext : IActionApplicableToZoneContext
    {
        public long ZoneId { get; set; }

        public ZoneChanges ZoneDraft { get; set; }
    }
}
