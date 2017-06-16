using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
    public class CompareCurrentToExpectedDealRecordsInput
    {
        public Boolean IsSale { get; set; }

        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }

        public Dictionary<DealDetailedZoneGroupTier, DealBillingSummary> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public class CompareCurrentToExpectedDealRecordsOutput
    {
        public HashSet<DateTime> DaysToReprocess { get; set; }
    }

    public sealed class CompareCurrentToExpectedDealRecords : BaseAsyncActivity<CompareCurrentToExpectedDealRecordsInput, CompareCurrentToExpectedDealRecordsOutput>
    {
        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealDetailedZoneGroupTier, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }

        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override CompareCurrentToExpectedDealRecordsOutput DoWorkWithResult(CompareCurrentToExpectedDealRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            List<DealDetailedProgress> itemsToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> itemsToUpdate = new List<DealDetailedProgress>();
            List<DateTime> daysToReprocess = new List<DateTime>();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            DealDetailedProgress currentDealDetailedProgresses;

            foreach (var kvp_expectedDealBillingSummaryRecord in inputArgument.ExpectedDealBillingSummaryRecords)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_expectedDealBillingSummaryRecord.Key;
                DealBillingSummary expectedDealBillingSummary = kvp_expectedDealBillingSummaryRecord.Value;

                if (!dealDetailedProgresses.TryGetValue(dealDetailedZoneGroupTier, out currentDealDetailedProgresses))
                {
                    itemsToAdd.Add(BuildDealDetailedProgress(expectedDealBillingSummary, null));
                }
                else if (!dealDetailedProgressManager.AreEqual(currentDealDetailedProgresses, expectedDealBillingSummary))
                {
                    itemsToUpdate.Add(BuildDealDetailedProgress(expectedDealBillingSummary, currentDealDetailedProgresses.DealDetailedProgressID));
                }
            }

            if (itemsToAdd.Count == 0 && itemsToUpdate.Count == 0)
                return null;

            if (itemsToAdd.Count > 0)
                daysToReprocess.AddRange(itemsToAdd.Select(itm => itm.FromTime.Date));

            if (itemsToUpdate.Count > 0)
                daysToReprocess.AddRange(itemsToUpdate.Select(itm => itm.FromTime.Date));

            return new CompareCurrentToExpectedDealRecordsOutput()
            {
                DaysToReprocess = new HashSet<DateTime>(daysToReprocess)
            };
        }

        protected override CompareCurrentToExpectedDealRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CompareCurrentToExpectedDealRecordsInput
            {
                IsSale = this.IsSale.Get(context),
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context),
                ExpectedDealBillingSummaryRecords = this.ExpectedDealBillingSummaryRecords.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CompareCurrentToExpectedDealRecordsOutput result)
        {
            this.DaysToReprocess.Set(context, result.DaysToReprocess);
        }

        #region Private Methods

        private DealReprocessInput BuildDealReprocessInput(DealBillingSummary dealBillingSummary)
        {
            return new DealReprocessInput()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30),
                UpToDurationInSec = dealBillingSummary.DurationInSeconds
            };
        }

        private DealDetailedProgress BuildDealDetailedProgress(DealBillingSummary dealBillingSummary, long? dealDetailedProgressID)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30),
                ReachedDurationInSeconds = dealBillingSummary.DurationInSeconds
            };

            if (dealDetailedProgressID.HasValue)
                dealDetailedProgress.DealDetailedProgressID = dealDetailedProgressID.Value;

            return dealDetailedProgress;
        }

        #endregion
    }
}
