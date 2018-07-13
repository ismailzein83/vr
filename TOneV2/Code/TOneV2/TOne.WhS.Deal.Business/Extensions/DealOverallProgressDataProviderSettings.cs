using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealOverallProgressDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("1BCAF6A3-1B38-41CB-9E70-08D9D1D570B5"); } }

        public override bool DoesSupportFilterOnAllFields
        {
            get { return false; }
        }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            var saleSwapDealInfoByDealId = new OverallDealnfoByDealId();
            var costSwapDealInfoByDealId = new OverallDealnfoByDealId();

            var deals = new SwapDealManager().GetSwapDealsBetweenDate(context.FromTime.Value, context.ToTime.Value);

            if (deals == null)
                return;

            var minDealBED = DateTime.Now;
            foreach (var dealDefinition in deals)
            {
                SwapDealSettings swapDealSetting = dealDefinition.Settings as SwapDealSettings;

                if (swapDealSetting == null)
                    continue;

                int? dealDays = null;
                if (swapDealSetting.EndDate.HasValue)
                    dealDays = (swapDealSetting.EndDate.Value - swapDealSetting.BeginDate).Days;

                int daysToEnd = 0;
                if (swapDealSetting.EndDate.HasValue)
                {
                    DateTime endDate = swapDealSetting.EndDate.Value;
                    daysToEnd = DateTime.Now > endDate ? 0 : (endDate - DateTime.Now).Days;
                }

                minDealBED = minDealBED > swapDealSetting.BeginDate
                  ? swapDealSetting.BeginDate
                  : minDealBED;

                OverallDealInfoByGroupNb saleProgressByGroupNb = saleSwapDealInfoByDealId.GetOrCreateItem(dealDefinition.DealId);
                OverallDealInfoByGroupNb costProgressByGroupNb = costSwapDealInfoByDealId.GetOrCreateItem(dealDefinition.DealId);

                foreach (var swapDealInbound in swapDealSetting.Inbounds)
                {
                    OverallDealInfo saleDealInfo = new OverallDealInfo
                    {
                        CarrierAccountId = swapDealSetting.CarrierAccountId,
                        DealId = dealDefinition.DealId,
                        DealBED = swapDealSetting.BeginDate,
                        DealEED = swapDealSetting.EndDate,
                        Rate = swapDealInbound.Rate,
                        DealDays = dealDays,
                        EstimatedVolume = swapDealInbound.Volume,
                        GroupName = swapDealInbound.Name,
                        ExtraVolumeRate = swapDealInbound.ExtraVolumeRate,
                        EstimatedAmount = swapDealInbound.Volume * swapDealInbound.Rate,
                        Status = swapDealSetting.Status,
                        RemainingDays = daysToEnd
                    };
                    saleProgressByGroupNb.Add(swapDealInbound.ZoneGroupNumber, saleDealInfo);
                }
                foreach (var swapDealOutbound in swapDealSetting.Outbounds)
                {
                    OverallDealInfo saleDealInfo = new OverallDealInfo
                    {
                        CarrierAccountId = swapDealSetting.CarrierAccountId,
                        DealId = dealDefinition.DealId,
                        DealBED = swapDealSetting.BeginDate,
                        DealEED = swapDealSetting.EndDate,
                        Rate = swapDealOutbound.Rate,
                        DealDays = dealDays,
                        GroupName = swapDealOutbound.Name,
                        EstimatedVolume = swapDealOutbound.Volume,
                        ExtraVolumeRate = swapDealOutbound.ExtraVolumeRate,
                        EstimatedAmount = swapDealOutbound.Volume * swapDealOutbound.Rate,
                        Status = swapDealSetting.Status,
                        RemainingDays = daysToEnd
                    };
                    costProgressByGroupNb.Add(swapDealOutbound.ZoneGroupNumber, saleDealInfo);
                }
            }

            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord analyticRecordSummary;

            AnalyticQuery saleAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigSaleDealZoneGroupNb", "SaleDealZoneGroupNb", "DayAsDate", "OrigSaleDeal", "SaleDeal", "SaleDealTierNb" },
                MeasureFields = new List<string> { "SaleDuration", "SaleRate" },
                FromTime = minDealBED,
                ToTime = context.ToTime.Value
            };
            List<AnalyticRecord> saleRecords = analyticManager.GetAllFilteredRecords(saleAnalyticQuery, out analyticRecordSummary);
            IEnumerable<DataRecordObject> saleDataRecordObjects = GetAnalyticRecords(saleRecords, saleSwapDealInfoByDealId, context.ToTime.Value, "IN");

            foreach (var dataRecord in saleDataRecordObjects)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }

            AnalyticQuery costAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigCostDealZoneGroupNb", "CostDealZoneGroupNb", "DayAsDate", "OrigCostDeal", "CostDeal", "CostDealTierNb" },
                MeasureFields = new List<string> { "CostDuration", "CostRate" },
                FromTime = minDealBED,
                ToTime = context.ToTime.Value
            };
            List<AnalyticRecord> costRecords = analyticManager.GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);
            IEnumerable<DataRecordObject> costDataRecordObjects = GetAnalyticRecords(costRecords, costSwapDealInfoByDealId, context.ToTime.Value, "OUT");

            foreach (var dataRecord in costDataRecordObjects)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }
        }

        private IEnumerable<DataRecordObject> GetAnalyticRecords(List<AnalyticRecord> analyticRecords, OverallDealnfoByDealId swapDealInfoByDealId, DateTime toDateDate, string Direction)
        {
            var salePricedTrafficByDealId = new OverallDealnfoByDealId();
            var salePricedTrafficByDealIdPlus = new OverallDealnfoByDealId();

            foreach (var saleRecord in analyticRecords)
            {
                var origSaleDealZoneGroupNb = saleRecord.DimensionValues[0];
                var saleDealZoneGroupNb = saleRecord.DimensionValues[1];
                var origSaleDealId = saleRecord.DimensionValues[3];
                var saleDealId = saleRecord.DimensionValues[4];
                var saleDealTierNb = saleRecord.DimensionValues[5];

                decimal saleDurationValue = 0;
                MeasureValue saleDuration;
                saleRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                if (saleDuration != null && saleDuration.Value != null)
                    saleDurationValue = Convert.ToDecimal(saleDuration.Value ?? 0.0);

                var day = saleRecord.DimensionValues[2];
                DateTime dateTimeValue;
                DateTime.TryParse(day.Name, out dateTimeValue);

                if (origSaleDealId != null && origSaleDealId.Value != null)
                {
                    if (saleDealId != null && saleDealId.Value != null)
                    {
                        int pricedDealId = (int)saleDealId.Value;
                        OverallDealInfoByGroupNb saleProgressByGroupNb;
                        if (swapDealInfoByDealId.TryGetValue(pricedDealId, out saleProgressByGroupNb))
                        {
                            int pricedGroupNb = (int)saleDealZoneGroupNb.Value;
                            OverallDealInfo dealInfo = saleProgressByGroupNb.GetRecord(pricedGroupNb);

                            int tierNb = (int)saleDealTierNb.Value;
                            switch (tierNb)
                            {
                                case 1:
                                    AddOrUpdateDealInfo(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, dealInfo, Direction, salePricedTrafficByDealId, toDateDate);
                                    break;
                                case 2:
                                    AddOrUpdateDealInfo(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, dealInfo, string.Format("{0}+", Direction), salePricedTrafficByDealIdPlus, toDateDate);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        int origSaleDealIdValue = (int)origSaleDealId.Value;
                        OverallDealInfoByGroupNb saleProgressByGroupNb;
                        if (swapDealInfoByDealId.TryGetValue(origSaleDealIdValue, out saleProgressByGroupNb))
                        {
                            int groupNb = (int)origSaleDealZoneGroupNb.Value;
                            OverallDealInfo dealInfo = saleProgressByGroupNb.GetRecord(groupNb);
                            AddOrUpdateDealInfo(origSaleDealIdValue, groupNb, saleDurationValue, dateTimeValue, dealInfo, string.Format("{0}+", Direction), salePricedTrafficByDealIdPlus, toDateDate);
                        }
                    }
                }
            }
            IEnumerable<DataRecordObject> dataRecords = salePricedTrafficByDealId.Values.SelectMany(progress => progress.Values)
                                                         .Select(DataRecordObjectMapper);
            return dataRecords;
        }
        private void AddOrUpdateDealInfo(int dealId, int groupNb, decimal saleDurationValue, DateTime dateTimeValue, OverallDealInfo dealInfo, string direction, OverallDealnfoByDealId trafficByDealId, DateTime toDateDate)
        {
            OverallDealInfoByGroupNb saleTrafficByGroupNb = trafficByDealId.GetOrCreateItem(dealId);

            OverallDealInfo pricedDealInfo;
            if (!saleTrafficByGroupNb.TryGetValue(groupNb, out pricedDealInfo))
            {
                pricedDealInfo = new OverallDealInfo
                {
                    CarrierAccountId = dealInfo.CarrierAccountId,
                    Direction = direction,
                    EstimatedVolume = dealInfo.EstimatedVolume,
                    GroupName = dealInfo.GroupName,
                    DealDays = dealInfo.DealDays,
                    DealRate = dealInfo.Rate,
                    DealBED = dealInfo.DealBED,
                    DealEED = dealInfo.DealEED,
                    DealId = dealInfo.DealId,
                    Status = dealInfo.Status,
                    RemainingDays = dealInfo.RemainingDays
                };
                saleTrafficByGroupNb.Add(groupNb, pricedDealInfo);
            }
            if (toDateDate == dateTimeValue)
                pricedDealInfo.ToDateVolume += saleDurationValue;

            pricedDealInfo.ReachedVolume += saleDurationValue;

            pricedDealInfo.RemainingVolume = pricedDealInfo.EstimatedVolume - pricedDealInfo.ReachedVolume;
            pricedDealInfo.RemainingVolumePrecentage = pricedDealInfo.EstimatedVolume == 0
                                                     ? 0
                                                     : (pricedDealInfo.ReachedVolume / pricedDealInfo.EstimatedVolume) * 100;
            pricedDealInfo.DealAmount = pricedDealInfo.EstimatedVolume * pricedDealInfo.DealRate;
            pricedDealInfo.ReachedAmount = pricedDealInfo.ReachedAmount * pricedDealInfo.DealRate;
            pricedDealInfo.ToDateVolume = pricedDealInfo.ToDateVolume * pricedDealInfo.DealRate;

            pricedDealInfo.ExpectedDays = pricedDealInfo.ToDateVolume == 0
                                        ? 0
                                        : (int)(pricedDealInfo.RemainingVolume / pricedDealInfo.ToDateVolume);
        }

        private DataRecordObject DataRecordObjectMapper(OverallDealInfo dealInfo)
        {
            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"CarrierAccount", dealInfo.CarrierAccountId},
                {"TrafficType",dealInfo.Direction},
                {"Deal",dealInfo.DealId},
                {"GroupName", dealInfo.GroupName},
                {"EstimatedVolume", dealInfo.EstimatedVolume},
                {"ReachedVolume", dealInfo.ReachedVolume},
                {"ToDateVolume", dealInfo.ToDateVolume},
                {"RemainngVolume", dealInfo.RemainingVolume},
                {"RemaingVolumePerc", dealInfo.RemainingVolumePrecentage},
                {"RemainingDays", dealInfo.RemainingDays},
                {"RemainingPerDays", dealInfo.RemainingPerDays},
                {"ExpectedDays", dealInfo.ExpectedDays},
                {"Rate", dealInfo.DealRate},
                {"EstimatedAmount", dealInfo.EstimatedAmount},
                {"ReachedAmount", dealInfo.ReachedAmount},
                {"DealBED", dealInfo.DealBED},
                {"DealEED", dealInfo.DealEED},
                {"Status", dealInfo.Status},
                {"Notes", dealInfo.Notes}
            };
            return new DataRecordObject(new Guid("AB46069D-0FFC-4027-9CCF-BD1AF8EC91F7"), swapDealProgressObject);
        }
    }

    #region public Class

    public class OverallDealnfoByDealId : Dictionary<Int32, OverallDealInfoByGroupNb>
    {

    }

    public class OverallDealInfoByGroupNb : Dictionary<Int32, OverallDealInfo>
    {

    }

    public class OverallDealInfo
    {
        public long DealId { get; set; }
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public DealStatus Status { get; set; }
        public string GroupName { get; set; }
        public int CarrierAccountId { get; set; }
        public decimal ReachedVolume { get; set; }
        public int EstimatedVolume { get; set; }
        public decimal ToDateVolume { get; set; }
        public decimal Rate { get; set; }
        public decimal? ExtraVolumeRate { get; set; }
        public int? DealDays { get; set; }
        public string Direction { get; set; }
        public decimal RemainingVolume { get; set; }
        public decimal RemainingVolumePrecentage { get; set; }
        public int Days { get; set; }
        public int RemainingDays { get; set; }
        public int RemainingPerDays { get; set; }
        public int ExpectedDays { get; set; }
        public int VolumeDaysExpected { get; set; }
        public int VolumeRemainingPerDay { get; set; }
        public decimal DealRate { get; set; }
        public decimal DealAmount { get; set; }
        public decimal EstimatedAmount { get; set; }
        public decimal ReachedAmount { get; set; }
        public decimal ToDateAmount { get; set; }
        public string Notes { get; set; }

    }
    #endregion
}
