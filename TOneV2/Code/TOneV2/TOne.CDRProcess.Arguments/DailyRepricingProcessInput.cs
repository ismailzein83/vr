using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.CDRProcess.Arguments
{
    public class DailyRepricingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime RepricingDay { get; set; }

        public bool DivideProcessIntoSubProcesses { get; set; }

        public override string GetTitle()
        {
            return String.Format("Daily Repricing Process for date {0:dd-MMM-yyyy}", this.RepricingDay);
        }
    }
}
