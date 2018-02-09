using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Entities
{
    public class BaseRouteOption
    {
        public string SupplierId { get; set; }
        public decimal? SupplierRate { get; set; }
        public bool IsBlocked { get; set; }
        public int NumberOfTries { get; set; }
        public bool IsValid { get { return NumberOfTries > 0; } }
    }
    public class RouteOption : BaseRouteOption
    {
        public int? Percentage { get; set; }

        public List<BackupRouteOption> Backups { get; set; }
    }

    public class BackupRouteOption : BaseRouteOption
    {
    }
}