﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
			if (!_sellingProductId.HasValue)
			{
				DateTime effectiveOn = DateTime.Today;
				_sellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(context.OwnerId, effectiveOn, false);
				if (!_sellingProductId.HasValue)
				{
					string errorMessage = string.Format("Customer '{0}' is not assigned to a SellingProduct on '{1}'", context.OwnerId, effectiveOn.ToShortDateString());
					throw new Vanrise.Entities.DataIntegrityValidationException(errorMessage);
				}
			}

			SaleEntityZoneRate customerZoneRate = context.GetCustomerZoneRate(context.OwnerId, _sellingProductId.Value, context.ZoneId);
			if (customerZoneRate == null || customerZoneRate.Rate == null)
				return false;

			SaleEntityZoneRate sellingProductZoneRate = context.GetSellingProductZoneRate(_sellingProductId.Value, context.ZoneId);
			if (sellingProductZoneRate == null || sellingProductZoneRate.Rate == null)
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
