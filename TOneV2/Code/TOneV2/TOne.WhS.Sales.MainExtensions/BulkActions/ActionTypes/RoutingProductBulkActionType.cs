using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class RoutingProductBulkActionType : BulkActionType
	{
		#region Fields

		private RoutingProductManager _routingProductManager = new RoutingProductManager();
		private RoutingProductZoneRelationType? _rpZoneRelationType;
		private IEnumerable<long> _rpZoneIds;
		private int? _sellingProductId;

		#endregion

		public override Guid ConfigId
		{
			get { return new Guid("67D0BD5E-8B7A-407E-B03B-5FAE05F10A01"); }
		}

		public int RoutingProductId { get; set; }

		public bool ApplyNewNormalRateBED { get; set; }

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			if (context.OwnerType == SalePriceListOwnerType.Customer)
			{
				if (!_sellingProductId.HasValue)
				{
					_sellingProductId = new RatePlanManager().GetSellingProductId(context.OwnerId, DateTime.Today, false);
				}
				if (UtilitiesManager.CustomerZoneHasPendingClosedNormalRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId, context.GetCustomerZoneRate))
					return false;
			}

			if (!_rpZoneRelationType.HasValue)
				SetRoutingProductFields();

			return (_rpZoneRelationType.Value == RoutingProductZoneRelationType.SpecificZones) ? _rpZoneIds.Contains(context.SaleZone.SaleZoneId) : true;
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			context.ZoneItem.NewRoutingProduct = new DraftNewSaleZoneRoutingProduct()
			{
				ZoneId = context.ZoneItem.ZoneId,
				ZoneRoutingProductId = RoutingProductId,
				BED = DateTime.Today,
				ApplyNewNormalRateBED = ApplyNewNormalRateBED
			};

			if (ApplyNewNormalRateBED)
			{
				DateTime? newNormalRateBED = GetZoneNewNormalRateBED(context.ZoneDraft);
				if (newNormalRateBED.HasValue)
					context.ZoneItem.NewRoutingProduct.BED = newNormalRateBED.Value;
			}

			context.ZoneItem.EffectiveRoutingProductId = RoutingProductId;
			context.ZoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(RoutingProductId);

			context.ZoneItem.EffectiveServiceIds = _routingProductManager.GetZoneServiceIds(RoutingProductId, context.ZoneItem.ZoneId);
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			context.ZoneDraft.NewRoutingProduct = new DraftNewSaleZoneRoutingProduct()
			{
				ZoneId = context.ZoneDraft.ZoneId,
				ZoneRoutingProductId = RoutingProductId,
				BED = DateTime.Today,
				ApplyNewNormalRateBED = ApplyNewNormalRateBED
			};

			if (ApplyNewNormalRateBED)
			{
				DateTime? newNormalRateBED = GetZoneNewNormalRateBED(context.ZoneDraft);
				if (newNormalRateBED.HasValue)
					context.ZoneDraft.NewRoutingProduct.BED = newNormalRateBED.Value;
			}
		}

		#region Private Methods

		private void SetRoutingProductFields()
		{
			RoutingProduct routingProduct = _routingProductManager.GetRoutingProduct(RoutingProductId);
			if (routingProduct == null)
				throw new NullReferenceException("routingProduct");
			if (routingProduct.Settings == null)
				throw new NullReferenceException("routingProduct.Settings");

			_rpZoneRelationType = routingProduct.Settings.ZoneRelationType;

			if (routingProduct.Settings.ZoneRelationType == RoutingProductZoneRelationType.SpecificZones)
			{
				HashSet<long> rpZoneIds = _routingProductManager.GetFilteredZoneIds(RoutingProductId);
				if (rpZoneIds == null || rpZoneIds.Count == 0)
					throw new Exception("rpZoneIds");
				_rpZoneIds = rpZoneIds;
			}
		}

		private DateTime? GetZoneNewNormalRateBED(ZoneChanges zoneDraft)
		{
			if (zoneDraft != null && zoneDraft.NewRates != null)
			{
				DraftRateToChange newNormalRate = zoneDraft.NewRates.FindRecord(x => !x.RateTypeId.HasValue);
				if (newNormalRate != null)
					return newNormalRate.BED;
			}
			return null;
		}

		#endregion
	}
}
