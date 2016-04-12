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
        public decimal DurationMargin { get; set; }
        public TimeUnitEnum DurationMarginTimeUnit { get; set; }
        public decimal TimeMargin { get; set; }
        public TimeUnitEnum TimeMarginTimeUnit { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public bool CompareCGPN  { get; set; }
    }
}
