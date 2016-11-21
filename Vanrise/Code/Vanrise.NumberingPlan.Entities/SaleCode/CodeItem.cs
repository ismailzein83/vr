using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public enum CodeItemDraftStatus
    {
        [Description("Existing Not Changed")]
        ExistingNotChanged = 0,
        [Description("New")]
        New = 1,
        [Description("Existing Closed")]
        ExistingClosed = 2,
        [Description("Moved From")]
        MovedFrom = 3,
        [Description("Moved To")]
        MovedTo = 4,
        [Description("Code in Closed Zone")]
        ClosedZoneCode = 5
    }


    public enum CodeItemStatus
    {
        [Description("Pending Effective")]
        PendingEffective = 0,

        [Description("Pending Closed")]
        PendingClosed = 1
    }

    public class CodeItem
    {
        public long? CodeId { get; set; }

        public string Message { get; set; }
        public string Code { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }

        public CodeItemDraftStatus DraftStatus { get; set; }

        public CodeItemStatus? Status { get; set; }

        /// <summary>
        /// in case the Code is moved, this property stores the Zone Name of the other code. the other code is the existing code if this is the new one and vice versa
        /// </summary>
        public string OtherCodeZoneName { get; set; }
    }
}
