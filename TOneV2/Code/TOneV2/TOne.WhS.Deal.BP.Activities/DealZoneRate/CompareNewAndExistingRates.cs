using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
	public class CompareNewAndExistingRatesInput
	{
		public Boolean IsSale { get; set; }

		public DealZoneRateByZoneGroup NewDealZoneRateByZoneGroup { get; set; }

		public IEnumerable<int> DealIdsToReevaluate { get; set; }
	}

	public class CompareNewAndExistingRatesOutput
	{
		public Dictionary<int, HashSet<DateTime>> DaysToReProcessByDealId { get; set; }
	}

	public sealed class CompareNewAndExistingRates : BaseAsyncActivity<CompareNewAndExistingRatesInput, CompareNewAndExistingRatesOutput>
	{
		[RequiredArgument]
		public InArgument<Boolean> IsSale { get; set; }

		[RequiredArgument]
		public InArgument<DealZoneRateByZoneGroup> NewDealZoneRateByZoneGroup { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<int>> DealIdsToReevaluate { get; set; }

		[RequiredArgument]
		public OutArgument<Dictionary<int, HashSet<DateTime>>> DaysToReProcessByDealId { get; set; }

		protected override CompareNewAndExistingRatesOutput DoWorkWithResult(CompareNewAndExistingRatesInput inputArgument, AsyncActivityHandle handle)
		{
			var isSale = inputArgument.IsSale;
			var newDealZoneRatesByZoneGroup = inputArgument.NewDealZoneRateByZoneGroup;
			var dealIdsToReevaluate = inputArgument.DealIdsToReevaluate;
			var dealZoneRateManager = new DealZoneRateManager();
			Dictionary<int, HashSet<DateTime>> daysToReProcessByDealIds = new Dictionary<int, HashSet<DateTime>>();

			var existingDealZoneRatesByZoneGroup = dealZoneRateManager.GetAllDealZoneRatesByDealIds(isSale, dealIdsToReevaluate);

			if (newDealZoneRatesByZoneGroup != null)
			{
				foreach (var newZoneGroupDealZoneRates in newDealZoneRatesByZoneGroup)
				{
					DealZoneRateByZoneId existingZoneGroupDealZoneRates;

					if (existingDealZoneRatesByZoneGroup == null || !existingDealZoneRatesByZoneGroup.TryGetValue(newZoneGroupDealZoneRates.Key, out existingZoneGroupDealZoneRates)) //ZoneGroup found in new data and not in the existing data
					{
						var daysToReProcess = GetRangeOfDaysForZoneGroupRates(newZoneGroupDealZoneRates.Value);
						if (daysToReProcess != null && daysToReProcess.Count() > 0)
						{
							var daysToReProcessByDealId = daysToReProcessByDealIds.GetOrCreateItem(newZoneGroupDealZoneRates.Key.DealId);
							daysToReProcessByDealId.UnionWith(daysToReProcess);
						}
					}
					else //ZoneGroup found in new data and in the existing data
					{
						var daysToReProcess = GetRangeOfDaysByComparingTwoZoneGroupRates(newZoneGroupDealZoneRates.Value, existingZoneGroupDealZoneRates);
						if (daysToReProcess != null && daysToReProcess.Count() > 0)
						{
							var daysToReProcessByDealId = daysToReProcessByDealIds.GetOrCreateItem(newZoneGroupDealZoneRates.Key.DealId);
							daysToReProcessByDealId.UnionWith(daysToReProcess);
						}
					}
				}
			}

			if (existingDealZoneRatesByZoneGroup != null)
			{
				foreach (var existingZoneGroupDealZoneRates in existingDealZoneRatesByZoneGroup)
				{
					if (newDealZoneRatesByZoneGroup == null || !newDealZoneRatesByZoneGroup.ContainsKey(existingZoneGroupDealZoneRates.Key)) //ZoneGroup found in existing data and not in the new data
					{
						var daysToReProcess = GetRangeOfDaysForZoneGroupRates(existingZoneGroupDealZoneRates.Value);
						if (daysToReProcess != null && daysToReProcess.Count() > 0)
						{
							var daysToReProcessByDealId = daysToReProcessByDealIds.GetOrCreateItem(existingZoneGroupDealZoneRates.Key.DealId);
							daysToReProcessByDealId.UnionWith(daysToReProcess);
						}
					}
				}
			}

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Comparing new and existing deal zone rates is done.");

			return new CompareNewAndExistingRatesOutput()
			{
				DaysToReProcessByDealId = daysToReProcessByDealIds
			};
		}

		protected override CompareNewAndExistingRatesInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new CompareNewAndExistingRatesInput
			{
				IsSale = this.IsSale.Get(context),
				NewDealZoneRateByZoneGroup = this.NewDealZoneRateByZoneGroup.Get(context),
				DealIdsToReevaluate = this.DealIdsToReevaluate.Get(context)
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, CompareNewAndExistingRatesOutput result)
		{
			context.SetValue(this.DaysToReProcessByDealId, result.DaysToReProcessByDealId);
		}

		private IEnumerable<DateTime> GetRangeOfDaysByComparingTwoZoneGroupRates(DealZoneRateByZoneId newZoneGroupDealZoneRates, DealZoneRateByZoneId existingZoneGroupDealZoneRates)
		{
			List<DateTime> days = new List<DateTime>();
			foreach (var newZoneDealZoneRates in newZoneGroupDealZoneRates)
			{
				DealZoneRateByTireNB existingZoneDealZoneRates;

				if (!existingZoneGroupDealZoneRates.TryGetValue(newZoneDealZoneRates.Key, out existingZoneDealZoneRates)) //Zone found in new data and not in the existing data
					days.AddRange(GetRangeOfDaysForZoneRates(newZoneDealZoneRates.Value));
				else //Zone found in new data and in the existing data
					days.AddRange(GetRangeOfDaysByComparingTwoZoneRates(newZoneDealZoneRates.Value, existingZoneDealZoneRates));
			}

			foreach (var existingZoneRates in existingZoneGroupDealZoneRates)
			{
				if (!newZoneGroupDealZoneRates.ContainsKey(existingZoneRates.Key)) //Zone found in existing data and not in the new data
					days.AddRange(GetRangeOfDaysForZoneRates(existingZoneRates.Value));
			}
			return days;
		}

		private IEnumerable<DateTime> GetRangeOfDaysByComparingTwoZoneRates(DealZoneRateByTireNB newZoneDealZoneRates, DealZoneRateByTireNB existingZoneDealZoneRates)
		{
			List<DateTime> days = new List<DateTime>();
			foreach (var newTierDealZoneRates in newZoneDealZoneRates)
			{
				List<DealZoneRate> existingTierDealZoneRates;

				if (!existingZoneDealZoneRates.TryGetValue(newTierDealZoneRates.Key, out existingTierDealZoneRates)) //ZoneTier found in new data and not in the existing data
					days.AddRange(GetRangeOfDaysForListOfRates(newTierDealZoneRates.Value));
				else //ZoneTier found in new data and in the existing data
					days.AddRange(GetRangeOfDaysByComparingTwoLists(newTierDealZoneRates.Value, existingTierDealZoneRates));

			}

			foreach (var existingTierRates in existingZoneDealZoneRates)
			{
				if (!newZoneDealZoneRates.ContainsKey(existingTierRates.Key)) //ZoneTier found in existing data and not in the new data
					days.AddRange(GetRangeOfDaysForListOfRates(existingTierRates.Value));
			}
			return days;
		}

		private IEnumerable<DateTime> GetRangeOfDaysByComparingTwoLists(List<DealZoneRate> newRates, List<DealZoneRate> existingRates)
		{
			var days = new List<DateTime>();
			IEnumerable<VRDateTimeRange> dateRanges = Utilities.GenerateDateRanges<DealZoneRate, DealZoneRate>(newRates.ToList(), existingRates.ToList(), shouldAddRange);
			foreach (var dateRange in dateRanges)
			{
				days.AddRange(GetRangeOfDaysBetweenTwoDates(dateRange.From, dateRange.To));
			}
			return days;
		}

		Func<DealZoneRate, DealZoneRate, bool> shouldAddRange = (newRate, existingRate) =>
	   {
		   if (newRate == null && existingRate == null)
			   return false;
		   if (newRate == null || existingRate == null)
			   return true;
		   if (newRate.Rate != existingRate.Rate || newRate.CurrencyId != existingRate.CurrencyId)
			   return true;
		   return false;
	   };

		private IEnumerable<DateTime> GetRangeOfDaysForZoneGroupRates(DealZoneRateByZoneId dealZoneRateByZoneId)
		{
			var days = new List<DateTime>();

			if (dealZoneRateByZoneId != null)
			{
				foreach (var zoneDealZoneRates in dealZoneRateByZoneId)
					days.AddRange(GetRangeOfDaysForZoneRates(zoneDealZoneRates.Value));
			}
			return days;
		}

		private IEnumerable<DateTime> GetRangeOfDaysForZoneRates(DealZoneRateByTireNB dealZoneRateByTireNB)
		{
			var days = new List<DateTime>();

			if (dealZoneRateByTireNB != null)
			{
				foreach (var tierDealZoneRates in dealZoneRateByTireNB)
					days.AddRange(GetRangeOfDaysForListOfRates(tierDealZoneRates.Value));
			}
			return days;
		}

		private IEnumerable<DateTime> GetRangeOfDaysForListOfRates(IEnumerable<DealZoneRate> rates)
		{
			var days = new List<DateTime>();

			if (rates != null)
			{
				foreach (var rate in rates)
					days.AddRange(GetRangeOfDaysBetweenTwoDates(rate.BED, rate.EED));
			}
			return days;
		}

		private IEnumerable<DateTime> GetRangeOfDaysBetweenTwoDates(DateTime from, DateTime? to)
		{
			var days = new List<DateTime>();

			if (to.VRGreaterThan(DateTime.Today))
				to = DateTime.Today;

			from = from.Date;
			to = to.Value.Date;
			for (var day = from; day <= to; day = day.AddDays(1))
				days.Add(day);

			return days;
		}
	}
}