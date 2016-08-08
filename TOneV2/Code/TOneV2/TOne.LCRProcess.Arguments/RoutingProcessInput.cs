using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCRProcess.Arguments
{
    public class RoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public bool IsLcrOnly { get; set; }

        public bool DivideProcessIntoSubProcesses { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# for Effective Time {0}", EffectiveTime);
        }
    }
}
