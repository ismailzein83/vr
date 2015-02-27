using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public abstract class BaseRouteOptionFilter
    {
        public virtual RouteOptionFilterResult Execute(object filterData, RouteSupplierOption option, RouteDetail routeDetail)
        {
            return null;
        }
    }

    public class RouteOptionFilterResult
    {
        public bool RemoveOption { get; set; }

        public bool BlockOption { get; set; }

        public bool Notify { get; set; }
    }
}
