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
			if (rpZones != null && rpZones.Count() > 0)
				_routes = new RPRouteManager().GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones);

			_costCalculationMethods = costCalculationMethods;
			_rateCalculationCostColumnConfigId = rateCalculationCostColumnConfigId;
			_rateCalculationMethod = rateCalculationMethod;
		}

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

		void SetCosts(IEnumerable<long> zoneIds, ZoneItem zoneItem, RPRouteDetail route, List<object> customObjects)
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
