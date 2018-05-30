using System;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        public override Guid ConfigId
        {
            get { return new Guid("7b5d42b4-b05c-4f54-9683-a73dd0cebd68"); }
        }

        public override bool DoesSupportFilterOnAllFields
        {
            get { return false; }
        }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            var saleProgressByDealId = new SwapDealnfoByDealId();
            var costProgressByDealId = new SwapDealnfoByDealId();
            var dealDetailedProgressManager = new DealDetailedProgressManager();
            var swapDealManager = new SwapDealManager();
            var minDealBED = DateTime.Now;

            IEnumerable<DealDefinition> deals = swapDealManager.GetSwapDealsEffectiveAfterDate(context.FromTime);

            foreach (var dealDefinition in deals)
            {
                SwapDealSettings swapDealSetting = dealDefinition.Settings as SwapDealSettings;

                if (swapDealSetting == null)
                    continue;

                SwapDealInfoByGroupNb saleProgressByGroupNb = saleProgressByDealId.GetOrCreateItem(dealDefinition.DealId);
                SwapDealInfoByGroupNb costProgressByGroupNb = costProgressByDealId.GetOrCreateItem(dealDefinition.DealId);

                minDealBED = minDealBED > swapDealSetting.BeginDate
                    ? swapDealSetting.BeginDate
                    : minDealBED;

                int daysToStart = swapDealSetting.BeginDate > DateTime.Today
                    ? (dealDefinition.Settings.BeginDate - DateTime.Today).Days
                    : 0;

                int daysToEnd = 0, daysToGrace = 0;
                if (swapDealSetting.EndDate.HasValue)
                {
                    DateTime endDate = swapDealSetting.EndDate.Value;
                    daysToEnd = (endDate - DateTime.Now).Days;
                    DateTime graceDate = endDate.AddDays(swapDealSetting.GracePeriod);
                    daysToGrace = (graceDate - DateTime.Today).Days;
                }

                foreach (var inbound in swapDealSetting.Inbounds)
                {
                    SwapDealInfo dealInfo = new SwapDealInfo
                    {
                        CarrierAccountId = swapDealSetting.CarrierAccountId,
                        DealId = dealDefinition.DealId,
                        DealBED = swapDealSetting.BeginDate,
                        DealEED = swapDealSetting.EndDate,
                        GroupVolumeInMinutes = inbound.Volume,
                        DaysTostart = daysToStart,
                        RemainingDaysToEnd = daysToEnd,
                        RemainingDaysToGrace = daysToGrace,
                        GroupNumber = inbound.ZoneGroupNumber,
                        GroupType = 1,
                    };
                    saleProgressByGroupNb.Add(inbound.ZoneGroupNumber, dealInfo);
                }
                foreach (var outbound in swapDealSetting.Outbounds)
                {
                    SwapDealInfo dealInfo = new SwapDealInfo
                    {
                        CarrierAccountId = swapDealSetting.CarrierAccountId,
                        DealId = dealDefinition.DealId,
                        DealBED = swapDealSetting.BeginDate,
                        DealEED = swapDealSetting.EndDate,
                        GroupVolumeInMinutes = outbound.Volume,
                        DaysTostart = daysToStart,
                        RemainingDaysToEnd = daysToEnd,
                        RemainingDaysToGrace = daysToGrace,
                        GroupNumber = outbound.ZoneGroupNumber,
                        GroupType = 2
                    };
                    costProgressByGroupNb.Add(outbound.ZoneGroupNumber, dealInfo);
                }
            }

            List<int> dealIds = deals.Select(deal => deal.DealId).ToList();
            IEnumerable<DealDetailedProgress> dealsDetailedProgresses = dealDetailedProgressManager.GetDealsDetailedProgress(dealIds, minDealBED, context.ToTime);

            foreach (var dealProgress in dealsDetailedProgresses)
            {
                AppendSwapDealInfos(dealProgress.IsSale ? saleProgressByDealId : costProgressByDealId, dealProgress);
            }
            List<SwapDealProgressDataRecordType> dataRecords = new List<SwapDealProgressDataRecordType>();
            dataRecords.AddRange(CreateRecordType(saleProgressByDealId));
            dataRecords.AddRange(CreateRecordType(costProgressByDealId));

            foreach (var dataRecord in dataRecords)
            {
                context.OnRecordLoaded(DataRecordObjectMapper(dataRecord), DateTime.Now);
            }
        }

        private List<SwapDealProgressDataRecordType> CreateRecordType(SwapDealnfoByDealId progressByDealId)
        {
            var dataRecords = new List<SwapDealProgressDataRecordType>();
            foreach (var kvp in progressByDealId.Values)
            {
                foreach (var dealInfo in kvp.Values)
                {
                    int dealLength = dealInfo.DealEED.HasValue
                        ? (dealInfo.DealEED.Value - dealInfo.DealBED).Days
                        : 1;

                    int dealCurrentDay = DateTime.Now >= dealInfo.DealBED
                        ? (DateTime.Now - dealInfo.DealBED).Days
                        : 0;

                    var expectedVolumeDailyInMin = dealInfo.GroupVolumeInMinutes / dealLength;
                    var overallExpectedVolumeInMin = expectedVolumeDailyInMin * dealCurrentDay;
                    var weeklyExpectedVolumeInMin = expectedVolumeDailyInMin * 7;

                    var recordType = new SwapDealProgressDataRecordType
                    {
                        CarrierAccountId = dealInfo.CarrierAccountId,
                        Deal = dealInfo.DealId,
                        RemainingDaysToGrace = dealInfo.RemainingDaysToGrace,
                        RemainingDaysToEnd = dealInfo.RemainingDaysToEnd,
                        DaysTostart = dealInfo.DaysTostart,
                        GroupNumber = dealInfo.GroupNumber,
                        GroupType = dealInfo.GroupType,
                        TodayProgressPerc =
                            expectedVolumeDailyInMin > 0 ? (dealInfo.TodayProgressInMinutes / expectedVolumeDailyInMin) * 100 : 0,
                        OverallProgressPerc =
                            overallExpectedVolumeInMin > 0 ? (dealInfo.OverallProgressInMinutes / overallExpectedVolumeInMin) * 100 : 0,
                        WeeklyProgressPerc =
                            weeklyExpectedVolumeInMin > 0 ? (dealInfo.WeeklyProgressMinutes / weeklyExpectedVolumeInMin) * 100 : 0,
                        
                    };
                    dataRecords.Add(recordType);
                }
            }
            return dataRecords;
        }
        private DataRecordObject DataRecordObjectMapper(SwapDealProgressDataRecordType swapDealProgress)
        {
            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"Deal", swapDealProgress.Deal},
                {"CarrierAccount", swapDealProgress.CarrierAccountId},
                {"Group", swapDealProgress.GroupNumber},
                {"GroupType", swapDealProgress.GroupType},
                {"TodayProgressPerc", swapDealProgress.TodayProgressPerc},
                {"YesterdayProgressPerc", swapDealProgress.YesterdayProgressPerc},
                {"WeeklyProgressPerc", swapDealProgress.WeeklyProgressPerc},
                {"OverallProgressPerc", swapDealProgress.OverallProgressPerc},
                {"DaysToStart", swapDealProgress.DaysTostart},
                {"RemainingDaysToEnd", swapDealProgress.RemainingDaysToEnd},
                {"RemainingDaysToGrace", swapDealProgress.RemainingDaysToGrace}
            };
            return new DataRecordObject(new Guid("AB46069D-0FFC-4027-9CCF-BD1AF8EC91F7"), swapDealProgressObject);
        }
        private void AppendSwapDealInfos(SwapDealnfoByDealId dealById, DealDetailedProgress dealProgress)
        {
            SwapDealInfoByGroupNb zoneGroup = dealById.GetRecord(dealProgress.DealId);
            SwapDealInfo swapDealProgress = zoneGroup.GetRecord(dealProgress.ZoneGroupNb);

            DateTime dealBED = dealProgress.FromTime;

            swapDealProgress.OverallProgressInMinutes += dealProgress.ReachedDurationInSeconds / 60;
            if (dealBED == DateTime.Today)
                swapDealProgress.TodayProgressInMinutes += dealProgress.ReachedDurationInSeconds / 60;
            if (dealBED == DateTime.Today.AddDays(-1))
                swapDealProgress.YesterdayProgressInMinutes += dealProgress.ReachedDurationInSeconds / 60;

            DateTime toWeekDate = DateTime.Now;
            DateTime fromWeekDate = DateTime.Now.Date.AddDays(-7);

            if (dealProgress.FromTime < toWeekDate && dealProgress.FromTime > fromWeekDate)
                swapDealProgress.WeeklyProgressMinutes += dealProgress.ReachedDurationInSeconds / 60;

        }
    }

    #region classes

    public class SwapDealnfoByDealId : Dictionary<Int32, SwapDealInfoByGroupNb>
    {

    }
    public class SwapDealInfoByGroupNb : Dictionary<Int32, SwapDealInfo>
    {

    }
    public class SwapDealInfo
    {
        public long DealId { get; set; }
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public int CarrierAccountId { get; set; }
        public int GroupNumber { get; set; }
        public int GroupVolumeInMinutes { get; set; }
        public decimal OverallProgressInMinutes { get; set; }
        public decimal TodayProgressInMinutes { get; set; }
        public decimal YesterdayProgressInMinutes { get; set; }
        public decimal WeeklyProgressMinutes { get; set; }
        public int DaysTostart { get; set; }
        public int RemainingDaysToEnd { get; set; }
        public int RemainingDaysToGrace { get; set; }
        public int GroupType { get; set; }
    }

    public class SwapDealProgressDataRecordType
    {
        public long Deal { get; set; }
        public int CarrierAccountId { get; set; }
        public int GroupNumber { get; set; }
        public int GroupType { get; set; }
        public decimal TodayProgressPerc { get; set; }
        public decimal YesterdayProgressPerc { get; set; }
        public decimal WeeklyProgressPerc { get; set; }
        public decimal OverallProgressPerc { get; set; }
        public int DaysTostart { get; set; }
        public int RemainingDaysToEnd { get; set; }
        public int RemainingDaysToGrace { get; set; }
    }

    #endregion
}
