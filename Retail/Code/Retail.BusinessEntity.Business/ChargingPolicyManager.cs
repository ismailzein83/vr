using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class ChargingPolicyManager
    {


        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
