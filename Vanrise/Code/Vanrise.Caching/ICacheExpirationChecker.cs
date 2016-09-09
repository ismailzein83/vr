using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public interface ICacheExpirationChecker
    {
        bool TryCheckExpirationFromRuntimeService<ParamType>(Type cacheManagerType, ParamType parameter, out bool isExpired);
    }
}
