using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiConvertedRoute : ConvertedRoute
    {
        public string CustomerId { get; set; }
        public string Code { get; set; }
        public string RouteCaseAsString { get; set; }
        public bool IsBlocked { get; set; }
    }
}