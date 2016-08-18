using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public enum TimeUnit
    {
        Minutes = 0,
        Hours = 1
    }

    public class RouteSettingsData : SettingData
    {
        public RouteDatabasesToKeep RouteDatabasesToKeep { get; set; }

        public SubProcessSettings SubProcessSettings { get; set; }
    }

    public class RouteDatabasesToKeep
    {
        public RouteDatabaseConfiguration CustomerRouteConfiguration { get; set; }

        public RouteDatabaseConfiguration ProductRouteConfiguration { get; set; }
    }

    public class RouteDatabaseConfiguration
    {
        //public int SpecificDBToKeep { get; set; }

        public int CurrentDBToKeep { get; set; }

        public int FutureDBToKeep { get; set; }

        public int MaximumEstimatedExecutionTime { get; set; }

        public TimeUnit TimeUnit { get; set; }
    }

    public class SubProcessSettings
    {
        public int CodeRangeCountThreshold { get; set; }

        public int MaxCodePrefixLength { get; set; }
    }
}
