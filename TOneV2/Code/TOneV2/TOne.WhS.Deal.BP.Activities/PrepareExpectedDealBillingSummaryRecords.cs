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
                        DealZoneGroupTierDetails firstDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(inputArgument.IsSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, 0);
                        if (firstDealZoneGroupTierDetails == null)
                            throw new Exception("At least one Tier should be defined by each dealZoneGroup"); //ToDo: DataIntegrity

                        dealProgressData = dealProgressDataByZoneGroup.GetOrCreateItem(dealZoneGroup);
                        dealProgressData.DealID = dealZoneGroup.DealId;
                        dealProgressData.ZoneGroupNb = dealZoneGroup.ZoneGroupNb;
                        dealProgressData.IsSale = inputArgument.IsSale;
                        dealProgressData.CurrentTierNb = firstDealZoneGroupTierDetails.TierNumber;
                        dealProgressData.ReachedDurationInSeconds = 0;
                        dealProgressData.TargetDurationInSeconds = firstDealZoneGroupTierDetails.VolumeInSeconds.HasValue ? firstDealZoneGroupTierDetails.VolumeInSeconds.Value : decimal.MaxValue;

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
                DealZoneGroupTierDetails dealZoneGroupTierDetails = GetUpToVolumeDealZoneGroupTierDetails(isSale, dealZoneGroupDataItem.DealID, dealZoneGroupDataItem.ZoneGroupNb,
                        dealZoneGroupDataItem.TotalReachedDurationInSeconds, out tierReachedDurationInSec);

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealZoneGroupDataItem.DealID, ZoneGroupNb = dealZoneGroupDataItem.ZoneGroupNb };
                DealProgressData dealProgressData = new DealProgressData()
                {
                    DealID = dealZoneGroupDataItem.DealID,
                    ZoneGroupNb = dealZoneGroupDataItem.ZoneGroupNb,
                    IsSale = dealZoneGroupDataItem.IsSale,
                    CurrentTierNb = dealZoneGroupTierDetails.TierNumber,
                    ReachedDurationInSeconds = tierReachedDurationInSec,
                    TargetDurationInSeconds = dealZoneGroupTierDetails.VolumeInSeconds
                };

                dealProgressDataByZoneGroup.Add(dealZoneGroup, dealProgressData);
            }

            return dealProgressDataByZoneGroup;
        }

        private DealZoneGroupTierDetails GetUpToVolumeDealZoneGroupTierDetails(bool isSale, int dealID, int zoneGroupNb, decimal totalZoneGroupDurationInSeconds, out decimal reachedDurationInSec)
        {
            if (isSale)
                return new DealDefinitionManager().GetUpToVolumeDealSaleZoneGroupTierDetails(dealID, zoneGroupNb, totalZoneGroupDurationInSeconds, out reachedDurationInSec);
            else
                return new DealDefinitionManager().GetUpToVolumeDealSupplierZoneGroupTierDetails(dealID, zoneGroupNb, totalZoneGroupDurationInSeconds, out reachedDurationInSec);
        }

        private DealZoneGroupTierDetails GetDealZoneGroupTierDetails(bool isSale, int dealId, int zoneGroupNb, int tierNb)
        {
            if (isSale)
                return new DealDefinitionManager().GetSaleDealZoneGroupTierDetails(dealId, zoneGroupNb, tierNb);
            else
                return new DealDefinitionManager().GetSupplierDealZoneGroupTierDetails(dealId, zoneGroupNb, tierNb);
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
                    DealZoneGroupTierDetails nextDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(isSale, baseDealBillingSummary.DealId, baseDealBillingSummary.DealZoneGroupNb, dealProgressData.CurrentTierNb + 1);
                    if (nextDealZoneGroupTierDetails == null)
                    {
                        dealProgressData.ReachedDurationInSeconds = dealProgressData.TargetDurationInSeconds.Value;
                        break;
                    }
                    dealProgressData.CurrentTierNb = nextDealZoneGroupTierDetails.TierNumber;
                    dealProgressData.TargetDurationInSeconds = nextDealZoneGroupTierDetails.VolumeInSeconds;
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
