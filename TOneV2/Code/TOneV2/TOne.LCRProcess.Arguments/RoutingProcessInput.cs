using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCRProcess.Arguments
{
    public class RoutingProcessInput
    {
        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public bool IsLcrOnly { get; set; }
    }
}
