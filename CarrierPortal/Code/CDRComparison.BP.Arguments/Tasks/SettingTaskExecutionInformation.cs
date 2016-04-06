using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace CDRComparison.BP.Arguments
{
    public class SettingTaskExecutionInformation : BPTaskExecutionInformation
    {
        public bool Decision { get; set; }
        public long DurationMargin { get; set; }
        public TimeUnitEnum DurationMarginTimeUnit { get; set; }
        public long TimeMargin { get; set; }
        public TimeUnitEnum TimeMarginTimeUnit { get; set; }
        public TimeSpan TimeOffset { get; set; }
    }
}
