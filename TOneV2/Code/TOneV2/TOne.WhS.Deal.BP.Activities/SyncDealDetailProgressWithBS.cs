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
	public class SyncDealDetailProgressWithBSInput
	{
		public Boolean IsSale { get; set; }

		public DateTime BeginDate { get; set; }

		public Dictionary<DealZoneGroup, List<DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

		public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }
	}

	public class SyncDealDetailProgressWithBSOutput
	{
		public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }
	}

	public sealed class SyncDealDetailProgressWithBS : BaseAsyncActivity<SyncDealDetailProgressWithBSInput, SyncDealDetailProgressWithBSOutput>
	{
		[RequiredArgument]
		public InArgument<Boolean> IsSale { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> BeginDate { get; set; }

		[RequiredArgument]
		public InArgument<Dictionary<DealZoneGroup, List<DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

		[RequiredArgument]
		public InOutArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }


		protected override SyncDealDetailProgressWithBSOutput DoWorkWithResult(SyncDealDetailProgressWithBSInput inputArgument, AsyncActivityHandle handle)
		{
			HashSet<DealZoneGroup> dealZoneGroups;
			HashSet<DealZoneGroup> affectedDealZoneGroups;
			SyncDealDetailProgressWithBillingStats(inputArgument.CurrentDealBillingSummaryRecords, inputArgument.IsSale, inputArgument.BeginDate, out affectedDealZoneGroups);

			string isSaleAsString = inputArgument.IsSale ? "Sale" : "Cost";
			handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Synchronizing {0} Deal Detail Progress Table With BillingStats Table is done", isSaleAsString), null);

			if (inputArgument.AffectedDealZoneGroups == null)
				dealZoneGroups = affectedDealZoneGroups;
			else if (affectedDealZoneGroups == null)
				dealZoneGroups = inputArgument.AffectedDealZoneGroups;
			else
				dealZoneGroups = inputArgument.AffectedDealZoneGroups.Union(affectedDealZoneGroups).ToHashSet();

			return new SyncDealDetailProgressWithBSOutput()
			{
				AffectedDealZoneGroups = dealZoneGroups
			};
		}

		protected override SyncDealDetailProgressWithBSInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new SyncDealDetailProgressWithBSInput
			{
				IsSale = this.IsSale.Get(context),
				BeginDate = this.BeginDate.Get(context),
				CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
				AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context)
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, SyncDealDetailProgressWithBSOutput result)
		{
			context.SetValue(this.AffectedDealZoneGroups, result.AffectedDealZoneGroups);
		}

		#region Private Methods

		private void SyncDealDetailProgressWithBillingStats(Dictionary<DealZoneGroup, List<DealBillingSummary>> currentDealBillingSummaryRecords, bool isSale, DateTime beginDate, out HashSet<DealZoneGroup> affectedDealZoneGroups)
		{
			DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
			DealProgressManager dealProgressManager = new DealProgressManager();

			if (currentDealBillingSummaryRecords == null)
			{
				Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgressesDictToDelete = dealDetailedProgressManager.GetDealDetailedProgressesByDate(isSale, beginDate, null);
				if (dealDetailedProgressesDictToDelete != null)
				{
					affectedDealZoneGroups = dealDetailedProgressesDictToDelete.Keys.Select(itm => new DealZoneGroup() { DealId = itm.DealId, ZoneGroupNb = itm.ZoneGroupNb }).ToHashSet();
					dealProgressManager.InsertAffectedDealZoneGroups(affectedDealZoneGroups, isSale);
					dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesDictToDelete.Values.Select(itm => itm.DealDetailedProgressId).ToList());
				}
				else
				{
					affectedDealZoneGroups = null;
				}
				return;
			}

			affectedDealZoneGroups = currentDealBillingSummaryRecords.Keys.ToHashSet();
			int intervalOffsetInMinutes = new ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();
			var dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgressesByDate(isSale, beginDate, null);

			if (dealDetailedProgresses != null)
				affectedDealZoneGroups.UnionWith(dealDetailedProgresses.Keys.Select(itm => new DealZoneGroup() { DealId = itm.DealId, ZoneGroupNb = itm.ZoneGroupNb }));


			List<DealDetailedProgress> dealDetailedProgressesToAdd = new List<DealDetailedProgress>();
			List<DealDetailedProgress> dealDetailedProgressesToUpdate = new List<DealDetailedProgress>();
			HashSet<long> dealDetailedProgressesToKeep = new HashSet<long>();

			DealDetailedProgress tempDealDetailedProgress;

			foreach (var currentDealBillingSummaryRecord in currentDealBillingSummaryRecords)
			{
				DealZoneGroup dealZoneGroup = currentDealBillingSummaryRecord.Key;

				foreach (var dealBillingSummary in currentDealBillingSummaryRecord.Value)
				{
					DealDetailedZoneGroupTier currentDealDetailedZoneGroupTier = BuildDealDetailedZoneGroupTier(dealBillingSummary, intervalOffsetInMinutes);

					if (dealDetailedProgresses != null && dealDetailedProgresses.TryGetValue(currentDealDetailedZoneGroupTier, out tempDealDetailedProgress))
					{
						dealDetailedProgressesToKeep.Add(tempDealDetailedProgress.DealDetailedProgressId);
						if (!dealDetailedProgressManager.AreEqual(tempDealDetailedProgress, dealBillingSummary))
							dealDetailedProgressesToUpdate.Add(BuildDealDetailedProgress(dealBillingSummary, tempDealDetailedProgress.DealDetailedProgressId, intervalOffsetInMinutes));
					}
					else
					{
						dealDetailedProgressesToAdd.Add(BuildDealDetailedProgress(dealBillingSummary, null, intervalOffsetInMinutes));
					}
				}
			}

			IEnumerable<DealDetailedProgress> dealDetailedProgressesToDelete = dealDetailedProgresses != null ? 
				dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressId)) : null;

			if (dealDetailedProgressesToDelete != null && dealDetailedProgressesToDelete.Any())
			{
				dealProgressManager.InsertAffectedDealZoneGroups(dealDetailedProgressesToDelete.Select(itm => new DealZoneGroup() { DealId = itm.DealId, ZoneGroupNb = itm.ZoneGroupNb }).ToHashSet(), isSale);
				dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete.Select(itm => itm.DealDetailedProgressId).ToList());
			}

			dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressesToAdd);
			dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressesToUpdate);
		}

		private DealDetailedZoneGroupTier BuildDealDetailedZoneGroupTier(DealBillingSummary dealBillingSummary, int intervalOffsetInMinutes)
		{
			return new DealDetailedZoneGroupTier()
			{
				DealId = dealBillingSummary.DealId,
				ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
				FromTime = dealBillingSummary.BatchStart,
				RateTierNb = dealBillingSummary.RateTierNb,
				TierNb = dealBillingSummary.TierNb,
				ToTime = dealBillingSummary.BatchStart.AddMinutes(intervalOffsetInMinutes)
			};
		}

		private DealDetailedProgress BuildDealDetailedProgress(DealBillingSummary dealBillingSummary, long? dealDetailedProgressID, int intervalOffsetInMinutes)
		{
			DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
			{
				IsSale = dealBillingSummary.IsSale,
				DealId = dealBillingSummary.DealId,
				ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
				TierNb = dealBillingSummary.TierNb,
				RateTierNb = dealBillingSummary.RateTierNb,
				FromTime = dealBillingSummary.BatchStart,
				ToTime = dealBillingSummary.BatchStart.AddMinutes(intervalOffsetInMinutes),
				ReachedDurationInSeconds = dealBillingSummary.DurationInSeconds
			};

			if (dealDetailedProgressID.HasValue)
				dealDetailedProgress.DealDetailedProgressId = dealDetailedProgressID.Value;

			return dealDetailedProgress;
		}

		#endregion
	}
}