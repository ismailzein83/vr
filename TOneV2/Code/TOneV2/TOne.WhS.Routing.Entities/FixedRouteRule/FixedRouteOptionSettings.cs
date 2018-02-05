using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionSettings
    {
        int SupplierId { get; }
        int NumberOfTries { get; }
        int? Percentage { get; }
    }

    public interface IRouteBackupOptionSettings
    {
        int SupplierId { get; }

        int NumberOfTries { get; }
    }

    public interface IFixedRouteOptionSettings
    {
        List<RouteOptionFilterSettings> Filters { get; set; }
    }

    public class FixedRouteOptionSettings : IRouteOptionSettings, IFixedRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }

        public int? Percentage { get; set; }

        public int NumberOfTries { get; set; }

        public List<FixedRouteBackupOptionSettings> Backups { get; set; }
    }

    public class FixedRouteBackupOptionSettings : IRouteBackupOptionSettings, IFixedRouteOptionSettings
    {
        public int SupplierId { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }

        public int NumberOfTries { get; set; }
    }
}