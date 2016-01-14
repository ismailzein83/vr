﻿using System;
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
        public IEnumerable<RPRouteDetail> _routes;
        public List<CostCalculationMethod> _costCalculationMethods;
        public int? _rateCalculationCostColumnConfigId;
        public RateCalculationMethod _rateCalculationMethod;

        public ZoneRouteOptionSetter(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, List<CostCalculationMethod> costCalculationMethods, int? rateCalculationCostColumnConfigId, RateCalculationMethod rateCalculationMethod)
        {
            RPRouteManager rpRouteManager = new RPRouteManager();
            _routes = rpRouteManager.GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones);
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
                var route = _routes.FindRecord(itm => itm.SaleZoneId == zoneItem.ZoneId);

                if (route != null)
                {
                    zoneItem.RouteOptions = route.RouteOptionsDetails;
                    SetZoneCostsAndCalculatedRate(zoneItem, route);
                }
                else if (_costCalculationMethods != null)
                {
                    zoneItem.Costs = new List<decimal?>();

                    foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                    {
                        zoneItem.Costs.Add(null);
                    }
                }
            }
        }

        void SetZoneCostsAndCalculatedRate(ZoneItem zoneItem, RPRouteDetail route)
        {
            if (_costCalculationMethods != null)
            {
                zoneItem.Costs = new List<decimal?>();

                foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                {
                    CostCalculationMethodContext context = new CostCalculationMethodContext() { Route = route };
                    costCalculationMethod.CalculateCost(context);
                    zoneItem.Costs.Add(context.Cost);
                }

                SetCalculatedRate(zoneItem);
            }
        }

        void SetCalculatedRate(ZoneItem zoneItem)
        {
            if (_rateCalculationMethod != null && zoneItem.Costs.Count > 0)
            {
                CostCalculationMethod costCalculationMethod = null;

                if (_rateCalculationCostColumnConfigId != null)
                    costCalculationMethod = _costCalculationMethods.FindRecord(itm => itm.ConfigId == (int)_rateCalculationCostColumnConfigId);

                if (costCalculationMethod != null)
                {
                    int index = _costCalculationMethods.IndexOf(costCalculationMethod);
                    RateCalculationMethodContext context = new RateCalculationMethodContext() { Cost = zoneItem.Costs[index] };

                    _rateCalculationMethod.CalculateRate(context);
                    zoneItem.CalculatedRate = context.Rate;
                }
            }
        }
    }
}
