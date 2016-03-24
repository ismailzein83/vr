using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public abstract class CDRSource
    {
        public int ConfigId { get; set; }

        public abstract void ReadCDRs(IReadCDRsFromSourceContext context);

        public abstract CDRSample ReadSample(IReadSampleFromSourceContext context);
    }
}
