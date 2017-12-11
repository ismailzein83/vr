using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public static class Helper
    {
        public static Route BuildRouteFromCustomerRoute(CustomerRoute customerRoute)
        {
            Route route = new Route
            {
                CustomerId = customerRoute.CustomerId.ToString(),
                SaleZoneId = customerRoute.SaleZoneId,
                SaleRate = customerRoute.Rate,
                Code = customerRoute.Code
            };
            if (customerRoute.Options != null)
            {
                route.Options = new List<RouteSync.Entities.RouteOption>();
                foreach (var customerRouteOption in customerRoute.Options)
                {
                    if (customerRouteOption.IsBlocked)
                        continue;

                    route.Options.Add(new RouteSync.Entities.RouteOption
                    {
                        SupplierId = customerRouteOption.SupplierId.ToString(),
                        SupplierRate = customerRouteOption.SupplierRate,
                        Percentage = customerRouteOption.Percentage,
                        IsBlocked = customerRouteOption.IsBlocked,
                        NumberOfTries = customerRouteOption.NumberOfTries
                    });
                }
            }
            return route;
        }

        public static List<Route> BuildRoutesFromCustomerRoutes(IEnumerable<CustomerRoute> customerRoutes)
        {
            if (customerRoutes == null)
                return null;

            List<Route> routes = new List<Route>();
            foreach (CustomerRoute customerRoute in customerRoutes)
                routes.Add(BuildRouteFromCustomerRoute(customerRoute));

            return routes;
        }
    }
}
