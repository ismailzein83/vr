using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class PrepareExpectedDealBillingSummaryRecordsInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> ExpectedBaseDealBillingSummaryRecords { get; set; }
    }

    public class PrepareExpectedDealBillingSummaryRecordsOutput
    {
        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public sealed class PrepareExpectedDealBillingSummaryRecords : BaseAsyncActivity<PrepareExpectedDealBillingSummaryRecordsInput, PrepareExpectedDealBillingSummaryRecordsOutput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>>> ExpectedBaseDealBillingSummaryRecords { get; set; }

        public OutArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> ExpectedDealBillingSummaryRecords { get; set; }

        protected override PrepareExpectedDealBillingSummaryRecordsOutput DoWorkWithResult(PrepareExpectedDealBillingSummaryRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            var expectedDealBillingSummaryRecordDict = new Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>();

            List<DealZoneGroupData> dealZoneGroupData = new DealDetailedProgressManager().GetDealZoneGroupDataBeforeDate(inputArgument.IsSale, inputArgument.BeginDate);
            Dictionary<DealZoneGroup, DealProgressData> dealProgressDataByZoneGroup = BuildDealProgressDataByZoneGroup(inputArgument.IsSale, dealZoneGroupData);

            foreach (var kvp_baseDealBillingSummary in inputArgument.ExpectedBaseDealBillingSummaryRecords)
            {
                DealZoneGroup dealZoneGroup = kvp_baseDealBillingSummary.Key;
                IOrderedEnumerable<BaseDealBillingSummary> orderBaseDealBillingSummaries = kvp_baseDealBillingSummary.Value.Values.OrderBy(itm => itm.BatchStart);

                foreach (var baseDealBillingSummary in orderBaseDealBillingSummaries)
                {
                    DealProgressData dealProgressData;
                    if (!dealProgressDataByZoneGroup.TryGetValue(dealZoneGroup, out dealProgressData)) //1st Tier
                    {
                        DealZoneGroupTier firstDealZoneGroupTier = GetDealZoneGroupTier(inputArgument.IsSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, 0);
                        if (firstDealZoneGroupTier == null)
                            throw new Exception("At least one Tier should be defined by each dealZoneGroup"); //ToDo: DataIntegrity

                        dealProgressData = dealProgressDataByZoneGroup.GetOrCreateItem(dealZoneGroup);
                        dealProgressData.DealID = dealZoneGroup.DealId;
                        dealProgressData.ZoneGroupNb = dealZoneGroup.ZoneGroupNb;
                        dealProgressData.IsSale = inputArgument.IsSale;
                        dealProgressData.CurrentTierNb = firstDealZoneGroupTier.TierNumber;
                        dealProgressData.ReachedDurationInSeconds = 0;
                        dealProgressData.TargetDurationInSeconds = firstDealZoneGroupTier.VolumeInSeconds.HasValue ? firstDealZoneGroupTier.VolumeInSeconds.Value : decimal.MaxValue;

                        BuildExpectedDealBillingSummaryRecords(inputArgument.IsSale, baseDealBillingSummary, dealProgressData, expectedDealBillingSummaryRecordDict);
                    }
                    else
                    {
                        BuildExpectedDealBillingSummaryRecords(inputArgument.IsSale, baseDealBillingSummary, dealProgressData, expectedDealBillingSummaryRecordDict);
                    }
                }
            }

            return new PrepareExpectedDealBillingSummaryRecordsOutput()
            {
                ExpectedDealBillingSummaryRecords = expectedDealBillingSummaryRecordDict
            };
        }

        protected override PrepareExpectedDealBillingSummaryRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExpectedDealBillingSummaryRecordsInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
                ExpectedBaseDealBillingSummaryRecords = this.ExpectedBaseDealBillingSummaryRecords.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareExpectedDealBillingSummaryRecordsOutput result)
        {
            this.ExpectedDealBillingSummaryRecords.Set(context, result.ExpectedDealBillingSummaryRecords);
        }

        private Dictionary<DealZoneGroup, DealProgressData> BuildDealProgressDataByZoneGroup(bool isSale, List<DealZoneGroupData> dealZoneGroupData)
        {
            Dictionary<DealZoneGroup, DealProgressData> dealProgressDataByZoneGroup = new Dictionary<DealZoneGroup, DealProgressData>();

            foreach (var dealZoneGroupDataItem in dealZoneGroupData)
            {
                decimal tierReachedDurationInSec;
                DealZoneGroupTier dealZoneGroupTier = GetUpToVolumeDealZoneGroupTier(isSale, dealZoneGroupDataItem.DealID, dealZoneGroupDataItem.ZoneGroupNb,
                        dealZoneGroupDataItem.TotalReachedDurationInSeconds, out tierReachedDurationInSec);

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealZoneGroupDataItem.DealID, ZoneGroupNb = dealZoneGroupDataItem.ZoneGroupNb };
                DealProgressData dealProgressData = new DealProgressData()
                {
                    DealID = dealZoneGroupDataItem.DealID,
                    ZoneGroupNb = dealZoneGroupDataItem.ZoneGroupNb,
                    IsSale = dealZoneGroupDataItem.IsSale,
                    CurrentTierNb = dealZoneGroupTier.TierNumber,
                    ReachedDurationInSeconds = tierReachedDurationInSec,
                    TargetDurationInSeconds = dealZoneGroupTier.VolumeInSeconds
                };

                dealProgressDataByZoneGroup.Add(dealZoneGroup, dealProgressData);
            }

            return dealProgressDataByZoneGroup;
        }

        private DealZoneGroupTier GetUpToVolumeDealZoneGroupTier(bool isSale, int dealID, int zoneGroupNb, decimal totalZoneGroupDurationInSeconds, out decimal reachedDurationInSec)
        {
            if (isSale)
                return new DealDefinitionManager().GetUpToVolumeDealSaleZoneGroupTier(dealID, zoneGroupNb, totalZoneGroupDurationInSeconds, out reachedDurationInSec);
            else
                return new DealDefinitionManager().GetUpToVolumeDealSupplierZoneGroupTier(dealID, zoneGroupNb, totalZoneGroupDurationInSeconds, out reachedDurationInSec);
        }

        private DealZoneGroupTier GetDealZoneGroupTier(bool isSale, int dealId, int zoneGroupNb, int tierNb)
        {
            if (isSale)
                return new DealDefinitionManager().GetSaleDealZoneGroupTier(dealId, zoneGroupNb, tierNb);
            else
                return new DealDefinitionManager().GetSupplierDealZoneGroupTier(dealId, zoneGroupNb, tierNb);
        }

        private void BuildExpectedDealBillingSummaryRecords(bool isSale, BaseDealBillingSummary baseDealBillingSummary, DealProgressData dealProgressData,
            Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> expectedDealBillingSummaryRecordDict)
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
                    DealBillingSummary dealBillingSummary = new DealBillingSummary()
                    {
                        BatchStart = baseDealBillingSummary.BatchStart,
                        DealId = baseDealBillingSummary.DealId,
                        DealZoneGroupNb = baseDealBillingSummary.DealZoneGroupNb,
                        DealTierNb = dealProgressData.CurrentTierNb,
                        DealRateTierNb = dealProgressData.CurrentTierNb,
                        DurationInSeconds = remainingDurationInSec,
                        IsSale = isSale
                    };
                    AddToExpectedDealBillingSummaryRecordDict(dealBillingSummary, expectedDealBillingSummaryRecordDict);

                    //Editing DealProgressData memoryTable
                    dealProgressData.ReachedDurationInSeconds += remainingDurationInSec;
                    remainingDurationInSec = 0;
                }
                else
                {
                    DealBillingSummary dealBillingSummary = new DealBillingSummary()
                    {
                        BatchStart = baseDealBillingSummary.BatchStart,
                        DealId = baseDealBillingSummary.DealId,
                        DealZoneGroupNb = baseDealBillingSummary.DealZoneGroupNb,
                        DealTierNb = dealProgressData.CurrentTierNb,
                        DealRateTierNb = dealProgressData.CurrentTierNb,
                        DurationInSeconds = remainingTierDurationInSec,
                        IsSale = isSale
                    };
                    AddToExpectedDealBillingSummaryRecordDict(dealBillingSummary, expectedDealBillingSummaryRecordDict);

                    remainingDurationInSec -= remainingTierDurationInSec;

                    //Editing DealProgressData memoryTable
                    DealZoneGroupTier nextDealZoneGroupTier = GetDealZoneGroupTier(isSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, dealProgressData.CurrentTierNb + 1);
                    if (nextDealZoneGroupTier == null)
                    {
                        dealProgressData.ReachedDurationInSeconds = dealProgressData.TargetDurationInSeconds.Value;
                        break;
                    }
                    dealProgressData.CurrentTierNb = nextDealZoneGroupTier.TierNumber;
                    dealProgressData.TargetDurationInSeconds = nextDealZoneGroupTier.VolumeInSeconds;
                    dealProgressData.ReachedDurationInSeconds = 0;
                }
            }
        }

        private void AddToExpectedDealBillingSummaryRecordDict(DealBillingSummary dealBillingSummary, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> expectedDealBillingSummaryRecordDict)
        {
            DealZoneGroupTierRate dealZoneGroupTierRate = new DealZoneGroupTierRate()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb
            };

            Dictionary<DateTime, DealBillingSummary> dealBillingSummaryByBatchStart = expectedDealBillingSummaryRecordDict.GetOrCreateItem(dealZoneGroupTierRate);
            dealBillingSummaryByBatchStart.Add(dealBillingSummary.BatchStart, dealBillingSummary);
        }
    }

    public class DealProgressData
    {
        public int DealID { get; set; }

        public int ZoneGroupNb { get; set; }

        public bool IsSale { get; set; }

        public int CurrentTierNb { get; set; }

        public decimal ReachedDurationInSeconds { get; set; }

        public decimal? TargetDurationInSeconds { get; set; }
    }
}
