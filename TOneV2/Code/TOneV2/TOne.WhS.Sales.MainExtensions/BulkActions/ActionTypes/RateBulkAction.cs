using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class RateBulkAction : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("A893F3C6-D4BF-4C60-BA7D-2A773791D7BD"); }
		}

		public CostCalculationMethod CostCalculationMethod { get; set; }

		public RateCalculationMethod RateCalculationMethod { get; set; }

		public DateTime BED { get; set; }

		public bool OverwriteDraftNewNormalRate { get; set; }

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			if (OverwriteDraftNewNormalRate)
				return true;
			return (context.ZoneDraft.NewRates == null || !context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue)); // The zone is applicable if no new normal rate exists
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			var newRates = new List<DraftRateToChange>();

			if (context.ZoneDraft != null && context.ZoneDraft.NewRates != null)
			{
				IEnumerable<DraftRateToChange> newOtherRates = context.ZoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
				newRates.AddRange(newOtherRates);
			}

			var newNormalRate = new DraftRateToChange()
			{
				ZoneId = context.ZoneItem.ZoneId,
				RateTypeId = null,
				BED = BED
			};

			var costCalculationContext = new CostCalculationMethodContext() { Route = null };
			CostCalculationMethod.CalculateCost(costCalculationContext);

			var rateCalculationContext = new RateCalculationMethodContext()
			{
				Cost = 1.5m //Cost = costCalculationContext.Cost
			};
			RateCalculationMethod.CalculateRate(rateCalculationContext);

			if (rateCalculationContext.Rate.HasValue)
				newNormalRate.Rate = rateCalculationContext.Rate.Value;

			newRates.Add(newNormalRate);
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			ZoneItem zoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);
		}
	}
}
