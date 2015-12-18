using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ZoneRouteOptionSetter
    {
        public IEnumerable<RPRouteDetail> _rpRoutes;
        public IEnumerable<CostCalculationMethod> _methods;

        public ZoneRouteOptionSetter(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, IEnumerable<CostCalculationMethod> methods)
        {
            RPRouteManager rpRouteManager = new RPRouteManager();
            _rpRoutes = rpRouteManager.GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones);
            _methods = methods;
        }

        public void SetZoneRouteOptionsAndCosts(IEnumerable<ZoneItem> zoneItems)
        {
            if (_rpRoutes == null)
                return;

            foreach (RPRouteDetail rpRoute in _rpRoutes) // The continue statement is used for readability
            {
                if (rpRoute.RouteOptionsDetails == null)
                    continue;

                ZoneItem zoneItem = zoneItems.FindRecord(itm => itm.ZoneId == rpRoute.SaleZoneId);

                if (zoneItem == null)
                    continue;

                zoneItem.RouteOptions = rpRoute.RouteOptionsDetails;

                if (_methods != null)
                {
                    zoneItem.Costs = new List<decimal>();

                    foreach (CostCalculationMethod method in _methods)
                    {
                        CostCalculationMethodContext context = new CostCalculationMethodContext() { Route = rpRoute };
                        method.CalculateCost(context);
                        zoneItem.Costs.Add(context.Cost);
                    }
                }
            } // foreach
        }
    }
}
