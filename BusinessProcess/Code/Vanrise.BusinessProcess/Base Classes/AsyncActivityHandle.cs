using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class AsyncActivityHandle
    {
        public BPSharedInstanceData SharedInstanceData { get; internal set; }

        Dictionary<string, object> _customData;
        public Dictionary<string, object> CustomData
        {
            get
            {
                if (_customData == null)
                {
                    lock (this)
                    {
                        if (_customData == null)
                            _customData = new Dictionary<string, object>();
                    }
                }
                return _customData;
            }
        }
    }
}
