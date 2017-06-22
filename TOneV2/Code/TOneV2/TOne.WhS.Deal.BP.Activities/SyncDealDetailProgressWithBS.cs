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
    public class SyncDealDetailProgressWithBSInput
    {
        public Boolean IsSale { get; set; }

        public DateTime BeginDate { get; set; }

        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }

        public Dictionary<DealZoneGroup, List<DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class SyncDealDetailProgressWithBS : BaseAsyncActivity<SyncDealDetailProgressWithBSInput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> BeginDate { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealZoneGroup, List<DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void DoWork(SyncDealDetailProgressWithBSInput inputArgument, AsyncActivityHandle handle)
        {
            SyncDealDetailProgressWithBillingStats(inputArgument.CurrentDealBillingSummaryRecords, inputArgument.AffectedDealZoneGroups, inputArgument.IsSale, inputArgument.BeginDate);
        }

        protected override SyncDealDetailProgressWithBSInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new SyncDealDetailProgressWithBSInput
            {
                IsSale = this.IsSale.Get(context),
                BeginDate = this.BeginDate.Get(context),
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context),
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context)
            };
        }

        #region Private Methods

        private void SyncDealDetailProgressWithBillingStats(Dictionary<DealZoneGroup, List<DealBillingSummary>> currentDealBillingSummaryRecords, HashSet<DealZoneGroup> affectedDealZoneGroups, bool isSale, DateTime beginDate)
        {
            if (currentDealBillingSummaryRecords == null)
                return;

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            var dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(affectedDealZoneGroups, isSale, beginDate);

            List<DealDetailedProgress> dealDetailedProgressesToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> dealDetailedProgressesToUpdate = new List<DealDetailedProgress>();
            HashSet<long> dealDetailedProgressesToKeep = new HashSet<long>();

            DealDetailedProgress tempDealDetailedProgress;

            foreach (var currentDealBillingSummaryRecord in currentDealBillingSummaryRecords)
            {
                DealZoneGroup dealZoneGroup = currentDealBillingSummaryRecord.Key;

                foreach (var dealBillingSummary in currentDealBillingSummaryRecord.Value)
                {
                    DealDetailedZoneGroupTier currentDealDetailedZoneGroupTier = BuildDealDetailedZoneGroupTier(dealBillingSummary);

                    if (dealDetailedProgresses != null && dealDetailedProgresses.TryGetValue(currentDealDetailedZoneGroupTier, out tempDealDetailedProgress))
                    {
                        dealDetailedProgressesToKeep.Add(tempDealDetailedProgress.DealDetailedProgressID);
                        if (!dealDetailedProgressManager.AreEqual(tempDealDetailedProgress, dealBillingSummary))
                            dealDetailedProgressesToUpdate.Add(BuildDealDetailedProgress(tempDealDetailedProgress.DealDetailedProgressID, dealBillingSummary));
                    }
                    else
                    {
                        dealDetailedProgressesToAdd.Add(BuildDealDetailedProgress(null, dealBillingSummary));
                    }
                }
            }

            List<long> dealDetailedProgressesToDelete = dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressID)).Select(itm => itm.DealDetailedProgressID).ToList();

            dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressesToAdd);
            dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressesToUpdate);
            dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete);
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

        #endregion
    }
}