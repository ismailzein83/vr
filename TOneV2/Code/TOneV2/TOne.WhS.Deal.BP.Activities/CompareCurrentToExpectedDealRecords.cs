using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.Common;

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

        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        public InArgument<Dictionary<DealDetailedZoneGroupTier, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }

        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override CompareCurrentToExpectedDealRecordsOutput DoWorkWithResult(CompareCurrentToExpectedDealRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            List<DealDetailedProgress> dealDetailedProgressToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> dealDetailedProgressToUpdate = new List<DealDetailedProgress>();

            List<DealProgress> dealProgressesToAdd = new List<DealProgress>();
            List<DealProgress> dealProgressesToUpdate = new List<DealProgress>();
            Dictionary<DealZoneGroup, decimal> totalReachedDurationByDealZoneGroup = new Dictionary<DealZoneGroup, decimal>();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            DealDetailedProgress currentDealDetailedProgresses;

            foreach (var kvp_expectedDealBillingSummaryRecord in inputArgument.ExpectedDealBillingSummaryRecords)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_expectedDealBillingSummaryRecord.Key;
                DealBillingSummary expectedDealBillingSummary = kvp_expectedDealBillingSummaryRecord.Value;

                if (!dealDetailedProgresses.TryGetValue(dealDetailedZoneGroupTier, out currentDealDetailedProgresses))
                {
                    dealDetailedProgressToAdd.Add(BuildDealDetailedProgress(expectedDealBillingSummary, null));
                }
                else if (!dealDetailedProgressManager.AreEqual(currentDealDetailedProgresses, expectedDealBillingSummary))
                {
                    dealDetailedProgressToUpdate.Add(BuildDealDetailedProgress(expectedDealBillingSummary, currentDealDetailedProgresses.DealDetailedProgressID));
                }

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealDetailedZoneGroupTier.DealID, ZoneGroupNb = dealDetailedZoneGroupTier.ZoneGroupNb };
                decimal totalReachedDurationInSeconds = totalReachedDurationByDealZoneGroup.GetOrCreateItem(dealZoneGroup, () => { return default(decimal); });
                totalReachedDurationInSeconds += expectedDealBillingSummary.DurationInSeconds;
                totalReachedDurationByDealZoneGroup[dealZoneGroup] = totalReachedDurationInSeconds;
            }

            var storedDealProgresses = new DealProgressManager().GetDealProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            DealProgress storedDealProgress;
            decimal tierReachedDurationInSec;

            foreach (var kvp_totalReachedDuration in totalReachedDurationByDealZoneGroup)
            {
                DealZoneGroup dealZoneGroup = kvp_totalReachedDuration.Key;

                DealZoneGroupTierDetails dealZoneGroupTierDetails =
                    dealDefinitionManager.GetUpToVolumeDealZoneGroupTierDetails(inputArgument.IsSale, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, kvp_totalReachedDuration.Value, out tierReachedDurationInSec);
                DealProgress expectedDealProgress = BuildDealProgress(inputArgument.IsSale, dealZoneGroup, dealZoneGroupTierDetails, tierReachedDurationInSec);

                if (storedDealProgresses != null && storedDealProgresses.TryGetValue(dealZoneGroup, out storedDealProgress))
                {
                    if (!expectedDealProgress.IsEqual(storedDealProgress))
                    {
                        expectedDealProgress.DealProgressID = storedDealProgress.DealProgressID;
                        dealProgressesToUpdate.Add(expectedDealProgress);
                    }
                }
                else
                {
                    dealProgressesToAdd.Add(expectedDealProgress);
                }
            }

            if (dealDetailedProgressToAdd.Count == 0 && dealDetailedProgressToUpdate.Count == 0)
                return null;

            dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressToAdd);
            dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressToUpdate);

            List<DateTime> daysToReprocess = new List<DateTime>();

            if (dealDetailedProgressToAdd.Count > 0)
                daysToReprocess.AddRange(dealDetailedProgressToAdd.Select(itm => itm.FromTime.Date));

            if (dealDetailedProgressToUpdate.Count > 0)
                daysToReprocess.AddRange(dealDetailedProgressToUpdate.Select(itm => itm.FromTime.Date));

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

        private DealProgress BuildDealProgress(bool isSale, DealZoneGroup dealZoneGroup, DealZoneGroupTierDetails dealZoneGroupTierDetails, decimal tierReachedDurationInSec)
        {
            return new DealProgress()
            {
                IsSale = isSale,
                DealID = dealZoneGroup.DealId,
                ZoneGroupNb = dealZoneGroup.ZoneGroupNb,
                CurrentTierNb = dealZoneGroupTierDetails.TierNumber,
                ReachedDurationInSeconds = tierReachedDurationInSec,
                TargetDurationInSeconds = dealZoneGroupTierDetails.VolumeInSeconds
            };
        }

        #endregion
    }
}
