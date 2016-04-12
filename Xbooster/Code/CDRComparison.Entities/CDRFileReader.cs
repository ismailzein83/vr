using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public abstract class CDRFileReader
    {
        public int ConfigId { get; set; }

        public abstract void ReadCDRs(IReadCDRsFromFileContext context);

        public abstract CDRSample ReadSample(IReadSampleFromFileContext context);
    }
}
