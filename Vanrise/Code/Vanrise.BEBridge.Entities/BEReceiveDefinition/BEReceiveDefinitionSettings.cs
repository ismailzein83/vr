using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class BEReceiveDefinitionSettings
    {
        public SourceBEReader SourceBEReader { get; set; }

        public TargetBEConvertor TargetBEConvertor { get; set; }

        public TargetBESynchronizer TargetBESynchronizer { get; set; }
    }
}
