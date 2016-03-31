using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace CDRComparison.BP.Arguments
{
    public class ConfigurationTaskExecutionInformation : BPTaskExecutionInformation
    {
        public TimeSpan TimeOffset { get; set; }
        public decimal DurationMargin { get; set; }
        public int TimeMargin { get; set; }
        public bool Decision { get; set; }
    }
}
