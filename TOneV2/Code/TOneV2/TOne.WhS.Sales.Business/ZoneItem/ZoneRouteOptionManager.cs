using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class ZoneRouteOptionManager
    {
        private IEnumerable<RPRouteDetail> _routes;
        private List<CostCalculationMethod> _costCalculationMethods;
        private Guid? _rateCalculationCostColumnConfigId;
        private RateCalculationMethod _rateCalculationMethod;

        public ZoneRouteOptionManager(SalePriceListOwnerType ownerType, int ownerId, int routingDatabaseId, Guid policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, List<CostCalculationMethod> costCalculationMethods, Guid? rateCalculationCostColumnConfigId, RateCalculationMethod rateCalculationMethod, int currencyId)
        {
            RPRouteManager rpRouteManager = new RPRouteManager();

			int? customerId = null;
			if (ownerType == SalePriceListOwnerType.Customer)
				customerId = ownerId;
			_routes = rpRouteManager.GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, currencyId, customerId);
            
			_costCalculationMethods = costCalculationMethods;
            _rateCalculationCostColumnConfigId = rateCalculationCostColumnConfigId;
            _rateCalculationMethod = rateCalculationMethod;
        }

        // The route option properties of a zone item are: RouteOptions, Costs and CalculatedRate
        public void SetZoneRouteOptionProperties(IEnumerable<ZoneItem> zoneItems)
        {
            if (zoneItems == null)
                return;
            foreach (ZoneItem zoneItem in zoneItems)
            {
                var route = _routes.FindRecord(x => x.SaleZoneId == zoneItem.ZoneId);
                if (route != null)
                {
                    zoneItem.RouteOptions = route.RouteOptionsDetails;
                    SetCosts(zoneItem, route);
                }
                else if (_costCalculationMethods != null)
                {
                    zoneItem.Costs = new List<decimal?>();
                    foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                        zoneItem.Costs.Add(null);
                }
                SetCalculatedRate(zoneItem);
            }
        }

        void SetCosts(ZoneItem zoneItem, RPRouteDetail route)
        {
            if (_costCalculationMethods == null)
                return;

            zoneItem.Costs = new List<decimal?>();

            foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
            {
                var context = new CostCalculationMethodContext() { Route = route };
                costCalculationMethod.CalculateCost(context);
                zoneItem.Costs.Add(context.Cost);
            }
        }

        void SetCalculatedRate(ZoneItem zoneItem)
        {
            if (zoneItem.ZoneEED.HasValue || _rateCalculationMethod == null)
                return;

            decimal? cost = null;

            if (_rateCalculationCostColumnConfigId.HasValue)
            {
                if (_costCalculationMethods == null)
                    throw new NullReferenceException("costCalculationMethods");

                CostCalculationMethod costCalculationMethod = _costCalculationMethods.FindRecord(x => x.ConfigId == _rateCalculationCostColumnConfigId.Value);
                if (costCalculationMethod == null)
                    throw new NullReferenceException("costCalculationMethod");

                int costIndex = _costCalculationMethods.IndexOf(costCalculationMethod);
                cost = zoneItem.Costs[costIndex];
            }

            RateCalculationMethodContext context = new RateCalculationMethodContext() { Cost = cost };
            _rateCalculationMethod.CalculateRate(context);

            if (context.Rate.HasValue)
            {
                if (!zoneItem.CurrentRate.HasValue || zoneItem.CurrentRate.Value != context.Rate.Value)
                    zoneItem.CalculatedRate = Decimal.Round(context.Rate.Value, GenericParameterManager.Current.GetLongPrecision());
            }
        }
    }
}
