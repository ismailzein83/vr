using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching.Runtime
{
    internal class GetCachingRuntimeProcessIdRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<int?>
    {
        public string CacheFullName { get; set; }
        public override int? Execute()
        {
            if (CachingDistributor.s_current == null)
                throw new NullReferenceException("CachingDistributor.s_current");
            return CachingDistributor.s_current.GetRuntimeProcessId(this.CacheFullName);
        }
    }
}
