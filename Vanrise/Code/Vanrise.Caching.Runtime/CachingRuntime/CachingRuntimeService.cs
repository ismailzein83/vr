using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

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

        public override void Execute()
        {            
        }



        public override Guid ConfigId
        {
            get { return new Guid("A42F26F0-BC31-4A7E-A852-33DAF1B9250E"); }
        }
    }
}
