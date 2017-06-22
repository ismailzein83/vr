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

            int intervalOffset = new ConfigManager().GetDealTechnicalSettingIntervalOffset();

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
                    DealDetailedZoneGroupTier currentDealDetailedZoneGroupTier = BuildDealDetailedZoneGroupTier(dealBillingSummary, intervalOffset);

                    if (dealDetailedProgresses != null && dealDetailedProgresses.TryGetValue(currentDealDetailedZoneGroupTier, out tempDealDetailedProgress))
                    {
                        dealDetailedProgressesToKeep.Add(tempDealDetailedProgress.DealDetailedProgressId);
                        if (!dealDetailedProgressManager.AreEqual(tempDealDetailedProgress, dealBillingSummary))
                            dealDetailedProgressesToUpdate.Add(BuildDealDetailedProgress(dealBillingSummary, tempDealDetailedProgress.DealDetailedProgressId, intervalOffset));
                    }
                    else
                    {
                        dealDetailedProgressesToAdd.Add(BuildDealDetailedProgress(dealBillingSummary, null, intervalOffset));
                    }
                }
            }

            List<long> dealDetailedProgressesToDelete = dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressId)).Select(itm => itm.DealDetailedProgressId).ToList();

            dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressesToAdd);
            dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressesToUpdate);
            dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete);
        }

        private DealDetailedZoneGroupTier BuildDealDetailedZoneGroupTier(DealBillingSummary dealBillingSummary, int intervalOffset)
        {
            return new DealDetailedZoneGroupTier()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
                FromTime = dealBillingSummary.BatchStart,
                RateTierNb = dealBillingSummary.RateTierNb,
                TierNb = dealBillingSummary.TierNb,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(intervalOffset)
            };
        }

        private DealDetailedProgress BuildDealDetailedProgress(DealBillingSummary dealBillingSummary, long? dealDetailedProgressID, int intervalOffset)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.TierNb,
                RateTierNb = dealBillingSummary.RateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(intervalOffset)
            };

            if (dealDetailedProgressID.HasValue)
                dealDetailedProgress.DealDetailedProgressId = dealDetailedProgressID.Value;

            return dealDetailedProgress;
        }

        #endregion
    }
}