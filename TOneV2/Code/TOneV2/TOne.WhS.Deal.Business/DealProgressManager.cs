using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public class DealProgressManager
    {
        #region Public Methods

        public Dictionary<DealZoneGroup, DealProgress> GetDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            List<DealProgress> dealProgressList = dealProgressDataManager.GetDealProgresses(dealZoneGroups, isSale);
            if (dealProgressList == null || dealProgressList.Count == 0)
                return null;

            return dealProgressList.ToDictionary(itm => new DealZoneGroup() { DealId = itm.DealID, ZoneGroupNb = itm.ZoneGroupNb }, itm => itm);
        }

        public void InsertDealProgresses(IEnumerable<DealProgress> dealProgresses)
        {
            if (dealProgresses == null || dealProgresses.Count() == 0)
                return;

            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            dealProgressDataManager.InsertDealProgresses(dealProgresses.ToList());
        }

        public void UpdateDealProgresses(IEnumerable<DealProgress> dealProgresses)
        {
            if (dealProgresses == null || dealProgresses.Count() == 0)
                return;

            IDealProgressDataManager dealProgressDataManager = DealDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            dealProgressDataManager.UpdateDealProgresses(dealProgresses.ToList());
        }

        public void UpdateDealProgressTable(Boolean isSale, DateTime beginDate, Dictionary<DealZoneGroup, DealProgress> dealProgressByZoneGroup,
            Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            List<DealProgress> itemsToAdd = new List<DealProgress>();
            List<DealProgress> itemsToUpdate = new List<DealProgress>();

            List<DealProgress> expectedDealProgresses = BuildExpectedDealProgresses(isSale, beginDate, currentDealBillingSummaryRecords);

            DealProgress currentDealProgress;
            foreach (var expectedDealProgress in expectedDealProgresses)
            {
                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = expectedDealProgress.DealID, ZoneGroupNb = expectedDealProgress.ZoneGroupNb };
                if (!dealProgressByZoneGroup.TryGetValue(dealZoneGroup, out currentDealProgress))
                {
                    itemsToAdd.Add(expectedDealProgress);
                }
                else if (!expectedDealProgress.IsEqual(currentDealProgress))
                {
                    expectedDealProgress.DealProgressID = currentDealProgress.DealProgressID;
                    itemsToUpdate.Add(expectedDealProgress);
                }
            }

            InsertDealProgresses(itemsToAdd);
            UpdateDealProgresses(itemsToUpdate);
        }

        #endregion

        #region Private Methods

        private List<DealProgress> BuildExpectedDealProgresses(Boolean isSale, DateTime beginDate, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            Dictionary<DealZoneGroupTier, decimal> currentDealZoneGroupTierDict = BuildDealZoneGroupTierDict(currentDealBillingSummaryRecords);

            List<DealZoneGroupTier> existingDealZoneGroupTiers = currentDealZoneGroupTierDict.Select(itm => new DealZoneGroupTier() { DealId = itm.Key.DealId, ZoneGroupNb = itm.Key.ZoneGroupNb, TierNb = itm.Key.TierNb }).ToList();
            List<DealZoneGroupTierData> previousDealZoneGroupTierData = new DealDetailedProgressManager().GetDealZoneGroupTierDataBeforeDate(isSale, beginDate, existingDealZoneGroupTiers);
            Dictionary<DealZoneGroupTier, DealZoneGroupTierData> previousDealZoneGroupTierDataDict = 
                previousDealZoneGroupTierData.ToDictionary(itm => new DealZoneGroupTier() { DealId = itm.DealID, ZoneGroupNb = itm.ZoneGroupNb, TierNb = itm.TierNb }, itm => itm);

            List<DealProgress> expectedDealProgresses = new List<DealProgress>();
            DealZoneGroupTierData tempDealZoneGroupTierData;

            foreach (var dealZoneGroupTierItem in currentDealZoneGroupTierDict)
            {
                decimal reachedDurationInSeconds = dealZoneGroupTierItem.Value;

                if (previousDealZoneGroupTierDataDict.TryGetValue(dealZoneGroupTierItem.Key, out tempDealZoneGroupTierData))
                    reachedDurationInSeconds += tempDealZoneGroupTierData.TotalReachedDurationInSeconds;

                DealZoneGroupTierDetails dealZoneGroupTierDetails = GetDealZoneGroupTierDetails(isSale, dealZoneGroupTierItem.Key.DealId, dealZoneGroupTierItem.Key.ZoneGroupNb, dealZoneGroupTierItem.Key.TierNb.Value);
                if (dealZoneGroupTierDetails == null)
                    throw new VRBusinessException(string.Format("TierNb '{0}' for Deal '{1}', ZoneGroupNb '{2}' and IsSale '{3}' doesn't exist", dealZoneGroupTierItem.Key.TierNb, dealZoneGroupTierItem.Key.DealId, dealZoneGroupTierItem.Key.ZoneGroupNb, isSale));

                decimal? targetDurationInSeconds = dealZoneGroupTierDetails.VolumeInSeconds;

                expectedDealProgresses.Add(new DealProgress()
                {
                    IsSale = isSale,
                    DealID = dealZoneGroupTierItem.Key.DealId,
                    ZoneGroupNb = dealZoneGroupTierItem.Key.ZoneGroupNb,
                    CurrentTierNb = dealZoneGroupTierItem.Key.TierNb.Value,
                    ReachedDurationInSeconds = reachedDurationInSeconds,
                    TargetDurationInSeconds = targetDurationInSeconds
                });
            }

            return expectedDealProgresses;
        }

        private Dictionary<DealZoneGroupTier, decimal> BuildDealZoneGroupTierDict(Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            Dictionary<DealZoneGroup, TierDealBillingSummary> tierDealBillingSummaryDict = new Dictionary<DealZoneGroup, TierDealBillingSummary>();
            TierDealBillingSummary tempTierDealBillingSummary;
            foreach (var item in currentDealBillingSummaryRecords)
            {
                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = item.Key.DealId, ZoneGroupNb = item.Key.ZoneGroupNb };
                tempTierDealBillingSummary = tierDealBillingSummaryDict.GetOrCreateItem(dealZoneGroup, () =>
                {
                    return new TierDealBillingSummary()
                    {
                        TierNb = item.Key.TierNb,
                        TotalReachedDurationInSeconds = 0
                    };
                });

                if (tempTierDealBillingSummary.TierNb > item.Key.TierNb)
                    continue;

                if (tempTierDealBillingSummary.TierNb == item.Key.TierNb)
                {
                    tempTierDealBillingSummary.TotalReachedDurationInSeconds += item.Value.Values.Sum(itm => itm.DurationInSeconds);
                }
                else
                {
                    tempTierDealBillingSummary.TierNb = item.Key.TierNb;
                    tempTierDealBillingSummary.TotalReachedDurationInSeconds = item.Value.Values.Sum(itm => itm.DurationInSeconds);
                }
            }

            return tierDealBillingSummaryDict.ToDictionary(itm => new DealZoneGroupTier() { DealId = itm.Key.DealId, TierNb = itm.Value.TierNb, ZoneGroupNb = itm.Key.ZoneGroupNb }, itm => itm.Value.TotalReachedDurationInSeconds);
        }

        private DealZoneGroupTierDetails GetDealZoneGroupTierDetails(Boolean isSale, int dealId, int zoneGroupNb, int currentTierNumber)
        {
            if (isSale)
                return new DealDefinitionManager().GetSaleDealZoneGroupTierDetails(dealId, zoneGroupNb, currentTierNumber);
            else
                return new DealDefinitionManager().GetSupplierDealZoneGroupTierDetails(dealId, zoneGroupNb, currentTierNumber);
        }

        #endregion

        #region Private Classes

        private class TierDealBillingSummary
        {
            public int TierNb { get; set; }
            public decimal TotalReachedDurationInSeconds { get; set; }
        }

        #endregion
    }
}