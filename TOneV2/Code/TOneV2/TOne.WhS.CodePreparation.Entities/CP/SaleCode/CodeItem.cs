using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public enum CodeItemStatus
    {
        [Description("Existing Not Changed")]
        ExistingNotChanged = 0,
        [Description("New")]
        New = 1,
        [Description("Existing Closed")]
        ExistingClosed = 2,
        [Description("Existing Moved")]
        ExistingMoved = 3,
        [Description("New Moved")]
        NewMoved = 4
    }

    public class CodeItem
    {
        public long? CodeId { get; set; }

        public string Message { get; set; }
        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public CodeItemStatus Status { get; set; }

        public string StatusDescription { get; set; }

        /// <summary>
        /// in case the Code is moved, this property stores the Zone Name of the other code. the other code is the existing code if this is the new one and vice versa
        /// </summary>
        public string OtherCodeZoneName { get; set; }
    }
}
