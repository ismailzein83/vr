using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class BEDBulkActionType : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("310EAF9D-68B5-466A-9CC4-96121B03A5FD"); }
		}

		public DateTime BED { get; set; }

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			// Check if a new normal rate exists
			return (context.ZoneDraft != null && context.ZoneDraft.NewRates != null && context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue));
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
			newNormalRate.BED = BED;
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			DraftRateToChange newNormalRate = GetZoneNewNormalRate(context.ZoneDraft);
			newNormalRate.BED = BED;
		}

		private DraftRateToChange GetZoneNewNormalRate(ZoneChanges zoneDraft)
		{
			return zoneDraft.NewRates.FindRecord(x => !x.RateTypeId.HasValue);
		}
	}
}
