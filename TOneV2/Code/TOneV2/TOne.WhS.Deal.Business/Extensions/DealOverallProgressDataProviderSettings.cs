using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;
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

        private enum PropertyName
        {
            Duration,
            Rate
        }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            var deals = new SwapDealManager().GetSwapDealsBetweenDate(context.FromTime.Value, context.ToTime.Value);

            if (deals == null)
                return;

            var saleDealInfos = new List<BaseDealInfo>();
            var costDealInfos = new List<BaseDealInfo>();

            var minDealBED = DateTime.Now;
            foreach (var dealDefinition in deals)
            {
                SwapDealSettings swapDealSetting = dealDefinition.Settings as SwapDealSettings;
                swapDealSetting.ThrowIfNull("dealDefinition.Settings", dealDefinition.DealId);

                int dealId = dealDefinition.DealId;
                DateTime dealBED = swapDealSetting.BeginDate;
                DateTime? dealEED = swapDealSetting.EndDate;
                int carrierAccountId = swapDealSetting.CarrierAccountId;

                int? dealDays = null;
                int daysToEnd = 0;

                if (dealEED.HasValue)
                {
                    dealDays = (dealEED.Value - dealBED).Days;
                    DateTime endDate = dealEED.Value;
                    daysToEnd = DateTime.Now > endDate ? 0 : (endDate - DateTime.Now).Days;
                }

                minDealBED = minDealBED > dealBED
                    ? dealBED
                    : minDealBED;

                foreach (var swapDealInbound in swapDealSetting.Inbounds)
                {
                    BaseDealInfo saleDealInfo = new BaseDealInfo
                    {
                        CarrierAccountId = carrierAccountId,
                        DealId = dealId,
                        DealBED = dealBED,
                        DealEED = dealEED,
                        Rate = swapDealInbound.Rate,
                        DealDays = dealDays,
                        EstimatedVolume = swapDealInbound.Volume,
                        GroupName = swapDealInbound.Name,
                        ExtraVolumeRate = swapDealInbound.ExtraVolumeRate,
                        EstimatedAmount = swapDealInbound.Volume * swapDealInbound.Rate,
                        Status = swapDealSetting.Status,
                        RemainingDays = daysToEnd,
                        GroupNumber = swapDealInbound.ZoneGroupNumber
                    };
                    saleDealInfos.Add(saleDealInfo);
                }
                foreach (var swapDealOutbound in swapDealSetting.Outbounds)
                {
                    BaseDealInfo costDealInfo = new BaseDealInfo
                    {
                        CarrierAccountId = carrierAccountId,
                        DealId = dealId,
                        DealBED = dealBED,
                        DealEED = dealEED,
                        Rate = swapDealOutbound.Rate,
                        DealDays = dealDays,
                        GroupName = swapDealOutbound.Name,
                        EstimatedVolume = swapDealOutbound.Volume,
                        ExtraVolumeRate = swapDealOutbound.ExtraVolumeRate,
                        EstimatedAmount = swapDealOutbound.Volume * swapDealOutbound.Rate,
                        Status = swapDealSetting.Status,
                        RemainingDays = daysToEnd,
                        GroupNumber = swapDealOutbound.ZoneGroupNumber
                    };
                    costDealInfos.Add(costDealInfo);
                }
            }
            DateTime fromTime = context.FromTime.Value < minDealBED ? context.FromTime.Value : minDealBED;

            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord analyticRecordSummary;
            AnalyticQuery saleAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigSaleDealZoneGroupNb", "SaleDealZoneGroupNb", "DayAsDate", "OrigSaleDeal", "SaleDeal", "SaleDealTierNb" },
                MeasureFields = new List<string> { "SaleDuration", "TotalSaleRateDuration" },
                FromTime = fromTime,
                ToTime = context.ToTime.Value
            };

            List<AnalyticRecord> saleRecords = analyticManager.GetAllFilteredRecords(saleAnalyticQuery, out analyticRecordSummary);
            IEnumerable<DataRecordObject> saleDataRecordObjects = GetBillingRecords(saleDealInfos, saleRecords, context.ToTime.Value, true);

            foreach (var dataRecord in saleDataRecordObjects)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }

            AnalyticQuery costAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigCostDealZoneGroupNb", "CostDealZoneGroupNb", "DayAsDate", "OrigCostDeal", "CostDeal", "CostDealTierNb" },
                MeasureFields = new List<string> { "CostDuration", "TotalCostRateDuration" },
                FromTime = fromTime,
                ToTime = context.ToTime.Value
            };

            List<AnalyticRecord> costRecords = analyticManager.GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);
            IEnumerable<DataRecordObject> costDataRecordObjects = GetBillingRecords(costDealInfos, costRecords, context.ToTime.Value, false);

            foreach (var dataRecord in costDataRecordObjects)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }
        }
        private IEnumerable<DataRecordObject> GetBillingRecords(List<BaseDealInfo> dealInfos, List<AnalyticRecord> analyticRecords, DateTime toDateDate, bool isSale)
        {
            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale);
            BillingDataByDealId trafficByDealId = new BillingDataByDealId();
            BillingDataByDealId trafficByDealIdPlus = new BillingDataByDealId();

            foreach (var record in analyticRecords)
            {
                var origSaleDealZoneGroupNb = record.DimensionValues[0];
                var saleDealZoneGroupNb = record.DimensionValues[1];
                var origSaleDealId = record.DimensionValues[3];
                var saleDealId = record.DimensionValues[4];
                var saleDealTierNb = record.DimensionValues[5];

                decimal saleDurationValue = 0;
                MeasureValue saleDuration;
                record.MeasureValues.TryGetValue(propertyNames[PropertyName.Duration], out saleDuration);
                if (saleDuration != null && saleDuration.Value != null)
                    saleDurationValue = Convert.ToDecimal(saleDuration.Value ?? 0.0);

                decimal rateValue = 0;
                MeasureValue rate;
                record.MeasureValues.TryGetValue(propertyNames[PropertyName.Rate], out rate);
                if (rate != null && rate.Value != null)
                    rateValue = Convert.ToDecimal(rate.Value ?? 0.0);

                var day = record.DimensionValues[2];
                DateTime dateTimeValue;
                DateTime.TryParse(day.Name, out dateTimeValue);

                if (origSaleDealId != null && origSaleDealId.Value != null)
                {
                    if (saleDealId != null && saleDealId.Value != null)
                    {
                        int pricedDealId = (int)saleDealId.Value;
                        int pricedGroupNb = (int)saleDealZoneGroupNb.Value;
                        int tierNb = (int)saleDealTierNb.Value;
                        switch (tierNb)
                        {
                            case 1:
                                AddOrUpdateBilling(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, null, trafficByDealId, toDateDate);
                                break;
                            case 2:
                                AddOrUpdateBilling(pricedDealId, pricedGroupNb, saleDurationValue, dateTimeValue, null, trafficByDealIdPlus, toDateDate);
                                break;
                        }
                    }
                    else
                    {
                        int origSaleDealIdValue = (int)origSaleDealId.Value;
                        int groupNb = (int)origSaleDealZoneGroupNb.Value;
                        AddOrUpdateBilling(origSaleDealIdValue, groupNb, saleDurationValue, dateTimeValue, rateValue, trafficByDealIdPlus, toDateDate);
                    }
                }
            }

            string direction = isSale ? "IN" : "OUT";
            List<DataRecordObject> records = GetDataRecordObject(dealInfos, trafficByDealId, direction);
            List<DataRecordObject> recordsPlus = GetDataRecordObjectPlusTraffic(dealInfos, trafficByDealIdPlus, string.Format("{0}+", direction));

            return records.Concat(recordsPlus);
        }
        private List<DataRecordObject> GetDataRecordObject(List<BaseDealInfo> dealInfos, BillingDataByDealId trafficByDealId, string direction)
        {
            List<DataRecordObject> datarecords = new List<DataRecordObject>();
            foreach (var deal in dealInfos)
            {
                BillingDataByGroupNb billingDataByGroupNb;
                if (trafficByDealId.TryGetValue(deal.DealId, out billingDataByGroupNb))
                {
                    BillingData billingData;
                    if (billingDataByGroupNb.TryGetValue(deal.GroupNumber, out billingData))
                    {
                        DataRecordObject dataRecordObject = CreateDataRecordObject(deal, direction, billingData, deal.Rate);
                        datarecords.Add(dataRecordObject);
                    }
                }
                else
                {
                    DataRecordObject dataRecordObject = CreateDataRecordObject(deal, direction, null, deal.Rate);
                    datarecords.Add(dataRecordObject);
                }
            }
            return datarecords;
        }
        private List<DataRecordObject> GetDataRecordObjectPlusTraffic(List<BaseDealInfo> dealInfos, BillingDataByDealId trafficByDealId, string direction)
        {
            List<DataRecordObject> datarecords = new List<DataRecordObject>();
            foreach (var deal in dealInfos)
            {
                BillingDataByGroupNb billingDataByGroupNb;
                if (trafficByDealId.TryGetValue(deal.DealId, out billingDataByGroupNb))
                {
                    BillingData billingData;
                    if (billingDataByGroupNb.TryGetValue(deal.GroupNumber, out billingData))
                    {
                        DataRecordObject dataRecordObject = CreatePlusDataRecordObject(deal, direction, billingData, deal.ExtraVolumeRate);
                        datarecords.Add(dataRecordObject);
                    }
                }
            }
            return datarecords;
        }
        private void AddOrUpdateBilling(int dealId, int groupNb, decimal saleDurationValue, DateTime dateTimeValue, decimal? rate, BillingDataByDealId billingDataByDealId, DateTime toDateDate)
        {
            BillingDataByGroupNb billingByGroupNb = billingDataByDealId.GetOrCreateItem(dealId);
            BillingData billing = billingByGroupNb.GetOrCreateItem(groupNb);

            if (toDateDate.Date == dateTimeValue.Date)
                billing.ToDateVolume += saleDurationValue;

            billing.ReachedVolume += saleDurationValue;
            if (rate.HasValue) billing.TotalRateDuration = rate.Value;
        }
        private DataRecordObject CreatePlusDataRecordObject(BaseDealInfo dealInfo, string direction, BillingData billingData, decimal? dealRate)
        {
            decimal reachedVolume = 0, toDateVolume = 0, rate = 0, remainingVolumePrecentage = 0;

            if (billingData != null)
            {
                reachedVolume = billingData.ReachedVolume;
                toDateVolume = billingData.ToDateVolume;
                if (billingData.TotalRateDuration.HasValue)
                {
                    decimal totalRate = billingData.TotalRateDuration.Value;
                    rate = totalRate / billingData.ReachedVolume;
                }
                else rate = dealRate.Value;
            }
            if (dealRate.HasValue)
                rate = dealRate.Value;

            decimal reachedAmount = reachedVolume * rate;

            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"CarrierAccount", dealInfo.CarrierAccountId},
                {"TrafficType", direction},
                {"Deal", dealInfo.DealId},
                {"GroupName", dealInfo.GroupName},
                {"EstimatedVolume", 0},
                {"ReachedVolume", reachedVolume},
                {"ToDateVolume", toDateVolume},
                {"RemainngVolume", 0},
                {"RemaingVolumePerc", remainingVolumePrecentage},
                {"RemainingDays", 0},
                {"RemainingPerDays", 0},
                {"ExpectedDays", 0},
                {"Rate", rate},
                {"EstimatedAmount",0},
                {"ReachedAmount", reachedAmount},
                {"DealBED", dealInfo.DealBED},
                {"DealEED", dealInfo.DealEED},
                {"Status", dealInfo.Status},
                {"Notes", dealInfo.Notes}
            };
            return new DataRecordObject(new Guid("1d21b26c-9541-4204-8440-cd8fccf11c61"), swapDealProgressObject);
        }

        private DataRecordObject CreateDataRecordObject(BaseDealInfo dealInfo, string direction, BillingData billingData, decimal dealRate)
        {
            decimal reachedVolume = 0, toDateVolume = 0, remainingVolume = 0, remainingVolumePrecentage = 0, remainingVolumePerDays = 0;

            if (billingData != null)
            {
                reachedVolume = billingData.ReachedVolume;
                toDateVolume = billingData.ToDateVolume;
                remainingVolume = dealInfo.EstimatedVolume - billingData.ReachedVolume;
                remainingVolumePrecentage = remainingVolume == 0
                    ? 0
                    : remainingVolume / dealInfo.EstimatedVolume * 100;
                remainingVolumePerDays = dealInfo.RemainingDays == 0 ? 0 : remainingVolume / dealInfo.RemainingDays;
            }

            decimal expectedDays = toDateVolume == 0 || remainingVolume == 0
                                    ? 0
                                    : (int)(remainingVolume / toDateVolume);

            decimal reachedAmount = reachedVolume * dealRate;

            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"CarrierAccount", dealInfo.CarrierAccountId},
                {"TrafficType", direction},
                {"Deal", dealInfo.DealId},
                {"GroupName", dealInfo.GroupName},
                {"EstimatedVolume", dealInfo.EstimatedVolume},
                {"ReachedVolume", reachedVolume},
                {"ToDateVolume", toDateVolume},
                {"RemainngVolume", remainingVolume},
                {"RemaingVolumePerc", remainingVolumePrecentage},
                {"RemainingDays", dealInfo.RemainingDays < 0 ? 0 : dealInfo.RemainingDays},
                {"RemainingPerDays", remainingVolumePerDays},
                {"ExpectedDays", expectedDays},
                {"Rate", dealRate},
                {"EstimatedAmount", dealInfo.EstimatedAmount},
                {"ReachedAmount", reachedAmount},
                {"DealBED", dealInfo.DealBED},
                {"DealEED", dealInfo.DealEED},
                {"Status", dealInfo.Status},
                {"Notes", dealInfo.Notes}
            };
            return new DataRecordObject(new Guid("1d21b26c-9541-4204-8440-cd8fccf11c61"), swapDealProgressObject);
        }
        private Dictionary<PropertyName, string> BuildPropertyNames(bool isSale)
        {
            string prefix = isSale ? "Sale" : "Cost";
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>
            {
                {PropertyName.Duration, string.Format("{0}Duration", prefix)},
                {PropertyName.Rate, string.Format("Total{0}RateDuration", prefix)}
            };
            return propertyNames;
        }
    }

    #region public Class

    public class BillingDataByDealId : Dictionary<Int32, BillingDataByGroupNb>
    {

    }

    public class BillingDataByGroupNb : Dictionary<Int32, BillingData>
    {

    }
    public class BaseDealInfo
    {
        public int DealId { get; set; }
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public DealStatus Status { get; set; }
        public string GroupName { get; set; }
        public int CarrierAccountId { get; set; }
        public int EstimatedVolume { get; set; }
        public decimal Rate { get; set; }
        public decimal? ExtraVolumeRate { get; set; }
        public int? DealDays { get; set; }
        public int RemainingDays { get; set; }
        public decimal EstimatedAmount { get; set; }
        public string Notes { get; set; }
        public int GroupNumber { get; set; }
    }
    public class BillingData
    {
        public decimal ToDateVolume { get; set; }
        public decimal ReachedVolume { get; set; }
        public decimal? TotalRateDuration { get; set; }
    }
    #endregion
}
