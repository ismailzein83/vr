using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public static class ContextExtensions
    {
        public static BPSharedInstanceData GetSharedInstanceData(this ActivityContext context)
        {
            var sharedData = context.GetExtension<BPSharedInstanceData>();
            if (sharedData == null)
                throw new NullReferenceException("BPSharedInstanceData");
            return sharedData;
        }
    }
}
