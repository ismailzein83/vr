using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BEBridge.Entities
{
    public class EntitySyncDefinition
    {
        public TargetBEConvertor TargetBEConvertor { get; set; }
        public TargetBESynchronizer TargetBESynchronizer { get; set; }
    }
}
