using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.CDRProcess.Arguments
{
    public class DailyRepricingProcessInput
    {
        public DateTime RepricingDay { get; set; }

        public bool DivideProcessIntoSubProcesses { get; set; }
    }
}
