﻿using System;
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

        public DealDetailedProgressManager()
        {
            _dataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();

        }

        #endregion

        #region Public Methods

        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> GetDealDetailedProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            IDealDetailedProgressDataManager dealDetailedProgressDataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();
            List<DealDetailedProgress> dealDetailedProgressList = dealDetailedProgressDataManager.GetDealDetailedProgresses(dealZoneGroups, isSale);
            if (dealDetailedProgressList == null || dealDetailedProgressList.Count == 0)
                return null;

            return dealDetailedProgressList.ToDictionary(itm => new DealDetailedZoneGroupTier()
            {
                DealID = itm.DealID,
                ZoneGroupNb = itm.ZoneGroupNb,
                TierNb = itm.TierNb,
                RateTierNb = itm.RateTierNb,
                FromTime = itm.FromTime,
                ToTime = itm.ToTime
            }
            , itm => itm);
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

        public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp)
        {
            return _dataManager.GetDealEvaluatorBeginDate(lastTimestamp);
        }

        public List<DealZoneGroupTierData> GetDealZoneGroupTierDataBeforeDate(bool isSale, DateTime beforeDate, List<DealZoneGroupTier> dealZoneGroupTiers)
        {
            if (dealZoneGroupTiers == null || dealZoneGroupTiers.Count > 0)
                return null;
            return _dataManager.GetDealZoneGroupTierDataBeforeDate(isSale, beforeDate, dealZoneGroupTiers);
        }

        #endregion
    }
}