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
        #region Fields / Constructors

        private IEnumerable<RPRouteDetail> _routes;
        private List<CostCalculationMethod> _costCalculationMethods;
        private Guid? _rateCalculationCostColumnConfigId;
        private RateCalculationMethod _rateCalculationMethod;

        public ZoneRouteOptionManager(SalePriceListOwnerType ownerType, int ownerId, int routingDatabaseId, Guid policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, List<CostCalculationMethod> costCalculationMethods, Guid? rateCalculationCostColumnConfigId, RateCalculationMethod rateCalculationMethod, int currencyId)
        {
            if (rpZones != null && rpZones.Count() > 0)
            {
                int? customerId = null;
                if (ownerType == SalePriceListOwnerType.Customer)
                    customerId = ownerId;
                _routes = new RPRouteManager().GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, currencyId, customerId);
            }

            _costCalculationMethods = costCalculationMethods;
            _rateCalculationCostColumnConfigId = rateCalculationCostColumnConfigId;
            _rateCalculationMethod = rateCalculationMethod;
        }

        #endregion

        // The route option properties of a zone item are: RouteOptions, Costs and CalculatedRate
        public void SetZoneRouteOptionProperties(IEnumerable<ZoneItem> zoneItems)
        {
            if (zoneItems == null)
                return;

            List<object> customObjects = new List<object>();

            foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                customObjects.Add(null);

            IEnumerable<long> zoneIds = zoneItems.Select(x => x.ZoneId);

            foreach (ZoneItem zoneItem in zoneItems)
            {
                RPRouteDetail route = _routes.FindRecord(x => x.SaleZoneId == zoneItem.ZoneId);
                zoneItem.RPRouteDetail = route;
                if (route != null && route.RouteOptionsDetails != null && route.RouteOptionsDetails.Count() > 0)
                {
                    SetCosts(zoneIds, zoneItem, route, customObjects);
                    SetZoneMarginProperties(zoneItem);
                }
                else if (_costCalculationMethods != null)
                {
                    zoneItem.Costs = new List<decimal?>();
                    foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                        zoneItem.Costs.Add(null);
                }
                //SetCalculatedRate(zoneItem);
            }
        }

        #region Private Methods

        private void SetCosts(IEnumerable<long> zoneIds, ZoneItem zoneItem, RPRouteDetail route, List<object> customObjects)
        {
            if (_costCalculationMethods == null)
                return;

            zoneItem.Costs = new List<decimal?>();

            for (int i = 0; i < _costCalculationMethods.Count; i++)
            {
                var context = new CostCalculationMethodContext() { ZoneIds = zoneIds, Route = route, CustomObject = customObjects[i] };
                _costCalculationMethods[i].CalculateCost(context);
                customObjects[i] = context.CustomObject;
                zoneItem.Costs.Add(context.Cost);
            }
        }

        private void SetZoneMarginProperties(ZoneItem zoneItem)
        {
            if (zoneItem.CurrentRate.HasValue)
            {
                decimal? firstSupplierRate = zoneItem.RPRouteDetail.RouteOptionsDetails.ElementAt(0).ConvertedSupplierRate;
                decimal margin = zoneItem.CurrentRate.Value - firstSupplierRate.Value;
                zoneItem.Margin = margin;
                zoneItem.MarginPercentage = (margin / firstSupplierRate.Value) * 100;
            }
        }

        #endregion
    }
}
