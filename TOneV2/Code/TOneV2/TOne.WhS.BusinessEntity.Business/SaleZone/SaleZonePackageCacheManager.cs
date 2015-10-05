using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZonePackageCacheManager : Vanrise.Caching.BaseCacheManager
    {
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return base.ShouldSetCacheExpired(parameter);
        }
    }
}
