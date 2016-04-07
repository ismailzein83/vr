using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDRSourceConfig
    {
        public int CDRSourceConfigId { get; set; }
        public string Name { get; set; }
        public CDRSource CDRSource { get; set; }
        public SettingsTaskExecutionInfo SettingsTaskExecutionInfo { get; set; }
        public bool IsPartnerCDRSource { get; set; }
    }

    public class SettingsTaskExecutionInfo
    {
        public decimal DurationMargin { get; set; }
        public TimeUnitEnum DurationMarginTimeUnit { get; set; }
        public decimal TimeMargin { get; set; }
        public TimeUnitEnum TimeMarginTimeUnit { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public bool CompareCGPN { get; set; }
    }
}
