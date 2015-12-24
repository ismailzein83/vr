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
        public List<CostCalculationMethod> _costCalculationMethods;
        public RateCalculationMethod _rateCalculationMethod;

        public ZoneRouteOptionSetter(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, List<CostCalculationMethod> costCalculationMethods, RateCalculationMethod rateCalculationMethod)
        {
            RPRouteManager rpRouteManager = new RPRouteManager();
            _rpRoutes = rpRouteManager.GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones);
            _costCalculationMethods = costCalculationMethods;
            _rateCalculationMethod = rateCalculationMethod;
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

                if (_costCalculationMethods != null)
                {
                    zoneItem.Costs = new List<decimal>();

                    foreach (CostCalculationMethod method in _costCalculationMethods)
                    {
                        CostCalculationMethodContext context = new CostCalculationMethodContext() { Route = rpRoute };
                        method.CalculateCost(context);
                        zoneItem.Costs.Add(context.Cost);
                    }

                    SetCalculatedRate(zoneItem);
                }
            } // foreach
        }

        void SetCalculatedRate(ZoneItem zoneItem)
        {
            if (_rateCalculationMethod != null)
            {
                CostCalculationMethod costCalculationMethod = _costCalculationMethods.FindRecord(itm => itm.ConfigId == _rateCalculationMethod.ConfigId);

                if (costCalculationMethod != null)
                {
                    int index = _costCalculationMethods.IndexOf(costCalculationMethod);
                    RateCalculationMethodContext context = new RateCalculationMethodContext();
                    context.Cost = zoneItem.Costs[index];
                    _rateCalculationMethod.CalculateRate(context);
                    zoneItem.CalculatedRate = context.Rate;
                }
            }
        }
    }
}
