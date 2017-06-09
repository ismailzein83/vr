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

        public void UpdateDealProgressTable(Boolean isSale, DateTime beginDate, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            List<DealProgress> itemsToAdd = new List<DealProgress>();
            List<DealProgress> itemsToUpdate = new List<DealProgress>();

            List<DealProgress> expectedDealProgresses = BuildExpectedDealProgresses(isSale, beginDate, currentDealBillingSummaryRecords);
            Dictionary<DealZoneGroup, DealProgress> dealProgressByZoneGroup = GetDealProgressByZoneGroup(isSale, currentDealBillingSummaryRecords);

            DealProgress currentDealProgress;
            foreach (var expectedDealProgress in expectedDealProgresses)
            {
                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = expectedDealProgress.DealID, ZoneGroupNb = expectedDealProgress.ZoneGroupNb };
                if (!dealProgressByZoneGroup.TryGetValue(dealZoneGroup, out currentDealProgress))
                {
                    itemsToAdd.Add(expectedDealProgress);
                }
                else
                {
                    if (!expectedDealProgress.IsEqual(currentDealProgress))
                    {
                        expectedDealProgress.DealProgressID = currentDealProgress.DealProgressID;
                        itemsToUpdate.Add(expectedDealProgress);
                    }
                }
            }

            InsertDealProgresses(itemsToAdd);
            UpdateDealProgresses(itemsToUpdate);
        }

        #endregion

        #region Private Methods

        private List<DealProgress> BuildExpectedDealProgresses(Boolean isSale, DateTime beginDate, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            Dictionary<DealZoneGroup, TierDealBillingSummary> tierDealBillingSummaryDict = BuildTierDealBillingSummaryDict(currentDealBillingSummaryRecords);

            List<DealProgress> expectedDealProgresses = new List<DealProgress>();

            List<DealZoneGroupTierRate> dealZoneGroupTierRates = currentDealBillingSummaryRecords.Keys.ToList();




            List<DealZoneGroupTierData> dealZoneGroupTierData = new DealDetailedProgressManager().GetDealZoneGroupTierDataBeforeDate(isSale, beginDate, dealZoneGroupTierRates);
            Dictionary<DealZoneGroupTierRate, DealZoneGroupTierData> dealZoneGroupTierDataDict = dealZoneGroupTierData.ToDictionary(itm => new DealZoneGroupTierRate() { DealId = itm.DealID, ZoneGroupNb = itm.ZoneGroupNb, TierNb = itm.TierNb });

            //List<DealZoneGroupTierRate> dealZoneGroupTierRates = new List<DealZoneGroupTierRate>();


            foreach (var kvp_dealBillingSummaryRecord in currentDealBillingSummaryRecords)
            {
                DealZoneGroupTierRate currentDealZoneGroupTierRate = kvp_dealBillingSummaryRecord.Key;
                Dictionary<DateTime, DealBillingSummary> currentDealBillingSummaryByBatchStart = kvp_dealBillingSummaryRecord.Value;

                IOrderedEnumerable<DealBillingSummary> orderedDealBillingSummaries = currentDealBillingSummaryByBatchStart.Values.OrderByDescending(itm => itm.DealTierNb);
                int currentTierNumber = orderedDealBillingSummaries.First().DealTierNb;



                DealZoneGroupTier dealZoneGroupTier = GetDealZoneGroupTier(isSale, currentDealZoneGroupTierRate.DealId, currentDealZoneGroupTierRate.ZoneGroupNb, currentTierNumber);
                if (dealZoneGroupTier == null)
                    throw new VRBusinessException(string.Format("TierNb '{0}' for Deal '{1}', ZoneGroupNb '{2}' and IsSale '{3}' doesn't exist", currentTierNumber, currentDealZoneGroupTierRate.DealId, currentDealZoneGroupTierRate.ZoneGroupNb, isSale));

                decimal? targetDurationInSeconds = dealZoneGroupTier.VolumeInSeconds;

                DealZoneGroupTierData tempDealZoneGroupTierData;
                decimal reachedDurationInSeconds = dealZoneGroupTierDataDict.TryGetValue(currentDealZoneGroupTierRate, out tempDealZoneGroupTierData) ? tempDealZoneGroupTierData.TotalReachedDurationInSeconds : 0;

                foreach (var dealBillingSummary in orderedDealBillingSummaries)
                {
                    if (dealBillingSummary.DealTierNb != currentTierNumber)
                        break;

                    reachedDurationInSeconds += dealBillingSummary.DurationInSeconds;
                }

                expectedDealProgresses.Add(new DealProgress()
                {
                    IsSale = isSale,
                    DealID = currentDealZoneGroupTierRate.DealId,
                    ZoneGroupNb = currentDealZoneGroupTierRate.ZoneGroupNb,
                    CurrentTierNb = currentTierNumber,
                    ReachedDurationInSeconds = reachedDurationInSeconds,
                    TargetDurationInSeconds = targetDurationInSeconds
                });
            }


            return expectedDealProgresses;
        }

        private Dictionary<DealZoneGroup, TierDealBillingSummary> BuildTierDealBillingSummaryDict(Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
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

            return tierDealBillingSummaryDict;
        }

        private DealZoneGroupTier GetDealZoneGroupTier(Boolean isSale, int dealId, int zoneGroupNb, int currentTierNumber)
        {
            if (isSale)
                return new DealDefinitionManager().GetSaleDealZoneGroupTier(dealId, zoneGroupNb, currentTierNumber);
            else
                return new DealDefinitionManager().GetSupplierDealZoneGroupTier(dealId, zoneGroupNb, currentTierNumber);
        }

        private Dictionary<DealZoneGroup, DealProgress> GetDealProgressByZoneGroup(bool isSale, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            IEnumerable<DealZoneGroup> dealZoneGroups = currentDealBillingSummaryRecords.Keys.Select(itm => new DealZoneGroup() { DealId = itm.DealId, ZoneGroupNb = itm.ZoneGroupNb });
            HashSet<DealZoneGroup> dealZoneGroupsHashSet = new HashSet<DealZoneGroup>(dealZoneGroups);
            return GetDealProgresses(dealZoneGroupsHashSet, isSale);
        }

        #endregion

        private class TierDealBillingSummary
        {
            public int TierNb { get; set; }
            public decimal TotalReachedDurationInSeconds { get; set; }
        }
    }
    
}