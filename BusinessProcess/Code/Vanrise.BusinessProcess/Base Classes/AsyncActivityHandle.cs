using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class AsyncActivityHandle
    {
        public BPSharedInstanceData SharedInstanceData { get; internal set; }

        public Dictionary<string,object> CustomData { get; set; }
    }
}
