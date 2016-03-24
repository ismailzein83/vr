using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class ReadSampleFromFileContext : ReadFromFileContext, IReadSampleFromFileContext
    {
        public ReadSampleFromFileContext(VRFile file) : base(file) { }
    }
}
