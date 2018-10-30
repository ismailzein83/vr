using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiConvertedRoutesPayload
    {
        public HuaweiConvertedRoutesPayload()
        {
            this.ConvertedRoutes = new List<HuaweiConvertedRoute>();
        }

        public List<HuaweiConvertedRoute> ConvertedRoutes { get; set; }
    }
}
