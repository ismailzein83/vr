using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDRSourceProfileSettings
    {
        public CDRSource Source { get; set; }

        public CDRNormalizationSettings NormalizationSettings { get; set; }

        public string DateTimeFormat { get; set; }

        public TimeSpan TimeZoneOffset { get; set; }
    }
}
