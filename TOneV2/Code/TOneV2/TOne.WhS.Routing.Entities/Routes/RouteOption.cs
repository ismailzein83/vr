using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class BaseRouteOption
    {
        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public bool IsBlocked { get; set; }

        public int? ExecutedRuleId { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int NumberOfTries { get; set; }
    }

    public class RouteOption : BaseRouteOption, IRouteOptionPercentageTarget
    {
        public int? Percentage { get; set; }

        public List<RouteBackupOption> Backups { get; set; }

        public bool IsFullyBlocked()
        {
            if (!this.IsBlocked)
                return false;

            if (this.Backups == null || this.Backups.Count == 0)
                return true;

            foreach (RouteBackupOption backup in this.Backups)
            {
                if (!backup.IsBlocked)
                    return false;
            }

            return true;
        }
    }

    public class RouteBackupOption : BaseRouteOption
    {

    }
}