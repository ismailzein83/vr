using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public class PrepareExpectedDealBillingSummaryRecordsInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }

        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }
    }

    public class PrepareExpectedDealBillingSummaryRecordsOutput
    {
        public Dictionary<DealDetailedZoneGroupTier, DealBillingSummary> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public sealed class PrepareExpectedDealBillingSummaryRecords : BaseAsyncActivity<PrepareExpectedDealBillingSummaryRecordsInput, PrepareExpectedDealBillingSummaryRecordsOutput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        public OutArgument<Dictionary<DealDetailedZoneGroupTier, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }

        protected override PrepareExpectedDealBillingSummaryRecordsOutput DoWorkWithResult(PrepareExpectedDealBillingSummaryRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> expectedBaseDealBillingSummaryRecords = BuildExpectedBaseDealBillingSummaryDict(dealDetailedProgresses);

            var dealProgressDataByZoneGroup = new Dictionary<DealZoneGroup, DealProgressData>();
            var expectedDealBillingSummaryByZoneGroupTier = new Dictionary<DealZoneGroupTier, List<DealBillingSummary>>();

            foreach (var kvp_baseDealBillingSummary in expectedBaseDealBillingSummaryRecords)
            {
                DealZoneGroup dealZoneGroup = kvp_baseDealBillingSummary.Key;
                IOrderedEnumerable<BaseDealBillingSummary> orderedBaseDealBillingSummaries = kvp_baseDealBillingSummary.Value.Values.OrderBy(itm => itm.BatchStart);

                foreach (var baseDealBillingSummary in orderedBaseDealBillingSummaries)
                {
                    DealProgressData dealProgressData;
                    if (!dealProgressDataByZoneGroup.TryGetValue(dealZoneGroup, out dealProgressData)) //1st Tier
                    {
                        DealZoneGroupTierDetails firstDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(inputArgument.IsSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, 0);
                        if (firstDealZoneGroupTierDetails == null)
                            throw new VRBusinessException(string.Format("dealId '{0}' and ZoneGroupNb '{1}' should contains at least one Tier"));

                        dealProgressData = dealProgressDataByZoneGroup.GetOrCreateItem(dealZoneGroup);
                        dealProgressData.DealID = dealZoneGroup.DealId;
                        dealProgressData.ZoneGroupNb = dealZoneGroup.ZoneGroupNb;
                        dealProgressData.IsSale = inputArgument.IsSale;
                        dealProgressData.CurrentTierNb = firstDealZoneGroupTierDetails.TierNumber;
                        dealProgressData.ReachedDurationInSeconds = 0;
                        dealProgressData.TargetDurationInSeconds = firstDealZoneGroupTierDetails.VolumeInSeconds;

                        BuildExpectedDealBillingSummaryDict(inputArgument.IsSale, baseDealBillingSummary, dealProgressData, expectedDealBillingSummaryByZoneGroupTier);
                    }
                    else
                    {
                        BuildExpectedDealBillingSummaryDict(inputArgument.IsSale, baseDealBillingSummary, dealProgressData, expectedDealBillingSummaryByZoneGroupTier);
                    }
                }
            }

            return new PrepareExpectedDealBillingSummaryRecordsOutput()
            {
                ExpectedDealBillingSummaryRecords = BuildExpectedDealBillingSummaryRecordDict(expectedDealBillingSummaryByZoneGroupTier)
            };
        }

        protected override PrepareExpectedDealBillingSummaryRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExpectedDealBillingSummaryRecordsInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareExpectedDealBillingSummaryRecordsOutput result)
        {
            this.ExpectedDealBillingSummaryRecords.Set(context, result.ExpectedDealBillingSummaryRecords);
        }


        #region Private Methods

        private Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> BuildExpectedBaseDealBillingSummaryDict(Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses)
        {
            var expectedBaseDealBillingSummaryRecords = new Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>>();

            Dictionary<DateTime, BaseDealBillingSummary> tempBaseDealBillingSummaryByBatchStart;
            BaseDealBillingSummary tempBaseDealBillingSummary;

            foreach (var kvp_dealDetailedProgress in dealDetailedProgresses)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_dealDetailedProgress.Key;
                DealDetailedProgress dealDetailedProgress = kvp_dealDetailedProgress.Value;

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealDetailedZoneGroupTier.DealID, ZoneGroupNb = dealDetailedZoneGroupTier.ZoneGroupNb };
                BaseDealBillingSummary baseDealBillingSummary = BuildBaseDealBillingSummary(dealDetailedProgress);

                if (!expectedBaseDealBillingSummaryRecords.TryGetValue(dealZoneGroup, out tempBaseDealBillingSummaryByBatchStart))
                {
                    tempBaseDealBillingSummaryByBatchStart = new Dictionary<DateTime, BaseDealBillingSummary>();
                    tempBaseDealBillingSummaryByBatchStart.Add(baseDealBillingSummary.BatchStart, baseDealBillingSummary);
                    expectedBaseDealBillingSummaryRecords.Add(dealZoneGroup, tempBaseDealBillingSummaryByBatchStart);
                }
                else
                {
                    if (!tempBaseDealBillingSummaryByBatchStart.TryGetValue(baseDealBillingSummary.BatchStart, out tempBaseDealBillingSummary))
                        tempBaseDealBillingSummaryByBatchStart.Add(baseDealBillingSummary.BatchStart, baseDealBillingSummary);
                    else
                        tempBaseDealBillingSummary.DurationInSeconds += baseDealBillingSummary.DurationInSeconds;
                }
            }

            return expectedBaseDealBillingSummaryRecords;
        }

        private BaseDealBillingSummary BuildBaseDealBillingSummary(DealDetailedProgress dealDetailedProgress)
        {
            BaseDealBillingSummary baseDealBillingSummary = new BaseDealBillingSummary()
            {
                BatchStart = dealDetailedProgress.FromTime,
                DealId = dealDetailedProgress.DealID,
                DealZoneGroupNb = dealDetailedProgress.ZoneGroupNb,
                IsSale = dealDetailedProgress.IsSale,
                DurationInSeconds = dealDetailedProgress.ReachedDurationInSeconds
            };
            return baseDealBillingSummary;
        }

        private DealZoneGroupTierDetails GetDealZoneGroupTierDetails(bool isSale, int dealId, int zoneGroupNb, int tierNb)
        {
            if (isSale)
                return new DealDefinitionManager().GetSaleDealZoneGroupTierDetails(dealId, zoneGroupNb, tierNb);
            else
                return new DealDefinitionManager().GetSupplierDealZoneGroupTierDetails(dealId, zoneGroupNb, tierNb);
        }

        private void BuildExpectedDealBillingSummaryDict(bool isSale, BaseDealBillingSummary baseDealBillingSummary, DealProgressData dealProgressData,
            Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryByZoneGroupTier)
        {
            decimal remainingDurationInSec = baseDealBillingSummary.DurationInSeconds;

            while (remainingDurationInSec > 0)
            {
                decimal remainingTierDurationInSec;
                if (dealProgressData.TargetDurationInSeconds.HasValue)
                    remainingTierDurationInSec = dealProgressData.TargetDurationInSeconds.Value - dealProgressData.ReachedDurationInSeconds;
                else
                    remainingTierDurationInSec = decimal.MaxValue;

                if (remainingTierDurationInSec >= remainingDurationInSec)
                {
                    DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, baseDealBillingSummary, dealProgressData.CurrentTierNb, dealProgressData.CurrentTierNb, remainingDurationInSec);
                    AddToDictionary(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);

                    //Editing DealProgressData memoryTable
                    dealProgressData.ReachedDurationInSeconds += remainingDurationInSec;
                    remainingDurationInSec = 0;
                }
                else
                {
                    if (remainingTierDurationInSec > 0)
                    {
                        DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, baseDealBillingSummary, dealProgressData.CurrentTierNb, dealProgressData.CurrentTierNb, remainingTierDurationInSec);
                        AddToDictionary(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);

                        remainingDurationInSec -= remainingTierDurationInSec;
                    }

                    DealZoneGroupTierDetails nextDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(isSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, dealProgressData.CurrentTierNb + 1);
                    if (nextDealZoneGroupTierDetails == null)
                    {
                        DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, baseDealBillingSummary, null, null, remainingDurationInSec);
                        AddToDictionary(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);

                        dealProgressData.ReachedDurationInSeconds = dealProgressData.TargetDurationInSeconds.Value;
                        remainingDurationInSec = 0;
                    }
                    else
                    {
                        CheckNextTierRetroActive(baseDealBillingSummary, nextDealZoneGroupTierDetails, expectedDealBillingSummaryByZoneGroupTier);

                        dealProgressData.CurrentTierNb = nextDealZoneGroupTierDetails.TierNumber;
                        dealProgressData.TargetDurationInSeconds = nextDealZoneGroupTierDetails.VolumeInSeconds;
                        dealProgressData.ReachedDurationInSeconds = 0;
                    }
                }
            }
        }

        private DealBillingSummary BuildDealBillingSummary(bool isSale, BaseDealBillingSummary baseDealBillingSummary, int? tierNb, int? rateTierNb, decimal remainingDurationInSec)
        {
            DealBillingSummary dealBillingSummary = new DealBillingSummary()
            {
                BatchStart = baseDealBillingSummary.BatchStart,
                DealId = baseDealBillingSummary.DealId,
                DealZoneGroupNb = baseDealBillingSummary.DealZoneGroupNb,
                DealTierNb = tierNb,
                DealRateTierNb = rateTierNb,
                DurationInSeconds = remainingDurationInSec,
                IsSale = isSale
            };
            return dealBillingSummary;
        }

        private void AddToDictionary(Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryByZoneGroupTier, DealBillingSummary dealBillingSummary)
        {
            DealZoneGroupTier dealZoneGroupTier = new DealZoneGroupTier()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                TierNb = dealBillingSummary.DealTierNb
            };

            List<DealBillingSummary> dealBillingSummaries = expectedDealBillingSummaryByZoneGroupTier.GetOrCreateItem(dealZoneGroupTier);
            dealBillingSummaries.Add(dealBillingSummary);
        }

        private void CheckNextTierRetroActive(BaseDealBillingSummary baseDealBillingSummary, DealZoneGroupTierDetails nextDealZoneGroupTierDetails,
            Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryRecordByZoneGroupTier)
        {
            if (nextDealZoneGroupTierDetails.RetroActiveFromTierNumber.HasValue)
            {
                int fromTierNumber = nextDealZoneGroupTierDetails.RetroActiveFromTierNumber.Value;
                int toTierNumber = nextDealZoneGroupTierDetails.TierNumber - 1;

                while (toTierNumber >= fromTierNumber)
                {
                    DealZoneGroupTier dealZoneGroupTier = new DealZoneGroupTier()
                    {
                        DealId = baseDealBillingSummary.DealId,
                        ZoneGroupNb = baseDealBillingSummary.DealZoneGroupNb,
                        TierNb = fromTierNumber
                    };

                    List<DealBillingSummary> dealBillingSummaries;
                    if (expectedDealBillingSummaryRecordByZoneGroupTier.TryGetValue(dealZoneGroupTier, out dealBillingSummaries))
                    {
                        foreach (var itm in dealBillingSummaries)
                            itm.DealRateTierNb = nextDealZoneGroupTierDetails.TierNumber;
                    }

                    fromTierNumber++;
                }
            }
        }

        private Dictionary<DealDetailedZoneGroupTier, DealBillingSummary> BuildExpectedDealBillingSummaryRecordDict(Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryRecordByZoneGroupTier)
        {
            var expectedDealBillingSummaryRecordDict = new Dictionary<DealDetailedZoneGroupTier, DealBillingSummary>();

            foreach (var dealBillingSummaryRecords in expectedDealBillingSummaryRecordByZoneGroupTier.Values)
            {
                foreach (var dealBillingSummaryRecord in dealBillingSummaryRecords)
                {
                    DealDetailedZoneGroupTier dealDetailedZoneGroupTier = new DealDetailedZoneGroupTier()
                    {
                        DealID = dealBillingSummaryRecord.DealId,
                        ZoneGroupNb = dealBillingSummaryRecord.DealZoneGroupNb,
                        TierNb = dealBillingSummaryRecord.DealTierNb,
                        RateTierNb = dealBillingSummaryRecord.DealRateTierNb,
                        FromTime = dealBillingSummaryRecord.BatchStart,
                        ToTime = dealBillingSummaryRecord.BatchStart.AddMinutes(30),
                    };

                    expectedDealBillingSummaryRecordDict.Add(dealDetailedZoneGroupTier, dealBillingSummaryRecord);
                }
            }

            return expectedDealBillingSummaryRecordDict;
        }

        #endregion

        #region Private Classes

        private class DealProgressData
        {
            public int DealID { get; set; }

            public int ZoneGroupNb { get; set; }

            public bool IsSale { get; set; }

            public int CurrentTierNb { get; set; }

            public decimal ReachedDurationInSeconds { get; set; }

            public decimal? TargetDurationInSeconds { get; set; }
        }

        #endregion
    }
}
