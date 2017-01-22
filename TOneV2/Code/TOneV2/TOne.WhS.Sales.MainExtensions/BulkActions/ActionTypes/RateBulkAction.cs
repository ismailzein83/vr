using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
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
			// The zone is applicable if no new normal rate exists
			return (context.ZoneDraft == null || context.ZoneDraft.NewRates == null || !context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue));
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

			decimal? cost = null;
			int? costCalculationMethodIndex = null;

			if (CostCalculationMethod != null)
				costCalculationMethodIndex = context.GetCostCalculationMethodIndex(CostCalculationMethod.ConfigId);

			if (costCalculationMethodIndex.HasValue)
			{
				ZoneItem zoneItem = context.GetContextZoneItem(context.ZoneItem.ZoneId);
				if (zoneItem == null)
					throw new Vanrise.Entities.DataIntegrityValidationException("zoneItem");
				if (zoneItem.Costs != null)
					cost = zoneItem.Costs.ElementAt(costCalculationMethodIndex.Value);
			}

			var rateCalculationContext = new RateCalculationMethodContext()
			{
				Cost = cost
			};
			RateCalculationMethod.CalculateRate(rateCalculationContext);

			if (rateCalculationContext.Rate.HasValue)
			{
				newNormalRate.Rate = GetRoundedRate(rateCalculationContext.Rate.Value);
				newRates.Add(newNormalRate);
				context.ZoneItem.NewRates = newRates;
			}
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			ZoneItem zoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);

			decimal? cost = null;
			int? costCalculationMethodIndex = null;

			if (CostCalculationMethod != null)
				costCalculationMethodIndex = context.GetCostCalculationMethodIndex(CostCalculationMethod.ConfigId);

			if (costCalculationMethodIndex.HasValue)
			{
				if (zoneItem.Costs != null)
					cost = zoneItem.Costs.ElementAt(costCalculationMethodIndex.Value);
			}

			var rateCalculationContext = new RateCalculationMethodContext()
			{
				Cost = cost
			};
			RateCalculationMethod.CalculateRate(rateCalculationContext);

			if (rateCalculationContext.Rate.HasValue)
			{
				var newRates = new List<DraftRateToChange>();

				if (context.ZoneDraft != null && context.ZoneDraft.NewRates != null)
				{
					IEnumerable<DraftRateToChange> newOtherRates = context.ZoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
					newRates.AddRange(newOtherRates);
				}

				var newNormalRate = new DraftRateToChange()
				{
					ZoneId = zoneItem.ZoneId,
					RateTypeId = null,
					Rate = GetRoundedRate(rateCalculationContext.Rate.Value),
					BED = BED
				};

				newRates.Add(newNormalRate);
				context.ZoneDraft.NewRates = newRates;
			}
		}

		private decimal GetRoundedRate(decimal rate)
		{
			return decimal.Round(rate, 4);
		}
	}
}
