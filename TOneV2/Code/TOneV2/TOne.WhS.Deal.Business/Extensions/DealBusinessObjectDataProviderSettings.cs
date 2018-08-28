using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        CurrencyManager currencyManager = new CurrencyManager();
        CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

        public override Guid ConfigId { get { return new Guid("7b5d42b4-b05c-4f54-9683-a73dd0cebd68"); } }

        public override bool DoesSupportFilterOnAllFields { get { return false; } }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            var saleProgressByDealId = new SwapDealnfoByDealId();
            var costProgressByDealId = new SwapDealnfoByDealId();
            var dealDetailedProgressManager = new DealDetailedProgressManager();
            var swapDealManager = new SwapDealManager();
            var minDealBED = DateTime.Now;

            IEnumerable<DealDefinition> deals = swapDealManager.GetSwapDealsEffectiveAfterDate(context.FromTime.Value);

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

                int daysToEnd = 0, daysToGrace = 0, dealLifeSpan = 1;
                if (swapDealSetting.EndDate.HasValue)
                {
                    DateTime endDate = swapDealSetting.EndDate.Value;
                    daysToEnd = swapDealSetting.EEDToStore > DateTime.Today ? (endDate - DateTime.Now).Days : 0;
                    DateTime graceDate = endDate.AddDays(swapDealSetting.GracePeriod);
                    daysToGrace = swapDealSetting.EEDToStore > DateTime.Today ? (graceDate - DateTime.Today).Days : 0;
                    dealLifeSpan = (endDate - swapDealSetting.BeginDate).Days;
                }

                int dealDay = 0;

                if (DateTime.Now < swapDealSetting.BeginDate)
                    dealDay = 0;
                else if (DateTime.Now > swapDealSetting.EndDate.Value)
                    dealDay = dealLifeSpan;
                else
                    dealDay = (DateTime.Now - swapDealSetting.BeginDate).Days;


                foreach (var inbound in swapDealSetting.Inbounds)
                {
                    SwapDealInfo dealInfo = new SwapDealInfo
                    {
                        CarrierAccountId = swapDealSetting.CarrierAccountId,
                        DealId = dealDefinition.DealId,
                        DealBED = swapDealSetting.BeginDate,
                        DealEED = swapDealSetting.EEDToStore,
                        GroupVolumeInMinutes = inbound.Volume,
                        DaysTostart = daysToStart,
                        RemainingDaysToEnd = daysToEnd,
                        RemainingDaysToGrace = daysToGrace,
                        GroupNumber = inbound.ZoneGroupNumber,
                        GroupName = inbound.Name,
                        GroupType = 1,
                        DealDay = dealDay,
                        DealLifeSpan = dealLifeSpan,
                        CurrencyId = swapDealSetting.CurrencyId,
                        Rate = inbound.Rate
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
                        DealEED = swapDealSetting.EEDToStore,
                        GroupVolumeInMinutes = outbound.Volume,
                        DaysTostart = daysToStart,
                        RemainingDaysToEnd = daysToEnd,
                        RemainingDaysToGrace = daysToGrace,
                        GroupNumber = outbound.ZoneGroupNumber,
                        GroupName = outbound.Name,
                        GroupType = 2,
                        DealDay = dealDay,
                        DealLifeSpan = dealLifeSpan,
                        CurrencyId = swapDealSetting.CurrencyId,
                        Rate = outbound.Rate
                    };
                    costProgressByGroupNb.Add(outbound.ZoneGroupNumber, dealInfo);
                }
            }

            List<int> dealIds = deals.Select(deal => deal.DealId).ToList();
            IEnumerable<DealDetailedProgress> dealsDetailedProgresses = dealDetailedProgressManager.GetDealsDetailedProgress(dealIds, minDealBED, context.ToTime.Value);

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
                    decimal expectedVolumeDailyInMin = (decimal)dealInfo.GroupVolumeInMinutes / dealInfo.DealLifeSpan;
                    decimal overallExpectedVolumeInMin = expectedVolumeDailyInMin * dealInfo.DealDay;
                    decimal weeklyExpectedVolumeInMin = expectedVolumeDailyInMin * 7;

                    var recordType = new SwapDealProgressDataRecordType
                    {
                        CarrierAccountId = dealInfo.CarrierAccountId,
                        Deal = dealInfo.DealId,
                        DealBED = dealInfo.DealBED,
                        DealEED = dealInfo.DealEED,
                        RemainingDaysToGrace = dealInfo.RemainingDaysToGrace,
                        RemainingDaysToEnd = dealInfo.RemainingDaysToEnd,
                        DaysTostart = dealInfo.DaysTostart,
                        GroupNumber = dealInfo.GroupNumber,
                        GroupName = dealInfo.GroupName,
                        GroupType = dealInfo.GroupType,
                        TodayProgressPerc = expectedVolumeDailyInMin > 0 ? (dealInfo.TodayProgressInMinutes / expectedVolumeDailyInMin) * 100 : 0,
                        OverallProgressPerc = overallExpectedVolumeInMin > 0 ? (dealInfo.OverallProgressInMinutes / overallExpectedVolumeInMin) * 100 : 0,
                        WeeklyProgressPerc = weeklyExpectedVolumeInMin > 0 ? (dealInfo.WeeklyProgressMinutes / weeklyExpectedVolumeInMin) * 100 : 0,
                        ReachedDuration = dealInfo.OverallProgressInMinutes,
                        ReachedAmount = dealInfo.OverallProgressInMinutes * dealInfo.Rate,
                        Currency = currencyManager.GetCurrencyName(dealInfo.CurrencyId),


                    };
                    dataRecords.Add(recordType);
                }
            }
            return dataRecords;
        }

        private DataRecordObject DataRecordObjectMapper(SwapDealProgressDataRecordType swapDealProgress)
        {
             var carrierProfileId = carrierAccountManager.GetCarrierProfileId(swapDealProgress.CarrierAccountId);
            var carrierName = carrierAccountManager.GetCarrierAccountName(swapDealProgress.CarrierAccountId);

            var swapDealProgressObject = new Dictionary<string, object>
            {
                {"Deal", swapDealProgress.Deal},
                {"DealBED", swapDealProgress.DealBED},
                {"DealEED", swapDealProgress.DealEED},
                {"CarrierAccount", swapDealProgress.CarrierAccountId },
                {"ProfileId", carrierProfileId },
                {"CarrierName", carrierName},
                {"Group", swapDealProgress.GroupNumber},
                {"GroupName", swapDealProgress.GroupName },
                {"GroupType", swapDealProgress.GroupType},
                {"TodayProgressPerc", swapDealProgress.TodayProgressPerc},
                {"YesterdayProgressPerc", swapDealProgress.YesterdayProgressPerc},
                {"WeeklyProgressPerc", swapDealProgress.WeeklyProgressPerc},
                {"OverallProgressPerc", swapDealProgress.OverallProgressPerc},
                {"DaysToStart", swapDealProgress.DaysTostart},
                {"RemainingDaysToEnd", swapDealProgress.RemainingDaysToEnd},
                {"RemainingDaysToGrace", swapDealProgress.RemainingDaysToGrace},
                {"ReachedDuration",swapDealProgress.ReachedDuration },
                {"ReachedAmount",swapDealProgress.ReachedAmount },
                {"Currency",swapDealProgress.Currency},
            };
            return new DataRecordObject(new Guid("AB46069D-0FFC-4027-9CCF-BD1AF8EC91F7"), swapDealProgressObject);
        }

        private void AppendSwapDealInfos(SwapDealnfoByDealId dealById, DealDetailedProgress dealProgress)
        {
            SwapDealInfoByGroupNb zoneGroup = dealById.GetRecord(dealProgress.DealId);
            SwapDealInfo swapDealProgress = zoneGroup.GetRecord(dealProgress.ZoneGroupNb);

            DateTime dealBED = dealProgress.FromTime;
            if (swapDealProgress != null)
            {
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
    }

    #region Classes

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
        public string GroupName { get; set; }
        public int GroupVolumeInMinutes { get; set; }
        public decimal OverallProgressInMinutes { get; set; }
        public decimal TodayProgressInMinutes { get; set; }
        public decimal YesterdayProgressInMinutes { get; set; }
        public decimal WeeklyProgressMinutes { get; set; }
        public int DaysTostart { get; set; }
        public int RemainingDaysToEnd { get; set; }
        public int RemainingDaysToGrace { get; set; }
        public int GroupType { get; set; }
        public int DealLifeSpan { get; set; }
        public int DealDay { get; set; }
        public int CurrencyId { get; set; }
        public decimal Rate { get; set; }
    }

    public class SwapDealProgressDataRecordType
    {
        public long Deal { get; set; }
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public int CarrierAccountId { get; set; }
        public string GroupName { get; set; }
        public int GroupNumber { get; set; }
        public int GroupType { get; set; }
        public decimal TodayProgressPerc { get; set; }
        public decimal YesterdayProgressPerc { get; set; }
        public decimal WeeklyProgressPerc { get; set; }
        public decimal OverallProgressPerc { get; set; }
        public int DaysTostart { get; set; }
        public int RemainingDaysToEnd { get; set; }
        public int RemainingDaysToGrace { get; set; }
        public decimal ReachedDuration { get; set; }
        public decimal ReachedAmount { get; set; }
        public int SumOfAttempts { get; set; }
        public int SumOfSuccessfulAttempts { get; set; }
        public string Currency { get; set; }

    }

    #endregion
}
