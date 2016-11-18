using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class ConfigurationCacheManager : Vanrise.Caching.BaseCacheManager
    {
        protected override bool IsTimeExpirable
        {
            get { return true; }
        }
    }
}
