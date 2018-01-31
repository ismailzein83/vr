using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class FixedRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }

        public int? Percentage { get; set; }

        public List<FixedRouteBackupOptionSettings> Backups { get; set; }
    }

    public class FixedRouteBackupOptionSettings
    {
        public int SupplierId { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }
    }
}