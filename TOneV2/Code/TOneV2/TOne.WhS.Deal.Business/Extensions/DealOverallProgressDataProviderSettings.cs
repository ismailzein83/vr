using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
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

            var minDealBED = DateTime.Now;
            foreach (var dealDefinition in deals)
            {
                SwapDealSettings swapDealSetting = dealDefinition.Settings as SwapDealSettings;

                if (swapDealSetting == null)
                    continue;

                int? dealDays = null;
                if (swapDealSetting.EndDate.HasValue)
                    dealDays = (swapDealSetting.EndDate.Value - swapDealSetting.BeginDate).Days;

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
                        DealVolume = swapDealInbound.Volume,
                        GroupName = swapDealInbound.Name,
                        ExtraVolumeRate = swapDealInbound.ExtraVolumeRate
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
                        DealVolume = swapDealOutbound.Volume,
                        ExtraVolumeRate = swapDealOutbound.ExtraVolumeRate
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
            IEnumerable<DataRecordObject> saleDataRecordObjects = GetAnalyticRecords(saleRecords, saleSwapDealInfoByDealId);

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
            IEnumerable<DataRecordObject> costDataRecordObjects = GetAnalyticRecords(costRecords, costSwapDealInfoByDealId);

            foreach (var dataRecord in costDataRecordObjects)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }
        }

        private IEnumerable<DataRecordObject> GetAnalyticRecords(List<AnalyticRecord> analyticRecords, OverallDealnfoByDealId swapDealInfoByDealId)
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

                MeasureValue saleDuration;
                saleRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                decimal saleDurationValue = Convert.ToDecimal(saleDuration.Value ?? 0.0);

                var day = saleRecord.DimensionValues[2];
                DateTime dateTimeValue;
                DateTime.TryParse(day.Name, out dateTimeValue);

                if (origSaleDealId != null)
                {
                    if (saleDealId != null)
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
                                    AddOrUpdateDealInfo(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, dealInfo, "IN", salePricedTrafficByDealId);
                                    break;
                                case 2:
                                    AddOrUpdateDealInfo(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, dealInfo, "IN+", salePricedTrafficByDealIdPlus);
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
                            AddOrUpdateDealInfo(origSaleDealIdValue, groupNb, saleDurationValue, dateTimeValue, dealInfo, "IN+", salePricedTrafficByDealIdPlus);
                        }
                    }
                }
                MeasureValue duration;
                saleRecord.MeasureValues.TryGetValue("SaleDuration", out duration);
            }
            IEnumerable<DataRecordObject> dataRecords = salePricedTrafficByDealId.Values.SelectMany(
                progress => progress.Values)
                .Select(DataRecordObjectMapper);
            return dataRecords;
        }
        private void AddOrUpdateDealInfo(int dealId, int groupNb, decimal saleDurationValue, DateTime dateTimeValue, OverallDealInfo dealInfo, string direction, OverallDealnfoByDealId trafficByDealId)
        {
            OverallDealInfoByGroupNb saleTrafficByGroupNb = trafficByDealId.GetOrCreateItem(dealId);

            OverallDealInfo pricedDealInfo;
            if (!saleTrafficByGroupNb.TryGetValue(groupNb, out pricedDealInfo))
            {
                pricedDealInfo = new OverallDealInfo
                {
                    CarrierName = new CarrierAccountManager().GetCarrierAccountName(dealInfo.CarrierAccountId),
                    Direction = direction,
                    DealVolume = dealInfo.DealVolume,
                    GroupName = dealInfo.GroupName,
                    DealDays = dealInfo.DealDays,
                    DealRate = dealInfo.Rate
                };
                saleTrafficByGroupNb.Add(groupNb, pricedDealInfo);
            }
            pricedDealInfo.PassedVolume += saleDurationValue;
            if (DateTime.Today == dateTimeValue)
                pricedDealInfo.TodayVolume += saleDurationValue;
            pricedDealInfo.RemainingVolume = pricedDealInfo.DealVolume - pricedDealInfo.PassedVolume;
            pricedDealInfo.RemainingVolumePrecentage = (pricedDealInfo.PassedVolume / pricedDealInfo.DealVolume) * 100;
            pricedDealInfo.DealAmount = pricedDealInfo.DealVolume * pricedDealInfo.DealRate;
            pricedDealInfo.PassedAmount = pricedDealInfo.PassedVolume * pricedDealInfo.DealRate;
            pricedDealInfo.TodayAmount = pricedDealInfo.TodayAmount * pricedDealInfo.DealRate;
        }

        private DataRecordObject DataRecordObjectMapper(OverallDealInfo dealInfo)
        {
            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"Carrier", dealInfo.CarrierName},
                {"Direction",dealInfo.Direction},
                {"GroupName", dealInfo.GroupName},
                {"DealVolume", dealInfo.DealVolume},
                {"PassedVolume", dealInfo.PassedVolume},
                {"TodaysVolume", dealInfo.TodayVolume},
                {"RemainngVolume", dealInfo.RemainingVolume},
                {"RemaingVolumePrec", dealInfo.RemainingVolumePrecentage},
                {"Days", dealInfo.DealDays},
                {"Rate", dealInfo.DealRate},
                {"PassedAmount", dealInfo.PassedAmount},
                {"TodayAmount", dealInfo.TodayAmount},
                {"From", dealInfo.DealBED},
                {"To", dealInfo.DealEED}
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
        public string GroupName { get; set; }
        public int CarrierAccountId { get; set; }
        public int DealVolume { get; set; }
        public decimal Rate { get; set; }
        public decimal? ExtraVolumeRate { get; set; }
        public int? DealDays { get; set; }
        public string CarrierName { get; set; }
        public string Direction { get; set; }
        public decimal PassedVolume { get; set; }
        public decimal TodayVolume { get; set; }
        public decimal RemainingVolume { get; set; }
        public decimal RemainingVolumePrecentage { get; set; }
        public int Days { get; set; }
        public int VolumeDaysExpected { get; set; }
        public int VolumeRemainingPerDay { get; set; }
        public decimal DealRate { get; set; }
        public decimal DealAmount { get; set; }
        public decimal PassedAmount { get; set; }
        public decimal TodayAmount { get; set; }

    }
    #endregion
}
