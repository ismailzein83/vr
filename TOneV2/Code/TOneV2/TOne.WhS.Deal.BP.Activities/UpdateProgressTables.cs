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

        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }

        public Dictionary<DealZoneGroup, List<DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class UpdateProgressTables : BaseAsyncActivity<UpdateProgressTablesInput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        public InArgument<Dictionary<DealZoneGroup, List<DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void DoWork(UpdateProgressTablesInput inputArgument, AsyncActivityHandle handle)
        {
            UpdateDealProgressTables(inputArgument.CurrentDealBillingSummaryRecords, inputArgument.AffectedDealZoneGroups, inputArgument.IsSale, inputArgument.BeginDate);
        }

        protected override UpdateProgressTablesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateProgressTablesInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context)
            };
        }

        #region Private Methods

        private void UpdateDealProgressTables(Dictionary<DealZoneGroup, List<DealBillingSummary>> currentDealBillingSummaryRecords, HashSet<DealZoneGroup> affectedDealZoneGroups, bool isSale, DateTime beginDate)
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

        private DealProgress BuildDealProgress(long? dealProgressId, DealZoneGroup dealZoneGroup, int tierNb, decimal reachedDurationInSeconds, bool isSale)
        {
            DealZoneGroupTierDetails dealZoneGroupTierDetails = new DealDefinitionManager().GetDealZoneGroupTierDetails(isSale, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, tierNb);
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

        #endregion
    }
}