using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class UpdateProgressTablesInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroup, List<DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class UpdateProgressTables : BaseAsyncActivity<UpdateProgressTablesInput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealZoneGroup, List<DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void DoWork(UpdateProgressTablesInput inputArgument, AsyncActivityHandle handle)
        {
            UpdateDealProgressTables(inputArgument.CurrentDealBillingSummaryRecords, inputArgument.IsSale);
        }

        protected override UpdateProgressTablesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateProgressTablesInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
            };
        }

        #region Private Methods

        private void UpdateDealProgressTables(Dictionary<DealZoneGroup, List<DealBillingSummary>> currentDealBillingSummaryRecords, bool isSale)
        {
            if (currentDealBillingSummaryRecords == null)
                return;

            HashSet<DealZoneGroup> affectedDealZoneGroups = currentDealBillingSummaryRecords.Keys.ToHashSet();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            var dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(affectedDealZoneGroups, isSale);

            DealProgressManager dealProgressManager = new DealProgressManager();
            var dealProgresses = dealProgressManager.GetDealProgresses(affectedDealZoneGroups, isSale);

            List<DealDetailedProgress> dealDetailedProgressesToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> dealDetailedProgressesToUpdate = new List<DealDetailedProgress>();
            HashSet<long> dealDetailedProgressesToKeep = new HashSet<long>();

            List<DealProgress> dealProgressesToAdd = new List<DealProgress>();
            List<DealProgress> dealProgressesToUpdate = new List<DealProgress>();

            DealDetailedProgress tempDealDetailedProgress;
            DealProgress tempDealProgress;

            foreach (var currentDealBillingSummaryRecord in currentDealBillingSummaryRecords)
            {
                DealZoneGroup dealZoneGroup = currentDealBillingSummaryRecord.Key;

                int maxTierNumber = 0;
                decimal tierReachedDurationInSeconds = 0;

                foreach (var dealBillingSummary in currentDealBillingSummaryRecord.Value)
                {
                    if (dealBillingSummary.DealTierNb > maxTierNumber)
                    {
                        maxTierNumber = dealBillingSummary.DealTierNb;
                        tierReachedDurationInSeconds = dealBillingSummary.DurationInSeconds;
                    }
                    else if (dealBillingSummary.DealTierNb == maxTierNumber)
                    {
                        tierReachedDurationInSeconds += dealBillingSummary.DurationInSeconds;
                    }

                    DealDetailedZoneGroupTier currentDealDetailedZoneGroupTier = BuildDealDetailedZoneGroupTier(dealBillingSummary);

                    if (dealDetailedProgresses != null && dealDetailedProgresses.TryGetValue(currentDealDetailedZoneGroupTier, out tempDealDetailedProgress))
                    {
                        dealDetailedProgressesToKeep.Add(tempDealDetailedProgress.DealDetailedProgressID);
                        if (!AreEqual(dealBillingSummary, tempDealDetailedProgress))
                            dealDetailedProgressesToUpdate.Add(BuildDealDetailedProgress(tempDealDetailedProgress.DealDetailedProgressID, dealBillingSummary));
                    }
                    else
                    {
                        dealDetailedProgressesToAdd.Add(BuildDealDetailedProgress(null, dealBillingSummary));
                    }
                }

                if (dealProgresses != null && dealProgresses.TryGetValue(dealZoneGroup, out tempDealProgress))
                {
                    DealProgress dealProgress = BuildDealProgress(tempDealProgress.DealProgressID, dealZoneGroup, maxTierNumber, tierReachedDurationInSeconds, isSale);
                    if (!dealProgress.IsEqual(tempDealProgress))
                        dealProgressesToUpdate.Add(dealProgress);
                }
                else
                {
                    DealProgress dealProgress = BuildDealProgress(null, dealZoneGroup, maxTierNumber, tierReachedDurationInSeconds, isSale);
                    dealProgressesToAdd.Add(dealProgress);
                }
            }

            List<long> dealDetailedProgressesToDelete = dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressID)).Select(itm => itm.DealDetailedProgressID).ToList();

            dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressesToAdd);
            dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressesToUpdate);
            dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete);

            dealProgressManager.InsertDealProgresses(dealProgressesToAdd);
            dealProgressManager.UpdateDealProgresses(dealProgressesToUpdate);
        }

        private DealDetailedZoneGroupTier BuildDealDetailedZoneGroupTier(DealBillingSummary dealBillingSummary)
        {
            return new DealDetailedZoneGroupTier()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                FromTime = dealBillingSummary.BatchStart,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                TierNb = dealBillingSummary.DealTierNb,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30)
            };
        }

        private DealDetailedProgress BuildDealDetailedProgress(long? dealDetailedProgressID, DealBillingSummary dealBillingSummary)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30)
            };

            if (dealDetailedProgressID.HasValue)
                dealDetailedProgress.DealDetailedProgressID = dealDetailedProgressID.Value;

            return dealDetailedProgress;
        }

        private DealProgress BuildDealProgress(long? dealProgressId, DealZoneGroup dealZoneGroup, int tierNb, decimal reachedDurationInSeconds, bool isSale)
        {
            DealZoneGroupTierDetails dealZoneGroupTierDetails = new DealDefinitionManager().GetDealZoneGroupTierDetails(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, tierNb, isSale);
            DealProgress dealProgress = new DealProgress()
            {
                DealID = dealZoneGroup.DealId,
                ZoneGroupNb = dealZoneGroup.ZoneGroupNb,
                IsSale = isSale,
                CurrentTierNb = tierNb,
                ReachedDurationInSeconds = reachedDurationInSeconds,
                TargetDurationInSeconds = dealZoneGroupTierDetails != null ? dealZoneGroupTierDetails.VolumeInSeconds : null
            };

            if (dealProgressId.HasValue)
                dealProgress.DealProgressID = dealProgressId.Value;

            return dealProgress;
        }

        private bool AreEqual(DealBillingSummary dealBillingSummary, DealDetailedProgress dealDetailedProgress)
        {
            if (dealBillingSummary == null || dealDetailedProgress == null)
                return false;

            if (dealDetailedProgress.FromTime != dealBillingSummary.BatchStart)
                return false;

            if (dealDetailedProgress.ToTime != dealBillingSummary.BatchStart.AddMinutes(30))
                return false;

            if (dealDetailedProgress.DealID != dealBillingSummary.DealId)
                return false;

            if (dealDetailedProgress.ZoneGroupNb != dealBillingSummary.DealZoneGroupNb)
                return false;

            if (dealDetailedProgress.ReachedDurationInSeconds != dealBillingSummary.DurationInSeconds)
                return false;

            if (dealDetailedProgress.TierNb != dealBillingSummary.DealTierNb)
                return false;

            if (dealDetailedProgress.RateTierNb != dealBillingSummary.DealRateTierNb)
                return false;

            return true;
        }

        #endregion
    }
}