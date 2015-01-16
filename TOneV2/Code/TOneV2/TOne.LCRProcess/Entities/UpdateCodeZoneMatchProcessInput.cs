using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCRProcess
{
    public class UpdateCodeZoneMatchProcessInput
    {
        public bool IsFuture { get; set; }
        public DateTime CodeEffectiveOn { get; set; }
        public bool GetChangedCodeGroupsOnly { get; set; }
    }
}
