using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public enum ZoneItemDraftStatus
    {
        [Description("Existing Not Changed")]
        ExistingNotChanged = 0,
        [Description("New")]
        New = 1,
        [Description("Existing Closed")]
        ExistingClosed = 2,
        [Description("Renamed")]
        Renamed = 3
    }

    public enum ZoneItemStatus
    {
        [Description("Pending Closed")]
        PendingClosed = 0,
        [Description("Pending Effective")]
        PendingEffective = 1
    }

    public class ZoneItem
    {
        public long? ZoneId { get; set; }

        public string Name { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }

        public ZoneItemDraftStatus DraftStatus { get; set; }

        public ZoneItemStatus? Status { get; set; }

        public string Message { get; set; }

        public int CountryId { get; set; }
    }
}
