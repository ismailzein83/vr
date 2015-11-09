using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class Changes
    {
        public DefaultChanges DefaultChanges { get; set; }

        public List<ZoneChanges> ZoneChanges { get; set; }
    }
}
