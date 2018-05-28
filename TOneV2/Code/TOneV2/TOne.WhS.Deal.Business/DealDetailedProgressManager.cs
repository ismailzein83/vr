using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Caching;

namespace TOne.WhS.Deal.Business
{
    public class DealDetailedProgressManager
    {
        #region Properties/Ctor

        IDealDetailedProgressDataManager _dataManager;
        int _dealTechnicalSettingIntervalOffset;

        public DealDetailedProgressManager()
        {
            _dataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();
            _dealTechnicalSettingIntervalOffset = new ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();
        }

        #endregion

        #region Public Methods

        public IEnumerable<DealDetailedProgress> GetDealsDetailedProgress(List<int> dealIds, DateTime fromDate, DateTime ToDate)
        {
            var dealDetailedProgressDataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();
            return dealDetailedProgressDataManager.GetDealsDetailedProgress(dealIds, fromDate, ToDate);
        }
        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> GetDealDetailedProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale, DateTime? beginDate = null, DateTime? endDate = null)
        {
            if (dealZoneGroups == null)
                return null;

            IDealDetailedProgressDataManager dealDetailedProgressDataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();
            List<DealDetailedProgress> dealDetailedProgressList = dealDetailedProgressDataManager.GetDealDetailedProgresses(dealZoneGroups, isSale, beginDate, endDate);
            if (dealDetailedProgressList == null || dealDetailedProgressList.Count == 0)
                return null;

            return dealDetailedProgressList.ToDictionary(itm => new DealDetailedZoneGroupTier()
            {
                DealId = itm.DealId,
                ZoneGroupNb = itm.ZoneGroupNb,
                TierNb = itm.TierNb,
                RateTierNb = itm.RateTierNb,
                FromTime = itm.FromTime,
                ToTime = itm.ToTime
            }, itm => itm);
        }

        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> GetDealDetailedProgressesByDate(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            IDealDetailedProgressDataManager dealDetailedProgressDataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();
            List<DealDetailedProgress> dealDetailedProgressList = dealDetailedProgressDataManager.GetDealDetailedProgressesByDate(isSale, beginDate, endDate);
            if (dealDetailedProgressList == null || dealDetailedProgressList.Count == 0)
                return null;

            return dealDetailedProgressList.ToDictionary(itm => new DealDetailedZoneGroupTier()
            {
                DealId = itm.DealId,
                ZoneGroupNb = itm.ZoneGroupNb,
                TierNb = itm.TierNb,
                RateTierNb = itm.RateTierNb,
                FromTime = itm.FromTime,
                ToTime = itm.ToTime
            }, itm => itm);
        }


        public void InsertDealDetailedProgresses(IEnumerable<DealDetailedProgress> dealDetailedProgresses)
        {
            if (dealDetailedProgresses != null && dealDetailedProgresses.Count() > 0)
                _dataManager.InsertDealDetailedProgresses(dealDetailedProgresses.ToList());
        }

        public void UpdateDealDetailedProgresses(IEnumerable<DealDetailedProgress> dealDetailedProgresses)
        {
            if (dealDetailedProgresses != null && dealDetailedProgresses.Count() > 0)
                _dataManager.UpdateDealDetailedProgresses(dealDetailedProgresses.ToList());
        }

        public void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds)
        {
            if (dealDetailedProgressIds != null && dealDetailedProgressIds.Count > 0)
                _dataManager.DeleteDealDetailedProgresses(dealDetailedProgressIds);
        }

        public void DeleteDealDetailedProgresses(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            _dataManager.DeleteDealDetailedProgresses(isSale, beginDate, endDate);
        }

        public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp)
        {
            return _dataManager.GetDealEvaluatorBeginDate(lastTimestamp);
        }

        public bool AreEqual(DealDetailedProgress dealDetailedProgress, DealBillingSummary dealBillingSummary)
        {
            if (dealDetailedProgress == null || dealBillingSummary == null)
                return false;

            if (dealDetailedProgress.FromTime != dealBillingSummary.BatchStart)
                return false;

            if (dealDetailedProgress.ToTime != dealBillingSummary.BatchStart.AddMinutes(_dealTechnicalSettingIntervalOffset))
                return false;

            if (dealDetailedProgress.DealId != dealBillingSummary.DealId)
                return false;

            if (dealDetailedProgress.ZoneGroupNb != dealBillingSummary.ZoneGroupNb)
                return false;

            if (dealDetailedProgress.ReachedDurationInSeconds != dealBillingSummary.DurationInSeconds)
                return false;

            if (dealDetailedProgress.TierNb != dealBillingSummary.TierNb)
                return false;

            if (dealDetailedProgress.RateTierNb != dealBillingSummary.RateTierNb)
                return false;

            return true;
        }

        public Byte[] GetMaxTimestamp()
        {
            return _dataManager.GetMaxTimestamp();
        }

        #endregion
    }
}