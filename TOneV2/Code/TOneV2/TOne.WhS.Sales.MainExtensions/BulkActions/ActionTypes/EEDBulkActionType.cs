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
	public class EEDBulkActionType : BulkActionType
	{
		private int? _sellingProductId = null;

		public override Guid ConfigId
		{
			get { return new Guid("736034AB-115F-464B-919D-052EBFDEDD5C"); }
		}

		public DateTime EED { get; set; }

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			if (context.OwnerType == SalePriceListOwnerType.SellingProduct)
				throw new Vanrise.Entities.DataIntegrityValidationException("The EED BulkAction cannot be applied to a SellingProduct Zone");

			if (context.SaleZone.BED > EED || context.SaleZone.EED.HasValue)
				return false;

			if (context.ZoneDraft != null && context.ZoneDraft.NewRates != null && context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue))
				return false;

			DateTime effectiveOn = DateTime.Today;
			Dictionary<int, DateTime> datesByCountry = UtilitiesManager.GetDatesByCountry(context.OwnerId, effectiveOn, false);
			
			DateTime soldOn;
			if (!datesByCountry.TryGetValue(context.SaleZone.CountryId, out soldOn))
				return false;
			if (soldOn > EED)
				return false;

			if (!_sellingProductId.HasValue)
			{
				_sellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(context.OwnerId, effectiveOn, false);
				if (!_sellingProductId.HasValue)
				{
					string errorMessage = string.Format("Customer '{0}' is not assigned to a SellingProduct on '{1}'", context.OwnerId, effectiveOn.ToShortDateString());
					throw new Vanrise.Entities.DataIntegrityValidationException(errorMessage);
				}
			}

			SaleEntityZoneRate customerZoneRate = context.GetCustomerZoneRate(context.OwnerId, _sellingProductId.Value, context.SaleZone.SaleZoneId);
			if (customerZoneRate == null || customerZoneRate.Rate == null)
				return false;
			else if (customerZoneRate.Rate.BED > EED)
				return false;

			SaleEntityZoneRate sellingProductZoneRate = context.GetSellingProductZoneRate(_sellingProductId.Value, context.SaleZone.SaleZoneId);
			if (sellingProductZoneRate == null || sellingProductZoneRate.Rate == null)
				return false;
			else if (sellingProductZoneRate.Rate.BED > EED)
				return false;

			return true;
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			var closedRates = new List<DraftRateToClose>();

			if (context.ZoneDraft != null && context.ZoneDraft.ClosedRates != null)
			{
				IEnumerable<DraftRateToClose> closedOtherRates = context.ZoneDraft.ClosedRates.FindAllRecords(x => x.RateTypeId.HasValue);
				closedRates.AddRange(closedOtherRates);
			}

			var closedNormalRate = new DraftRateToClose()
			{
				ZoneId = context.ZoneItem.ZoneId,
				RateTypeId = null,
				EED = EED
			};

			closedRates.Add(closedNormalRate);
			context.ZoneItem.ClosedRates = closedRates;
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			var closedRates = new List<DraftRateToClose>();

			if (context.ZoneDraft.ClosedRates != null)
			{
				IEnumerable<DraftRateToClose> closedOtherRates = context.ZoneDraft.ClosedRates.FindAllRecords(x => x.RateTypeId.HasValue);
				closedRates.AddRange(closedOtherRates);
			}

			var closedNormalRate = new DraftRateToClose()
			{
				ZoneId = context.ZoneDraft.ZoneId,
				RateTypeId = null,
				EED = EED
			};

			closedRates.Add(closedNormalRate);
			context.ZoneDraft.ClosedRates = closedRates;
		}
	}
}
