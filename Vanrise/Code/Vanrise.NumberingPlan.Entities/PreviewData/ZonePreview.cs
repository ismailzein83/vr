using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public enum ZoneChangeType
    {
        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Renamed")]
        Renamed = 3,
        
        [Description("Pending Effective")]
        PendingEffective = 4,

        [Description("Pending Closed")]
        PendingClosed = 5,
    };


    public class ZonePreview
    {
        public int CountryId { get; set; }
        public string ZoneName { get; set; }
        public string RecentZoneName { get; set; }
        public ZoneChangeType ChangeTypeZone { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public int NewCodes { get; set; }
        public int DeletedCodes { get; set; }
        public int CodesMovedTo { get; set; }
        public int CodesMovedFrom { get; set; }
        public int PendingEffectiveCodes { get; set; }
        public int PendingClosedCodes { get; set; }

    }

}
