using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class FixedRouteOptionSettings : IRouteOptionSettings, IFixedRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int NumberOfTries { get; set; }

        public int? Percentage { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }

        public List<FixedRouteBackupOptionSettings> Backups { get; set; }
    }

    public class FixedRouteBackupOptionSettings : IRouteBackupOptionSettings, IFixedRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int NumberOfTries { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }
    }
}