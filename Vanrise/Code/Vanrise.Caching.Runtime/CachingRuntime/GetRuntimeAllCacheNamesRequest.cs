using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching.Runtime
{
    public class GetRuntimeAllCacheNamesRequest :  Vanrise.Runtime.Entities.InterRuntimeServiceRequest<List<String>>
    {
        public override List<string> Execute()
        {
            return CachingRuntimeService.s_currentCacheFullNames.ToList();
        }
    }
}
