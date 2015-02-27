using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteOptionBuildContext : IRouteOptionBuildContext
    {
        RouteSupplierOption _routeOption;

        RouteBuildContext _parentContext;

        public RouteOptionBuildContext(RouteSupplierOption routeOption, RouteBuildContext parentContext)
        {
            _routeOption = routeOption;
            _parentContext = parentContext;
        }

        public RouteSupplierOption RouteOption
        {
            get
            {
                return _routeOption;
            }
        }

        public void BlockOption()
        {

        }

        public void RemoveOption()
        {

        }
    }
}
