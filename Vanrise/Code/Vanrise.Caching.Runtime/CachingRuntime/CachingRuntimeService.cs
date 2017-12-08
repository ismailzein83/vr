using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;

namespace Vanrise.Caching.Runtime
{
    public class CachingRuntimeService : RuntimeService
    {
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Caching_CachingRuntimeService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        internal static HashSet<string> s_currentCacheFullNames = new HashSet<string>();
        //internal static bool s_isCurrent

        protected override void Execute()
        {            
        }

        
    }
}
