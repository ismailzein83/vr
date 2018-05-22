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

        private List<CostCalculationMethod> _costCalculationMethods;
        private Guid? _rateCalculationCostColumnConfigId;
        private RateCalculationMethod _rateCalculationMethod;

        private int _longPrecisionValue;
        private int _normalPrecisionValue;

        private Dictionary<long, RPRouteDetailByZone> _rpRoutesByZoneId = new Dictionary<long, RPRouteDetailByZone>();

        private int? _numberOfOptions;
        private bool _includeBlockedSuppliers;
        public ZoneRouteOptionManager(SalePriceListOwnerType ownerType, int ownerId, int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, List<CostCalculationMethod> costCalculationMethods, Guid? rateCalculationCostColumnConfigId, RateCalculationMethod rateCalculationMethod, int currencyId, int longPrecisionValue, int normalPrecisionValue, bool includeBlockedSuppliers)
        {
            if (rpZones != null && rpZones.Count() > 0)
            {
                int? customerId = null;
                if (ownerType == SalePriceListOwnerType.Customer)
                    customerId = ownerId;
                IEnumerable<RPRouteDetailByZone> routes = new RPRouteManager().GetRPRoutes(routingDatabaseId, policyConfigId, null, rpZones, currencyId, customerId, includeBlockedSuppliers);
                StructureRPRoutesByZoneId(routes);
            }

            _costCalculationMethods = costCalculationMethods;
            _rateCalculationCostColumnConfigId = rateCalculationCostColumnConfigId;
            _rateCalculationMethod = rateCalculationMethod;

            _longPrecisionValue = longPrecisionValue;
            _normalPrecisionValue = normalPrecisionValue;
            _numberOfOptions = numberOfOptions;
            _includeBlockedSuppliers = includeBlockedSuppliers;
        }

        #endregion

        // The route option properties of a zone item are: RouteOptions, Costs and CalculatedRate
        public void SetZoneRouteOptionProperties(IEnumerable<ZoneItem> zoneItems)
        {
            if (zoneItems == null)
                return;

            IEnumerable<long> zoneIds = zoneItems.MapRecords(x => x.ZoneId);

            var customObjects = new List<object>();
            var emptyCosts = new List<decimal?>(); // This list is used for display purposes

            if (_costCalculationMethods != null)
            {
                foreach (CostCalculationMethod costCalculationMethod in _costCalculationMethods)
                {
                    customObjects.Add(null);
                    emptyCosts.Add(null);
                }
            }

            foreach (ZoneItem zoneItem in zoneItems)
            {
                RPRouteDetailByZone route = _rpRoutesByZoneId.GetRecord(zoneItem.ZoneId);
                zoneItem.RouteOptionsDetailsForView = getRouteOptionDetailsForView(route, _numberOfOptions);
                if (_includeBlockedSuppliers && route != null && route.RouteOptionsDetails != null)
                    route.RouteOptionsDetails = route.RouteOptionsDetails.FindAllRecords(item => item.SupplierStatus != SupplierStatus.Block);
                zoneItem.RPRouteDetail = route;

                if (route != null && route.RouteOptionsDetails != null && route.RouteOptionsDetails.Count() > 0)
                {
                    SetCosts(zoneIds, zoneItem, route, customObjects);
                    SetZoneMarginProperties(zoneItem);
                }
                else if (_costCalculationMethods != null)
                    zoneItem.Costs = emptyCosts;
           }
        }

        #region Private Methods

        private void SetCosts(IEnumerable<long> zoneIds, ZoneItem zoneItem, RPRouteDetailByZone route, List<object> customObjects)
        {
            if (_costCalculationMethods == null)
                return;

            zoneItem.Costs = new List<decimal?>();

            for (int i = 0; i < _costCalculationMethods.Count; i++)
            {
                var context = new CostCalculationMethodContext() { ZoneIds = zoneIds, Route = route, CustomObject = customObjects[i], NumberOfOptions = _numberOfOptions };
                _costCalculationMethods[i].CalculateCost(context);
                customObjects[i] = context.CustomObject;
                zoneItem.Costs.Add(decimal.Round(context.Cost, _longPrecisionValue));
            }
        }

        private void SetZoneMarginProperties(ZoneItem zoneItem)
        {
            if (zoneItem.CurrentRate.HasValue)
            {
                decimal? firstSupplierRate = zoneItem.RPRouteDetail.RouteOptionsDetails.ElementAt(0).ConvertedSupplierRate;
                if (firstSupplierRate.HasValue)
                {
                    decimal margin = zoneItem.CurrentRate.Value - firstSupplierRate.Value;
                    zoneItem.Margin = decimal.Round(margin, _longPrecisionValue);
                    zoneItem.MarginPercentage = (firstSupplierRate.Value > 0) ? decimal.Round(((margin / firstSupplierRate.Value) * 100), _normalPrecisionValue) : 0;
                }
            }
        }

        private void StructureRPRoutesByZoneId(IEnumerable<RPRouteDetailByZone> rpRoutes)
        {
            if (rpRoutes == null || rpRoutes.Count() == 0)
                return;

            foreach (RPRouteDetailByZone rpRoute in rpRoutes)
            {
                if (!_rpRoutesByZoneId.ContainsKey(rpRoute.SaleZoneId))
                    _rpRoutesByZoneId.Add(rpRoute.SaleZoneId, rpRoute);
            }
        }

        #endregion

        private IEnumerable<RPRouteOptionDetail> getRouteOptionDetailsForView(RPRouteDetailByZone route, int? numberOfOptions)
        {
            int nbOfActiveRouteOptionDetails = 0;
            List<RPRouteOptionDetail> routeOptionDetailsResult = new List<RPRouteOptionDetail>();
            if (route != null && route.RouteOptionsDetails != null && route.RouteOptionsDetails.Count() > 0)
            {
                if (!numberOfOptions.HasValue)
                    return route.RouteOptionsDetails;
                int index = 0;
                while (index < route.RouteOptionsDetails.Count() && nbOfActiveRouteOptionDetails < numberOfOptions)
                {
                    var routeOptionsDetail = route.RouteOptionsDetails.ElementAt(index);
                    routeOptionDetailsResult.Add(routeOptionsDetail);
                    if (routeOptionsDetail.SupplierStatus != SupplierStatus.Block)
                        nbOfActiveRouteOptionDetails++;
                    index++;
                }
            }
            return routeOptionDetailsResult;
        }
    }
}
