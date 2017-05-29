using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{
    public class TargetBESynchronizerInitializeContext : ITargetBESynchronizerInitializeContext
    {
        public object InitializationData
        {
            get;
            set;
        }
    }

    public class TargetBEConvertorInitializeContext : ITargetBEConvertorInitializeContext
    {
        public object InitializationData
        {
            get;
            set;
        }
    }
}
