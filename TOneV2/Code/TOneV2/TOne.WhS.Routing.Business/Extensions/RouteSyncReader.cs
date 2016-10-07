using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.Extensions
{
    public enum TOneRouteRangeType { ByCustomer = 0 }

    public class RouteSyncReader : RouteReader
    {
        public TOneRouteRangeType? RangeType { get; set; }

        public override bool TryGetReadRanges(IRouteReaderGetReadRangesContext context)
        {
            if (this.RangeType.HasValue)
            {
                switch(this.RangeType.Value)
                {
                    case TOneRouteRangeType.ByCustomer:
                        context.RangeType = RouteRangeType.ByCustomer;
                        context.Ranges = new BusinessEntity.Business.CarrierAccountManager().GetAllCustomers().Select(customer => new RouteRangeInfo { CustomerId = customer.CarrierAccountId.ToString() }).ToList();
                        break;
                }
                return true;
            }
            else
                return false;
        }

        public override void ReadRoutes(IRouteReaderContext context)
        {
            int? customerId = null;
            string codePrefix = null;
            GetCustomerAndCode(context, out customerId, out codePrefix);

            Action<CustomerRoute> onCustomerRouteLoaded = BuildCustomerRouteLoadedAction(context);

            CustomerRouteManager routeManager = new CustomerRouteManager();
            routeManager.LoadRoutesFromCurrentDB(customerId, codePrefix, onCustomerRouteLoaded);
        }

        private static Action<CustomerRoute> BuildCustomerRouteLoadedAction(IRouteReaderContext context)
        {
            Action<CustomerRoute> onCustomerRouteLoaded = (customerRoute) =>
            {
                Route route = new Route
                {
                    CustomerId = customerRoute.CustomerId.ToString(),
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
                            Percentage = customerRouteOption.Percentage
                        });
                    }
                }
                context.OnRouteReceived(route, null);
            };
            return onCustomerRouteLoaded;
        }

        private static void GetCustomerAndCode(IRouteReaderContext context, out int? customerId, out string codePrefix)
        {
             customerId = null;
            codePrefix = null;
            if (context.RouteRangeInfo != null)
            {
                var rangeInfo = context.RouteRangeInfo;
                if (rangeInfo.CustomerId != null)
                {
                    int customerId_local;
                    if (!int.TryParse(rangeInfo.CustomerId, out customerId_local))
                        throw new Exception(String.Format("Invalid Customer Id '{0}'", context.RouteRangeInfo.CustomerId));
                    customerId = customerId_local;
                }
                codePrefix = rangeInfo.CodePrefix;
            }
        }
    }
}
