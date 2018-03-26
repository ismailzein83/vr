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
    public class PrepareExpectedDealTrafficRecordsInput
    {
        public Boolean IsSale { get; set; }

        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> DealDetailedProgresses { get; set; }
    }

    public class PrepareExpectedDealTrafficRecordsOutput
    {
        public Dictionary<DealZoneGroup, DealProgressData> ExpectedDealProgressData { get; set; }

        public Dictionary<DealDetailedZoneGroupTierWithoutRate, DealBillingSummary> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public sealed class PrepareExpectedDealTrafficRecords : BaseAsyncActivity<PrepareExpectedDealTrafficRecordsInput, PrepareExpectedDealTrafficRecordsOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>> DealDetailedProgresses { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<DealDetailedZoneGroupTierWithoutRate, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<DealZoneGroup, DealProgressData>> ExpectedDealProgressData { get; set; }

        protected override PrepareExpectedDealTrafficRecordsOutput DoWorkWithResult(PrepareExpectedDealTrafficRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            var dealProgressDataByZoneGroup = new Dictionary<DealZoneGroup, DealProgressData>();
            var expectedDealBillingSummaryByZoneGroupTier = new Dictionary<DealZoneGroupTier, List<DealBillingSummary>>();

            if (inputArgument.DealDetailedProgresses != null)
            {
                Dictionary<DealZoneGroup, Dictionary<DateTime, decimal>> expectedDealTrafficRecords = BuildExpectedDealTraffic(inputArgument.DealDetailedProgresses);

                var dealDefinitionManager = new DealDefinitionManager();

                foreach (var kvp_expectedDealTrafficRecord in expectedDealTrafficRecords)
                {
                    DealZoneGroup dealZoneGroup = kvp_expectedDealTrafficRecord.Key;
                    var orderedDealTrafficRecords = kvp_expectedDealTrafficRecord.Value.OrderBy(itm => itm.Key);

                    foreach (var dealTrafficRecord in orderedDealTrafficRecords)
                    {
                        DealProgressData dealProgressData;
                        if (!dealProgressDataByZoneGroup.TryGetValue(dealZoneGroup, out dealProgressData)) //1st Tier
                        {
                            DealZoneGroupTierDetailsWithoutRate firstDealZoneGroupTierDetails = dealDefinitionManager.GetDealZoneGroupTierDetailsWithoutRate(inputArgument.IsSale, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, 1);
                            if (firstDealZoneGroupTierDetails == null)
                                throw new VRBusinessException(string.Format("dealId '{0}' and ZoneGroupNb '{1}' should contains at least one Tier"));

                            dealProgressData = new DealProgressData();
                            dealProgressData.DealId = dealZoneGroup.DealId;
                            dealProgressData.ZoneGroupNb = dealZoneGroup.ZoneGroupNb;
                            dealProgressData.IsSale = inputArgument.IsSale;
                            dealProgressData.CurrentTierNb = firstDealZoneGroupTierDetails.TierNb;
                            dealProgressData.ReachedDurationInSeconds = 0;
                            dealProgressData.TargetDurationInSeconds = firstDealZoneGroupTierDetails.VolumeInSeconds;
                            dealProgressDataByZoneGroup.Add(dealZoneGroup, dealProgressData);
                        }

                        BuildExpectedDealBillingSummaryDict(inputArgument.IsSale, dealZoneGroup, dealTrafficRecord.Key, dealTrafficRecord.Value, dealProgressData, expectedDealBillingSummaryByZoneGroupTier);
                    }
                }

                string isSaleAsString = inputArgument.IsSale ? "Sale" : "Cost";
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Prepare Expected {0} Deal Traffic Records is done", isSaleAsString), null);
            }

            return new PrepareExpectedDealTrafficRecordsOutput()
            {
                ExpectedDealBillingSummaryRecords = expectedDealBillingSummaryByZoneGroupTier.Count > 0 ? BuildExpectedDealBillingSummaryRecordDict(expectedDealBillingSummaryByZoneGroupTier) : null,
                ExpectedDealProgressData = dealProgressDataByZoneGroup.Count > 0 ? dealProgressDataByZoneGroup : null
            };
        }

        protected override PrepareExpectedDealTrafficRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExpectedDealTrafficRecordsInput
            {
                IsSale = this.IsSale.Get(context),
                DealDetailedProgresses = this.DealDetailedProgresses.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareExpectedDealTrafficRecordsOutput result)
        {
            this.ExpectedDealProgressData.Set(context, result.ExpectedDealProgressData);
            this.ExpectedDealBillingSummaryRecords.Set(context, result.ExpectedDealBillingSummaryRecords);
        }

        #region Private Methods

        private Dictionary<DealZoneGroup, Dictionary<DateTime, decimal>> BuildExpectedDealTraffic(Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses)
        {
            var expectedDealTrafficRecords = new Dictionary<DealZoneGroup, Dictionary<DateTime, decimal>>();

            Dictionary<DateTime, decimal> tempDealTrafficByBatchStart;
            decimal tempDurationInSeconds;

            foreach (var kvp_dealDetailedProgress in dealDetailedProgresses)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_dealDetailedProgress.Key;
                DealDetailedProgress dealDetailedProgress = kvp_dealDetailedProgress.Value;

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealDetailedZoneGroupTier.DealId, ZoneGroupNb = dealDetailedZoneGroupTier.ZoneGroupNb };
                tempDealTrafficByBatchStart = expectedDealTrafficRecords.GetOrCreateItem(dealZoneGroup, () => { return new Dictionary<DateTime, decimal>(); });
                tempDurationInSeconds = tempDealTrafficByBatchStart.GetOrCreateItem(dealDetailedProgress.FromTime, () => { return 0; });
                tempDurationInSeconds += dealDetailedProgress.ReachedDurationInSeconds;
                tempDealTrafficByBatchStart[dealDetailedProgress.FromTime] = tempDurationInSeconds;
            }

            return expectedDealTrafficRecords;
        }

        private void BuildExpectedDealBillingSummaryDict(bool isSale, DealZoneGroup dealZoneGroup, DateTime batchStart, decimal remainingDurationInSec, DealProgressData dealProgressData,
            Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryByZoneGroupTier)
        {
            decimal remainingTierDurationInSec = dealProgressData.TargetDurationInSeconds.HasValue ? (dealProgressData.TargetDurationInSeconds.Value - dealProgressData.ReachedDurationInSeconds) : decimal.MaxValue;

            if (remainingTierDurationInSec >= remainingDurationInSec)
            {
                DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, dealZoneGroup, batchStart, dealProgressData.CurrentTierNb, dealProgressData.CurrentTierNb, remainingDurationInSec);
                FillExpectedDealZoneGroupTierDict(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);

                //Editing DealProgressData memoryTable
                dealProgressData.ReachedDurationInSeconds += remainingDurationInSec;
            }
            else
            {
                if (remainingTierDurationInSec > 0)
                {
                    DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, dealZoneGroup, batchStart, dealProgressData.CurrentTierNb, dealProgressData.CurrentTierNb, remainingTierDurationInSec);
                    FillExpectedDealZoneGroupTierDict(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);

                    remainingDurationInSec -= remainingTierDurationInSec;
                    dealProgressData.ReachedDurationInSeconds = dealProgressData.TargetDurationInSeconds.Value;
                }

                //Editing DealProgressData memoryTable
                DealZoneGroupTierDetailsWithoutRate nextDealZoneGroupTierDetails = new DealDefinitionManager().GetDealZoneGroupTierDetailsWithoutRate(isSale, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, dealProgressData.CurrentTierNb + 1);
                if (nextDealZoneGroupTierDetails == null)
                {
                    DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, dealZoneGroup, batchStart, null, null, remainingDurationInSec);
                    FillExpectedDealZoneGroupTierDict(expectedDealBillingSummaryByZoneGroupTier, dealBillingSummary);
                }
                else
                {
                    CheckNextTierRetroActive(dealZoneGroup, nextDealZoneGroupTierDetails, expectedDealBillingSummaryByZoneGroupTier);

                    dealProgressData.CurrentTierNb = nextDealZoneGroupTierDetails.TierNb;
                    dealProgressData.TargetDurationInSeconds = nextDealZoneGroupTierDetails.VolumeInSeconds;
                    dealProgressData.ReachedDurationInSeconds = 0;
                    BuildExpectedDealBillingSummaryDict(isSale, dealZoneGroup, batchStart, remainingDurationInSec, dealProgressData, expectedDealBillingSummaryByZoneGroupTier);
                }
            }
        }

        private DealBillingSummary BuildDealBillingSummary(bool isSale, DealZoneGroup dealZoneGroup, DateTime batchStart, int? tierNb, int? rateTierNb, decimal remainingDurationInSec)
        {
            DealBillingSummary dealBillingSummary = new DealBillingSummary()
            {
                BatchStart = batchStart,
                DealId = dealZoneGroup.DealId,
                ZoneGroupNb = dealZoneGroup.ZoneGroupNb,
                TierNb = tierNb,
                RateTierNb = rateTierNb,
                DurationInSeconds = remainingDurationInSec,
                IsSale = isSale
            };
            return dealBillingSummary;
        }

        private void FillExpectedDealZoneGroupTierDict(Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryByZoneGroupTier, DealBillingSummary dealBillingSummary)
        {
            DealZoneGroupTier dealZoneGroupTier = new DealZoneGroupTier()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
                TierNb = dealBillingSummary.TierNb
            };

            List<DealBillingSummary> dealBillingSummaries = expectedDealBillingSummaryByZoneGroupTier.GetOrCreateItem(dealZoneGroupTier);
            dealBillingSummaries.Add(dealBillingSummary);
        }

        private void CheckNextTierRetroActive(DealZoneGroup dealZoneGroup, DealZoneGroupTierDetailsWithoutRate nextDealZoneGroupTierDetails,
            Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryRecordByZoneGroupTier)
        {
            if (nextDealZoneGroupTierDetails.RetroActiveFromTierNb.HasValue)
            {
                int fromTierNumber = nextDealZoneGroupTierDetails.RetroActiveFromTierNb.Value;
                int toTierNumber = nextDealZoneGroupTierDetails.TierNb - 1;

                while (toTierNumber >= fromTierNumber)
                {
                    DealZoneGroupTier dealZoneGroupTier = new DealZoneGroupTier()
                    {
                        DealId = dealZoneGroup.DealId,
                        ZoneGroupNb = dealZoneGroup.ZoneGroupNb,
                        TierNb = fromTierNumber
                    };

                    List<DealBillingSummary> dealBillingSummaries;
                    if (expectedDealBillingSummaryRecordByZoneGroupTier.TryGetValue(dealZoneGroupTier, out dealBillingSummaries))
                    {
                        foreach (var itm in dealBillingSummaries)
                            itm.RateTierNb = nextDealZoneGroupTierDetails.TierNb;
                    }

                    fromTierNumber++;
                }
            }
        }

        private Dictionary<DealDetailedZoneGroupTierWithoutRate, DealBillingSummary> BuildExpectedDealBillingSummaryRecordDict(Dictionary<DealZoneGroupTier, List<DealBillingSummary>> expectedDealBillingSummaryRecordByZoneGroupTier)
        {
            var expectedDealBillingSummaryRecordDict = new Dictionary<DealDetailedZoneGroupTierWithoutRate, DealBillingSummary>();

            int intervalOffsetInMinutes = new ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();

            foreach (var dealBillingSummaryRecords in expectedDealBillingSummaryRecordByZoneGroupTier.Values)
            {
                foreach (var dealBillingSummaryRecord in dealBillingSummaryRecords)
                {
                    DealDetailedZoneGroupTierWithoutRate dealDetailedZoneGroupTier = new DealDetailedZoneGroupTierWithoutRate()
                    {
                        DealId = dealBillingSummaryRecord.DealId,
                        ZoneGroupNb = dealBillingSummaryRecord.ZoneGroupNb,
                        TierNb = dealBillingSummaryRecord.TierNb,
                        //RateTierNb = dealBillingSummaryRecord.RateTierNb,
                        FromTime = dealBillingSummaryRecord.BatchStart,
                        ToTime = dealBillingSummaryRecord.BatchStart.AddMinutes(intervalOffsetInMinutes),
                    };

                    expectedDealBillingSummaryRecordDict.Add(dealDetailedZoneGroupTier, dealBillingSummaryRecord);
                }
            }

            return expectedDealBillingSummaryRecordDict;
        }

        #endregion
    }
}
