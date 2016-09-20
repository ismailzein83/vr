using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public abstract class CDRFileReader
    {
        public abstract Guid ConfigId { get; }

        public abstract void ReadCDRs(IReadCDRsFromFileContext context);

        public abstract CDRSample ReadSample(IReadSampleFromFileContext context);
    }
}
