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

        public DealDetailedProgressManager()
        {
            _dataManager = DealDataManagerFactory.GetDataManager<IDealDetailedProgressDataManager>();

        }

        #endregion

        #region Public Methods

        public void InsertDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            if (dealDetailedProgresses != null && dealDetailedProgresses.Count > 0)
                _dataManager.InsertDealDetailedProgresses(dealDetailedProgresses);
        }

        public void UpdateDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            if (dealDetailedProgresses != null && dealDetailedProgresses.Count > 0)
                _dataManager.UpdateDealDetailedProgresses(dealDetailedProgresses);
        }

        public void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds)
        {
            if (dealDetailedProgressIds != null && dealDetailedProgressIds.Count > 0)
                _dataManager.DeleteDealDetailedProgresses(dealDetailedProgressIds);
        }

        public void UpdateDealDetailedProgressTable(bool isSale, DateTime beginDate, Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords)
        {
            List<DealDetailedProgress> itemsToAdd;
            List<DealDetailedProgress> itemsToUpdate;
            HashSet<long> itemsToKeep;

            List<DealDetailedProgress> dealDetailedProgresses = _dataManager.GetDealDetailedProgresses(isSale, beginDate);
            Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealDetailedProgress>> dealDetailedProgressRecords = ConvertDealDetailedProgressToDict(dealDetailedProgresses);

            GetDealDetailedProgressDataToModify(currentDealBillingSummaryRecords, dealDetailedProgressRecords, out itemsToAdd, out itemsToUpdate, out itemsToKeep);
            List<long> itemsToDelete = dealDetailedProgresses.FindAllRecords(itm => !itemsToKeep.Contains(itm.DealDetailedProgressID)).Select(itm => itm.DealDetailedProgressID).ToList();

            InsertDealDetailedProgresses(itemsToAdd);
            UpdateDealDetailedProgresses(itemsToUpdate);
            DeleteDealDetailedProgresses(itemsToDelete);
        }

        public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp)
        {
            return _dataManager.GetDealEvaluatorBeginDate(lastTimestamp);
        }

        public List<DealZoneGroupData> GetDealZoneGroupDataBeforeDate(bool isSale, DateTime beforeDate, List<DealZoneGroup> dealZoneGroups)
        {
            if (dealZoneGroups == null || dealZoneGroups.Count > 0)
                return null;
            return _dataManager.GetDealZoneGroupDataBeforeDate(isSale, beforeDate, dealZoneGroups);
        }

        public List<DealZoneGroupTierData> GetDealZoneGroupTierDataBeforeDate(bool isSale, DateTime beforeDate, List<DealZoneGroupTier> dealZoneGroupTiers)
        {
            if (dealZoneGroupTiers == null || dealZoneGroupTiers.Count > 0)
                return null;
            return _dataManager.GetDealZoneGroupTierDataBeforeDate(isSale, beforeDate, dealZoneGroupTiers);
        }

        #endregion

        #region Private Methods

        private Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealDetailedProgress>> ConvertDealDetailedProgressToDict(List<DealDetailedProgress> dealDetailedProgresses)
        {
            var dealDetailedProgressRecords = new Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealDetailedProgress>>();
            Dictionary<DateTime, DealDetailedProgress> dealDetailedProgressItem;

            foreach (var dealDetailedProgress in dealDetailedProgresses)
            {
                DealZoneGroupTierRate dealZoneGroupTierRate = new DealZoneGroupTierRate()
                {
                    DealId = dealDetailedProgress.DealID,
                    ZoneGroupNb = dealDetailedProgress.ZoneGroupNb,
                    TierNb = dealDetailedProgress.TierNb,
                    RateTierNb = dealDetailedProgress.RateTierNb
                };

                if (!dealDetailedProgressRecords.TryGetValue(dealZoneGroupTierRate, out dealDetailedProgressItem))
                {
                    dealDetailedProgressItem = new Dictionary<DateTime, DealDetailedProgress>();
                    dealDetailedProgressItem.Add(dealDetailedProgress.FromTime, dealDetailedProgress);
                    dealDetailedProgressRecords.Add(dealZoneGroupTierRate, dealDetailedProgressItem);
                }
                else
                {
                    if (!dealDetailedProgressItem.ContainsKey(dealDetailedProgress.FromTime))
                        dealDetailedProgressItem.Add(dealDetailedProgress.FromTime, dealDetailedProgress);
                    else
                        throw new VRBusinessException(string.Format("FromTime '{0}' for DealId '{1}', ZoneGroupNb '{2}', TierNb '{3}', RateTierNb '{4}' exists more than once in Deal Detailed Progress.",
                            dealDetailedProgress.FromTime, dealDetailedProgress.DealID, dealDetailedProgress.ZoneGroupNb, dealDetailedProgress.TierNb, dealDetailedProgress.RateTierNb));
                }
            }

            return dealDetailedProgressRecords;
        }

        private void GetDealDetailedProgressDataToModify(Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords,
            Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealDetailedProgress>> dealDetailedProgressRecords, out List<DealDetailedProgress> itemsToAdd,
            out List<DealDetailedProgress> itemsToUpdate, out HashSet<long> itemsToKeep)
        {
            itemsToAdd = new List<DealDetailedProgress>();
            itemsToUpdate = new List<DealDetailedProgress>();
            itemsToKeep = new HashSet<long>();

            Dictionary<DateTime, DealDetailedProgress> dealDetailedProgressItem;
            DealDetailedProgress dealDetailedProgress;

            foreach (var kvp_dealBillingSummaryRecord in currentDealBillingSummaryRecords)
            {
                DealZoneGroupTierRate dealZoneGroupTierRate = kvp_dealBillingSummaryRecord.Key;
                Dictionary<DateTime, DealBillingSummary> dealBillingSummaryByBatchStart = kvp_dealBillingSummaryRecord.Value;

                if (!dealDetailedProgressRecords.TryGetValue(dealZoneGroupTierRate, out dealDetailedProgressItem))
                {
                    itemsToAdd.AddRange(dealBillingSummaryByBatchStart.Values.Select(itm => new DealDetailedProgress()
                    {
                        DealID = itm.DealId,
                        ZoneGroupNb = itm.DealZoneGroupNb,
                        IsSale = itm.IsSale,
                        TierNb = itm.DealTierNb,
                        RateTierNb = itm.DealRateTierNb,
                        FromTime = itm.BatchStart,
                        ToTime = itm.BatchStart.AddMinutes(30)
                    }));
                }
                else
                {
                    foreach (var dealBillingSummaryRecordByBatchStart in dealBillingSummaryByBatchStart)
                    {
                        if (!dealDetailedProgressItem.TryGetValue(dealBillingSummaryRecordByBatchStart.Key, out dealDetailedProgress))
                        {
                            itemsToAdd.Add(new DealDetailedProgress()
                            {
                                DealID = dealBillingSummaryRecordByBatchStart.Value.DealId,
                                ZoneGroupNb = dealBillingSummaryRecordByBatchStart.Value.DealZoneGroupNb,
                                IsSale = dealBillingSummaryRecordByBatchStart.Value.IsSale,
                                TierNb = dealBillingSummaryRecordByBatchStart.Value.DealTierNb,
                                RateTierNb = dealBillingSummaryRecordByBatchStart.Value.DealRateTierNb,
                                FromTime = dealBillingSummaryRecordByBatchStart.Value.BatchStart,
                                ToTime = dealBillingSummaryRecordByBatchStart.Value.BatchStart.AddMinutes(30),
                            });
                        }
                        else
                        {
                            itemsToKeep.Add(dealDetailedProgress.DealDetailedProgressID);

                            if (!AreEqual(dealBillingSummaryRecordByBatchStart.Value, dealDetailedProgress))
                            {
                                itemsToUpdate.Add(new DealDetailedProgress()
                                {
                                    DealDetailedProgressID = dealDetailedProgress.DealDetailedProgressID,
                                    DealID = dealBillingSummaryRecordByBatchStart.Value.DealId,
                                    ZoneGroupNb = dealBillingSummaryRecordByBatchStart.Value.DealZoneGroupNb,
                                    IsSale = dealBillingSummaryRecordByBatchStart.Value.IsSale,
                                    TierNb = dealBillingSummaryRecordByBatchStart.Value.DealTierNb,
                                    RateTierNb = dealBillingSummaryRecordByBatchStart.Value.DealRateTierNb,
                                    FromTime = dealBillingSummaryRecordByBatchStart.Value.BatchStart,
                                    ToTime = dealBillingSummaryRecordByBatchStart.Value.BatchStart.AddMinutes(30),
                                });
                            }
                        }
                    }
                }
            }
        }

        private bool AreEqual(DealBillingSummary dealBillingSummary, DealDetailedProgress dealDetailedProgress)
        {
            if (dealBillingSummary == null || dealDetailedProgress == null)
                return false;

            if (dealDetailedProgress.FromTime != dealBillingSummary.BatchStart)
                return false;

            if (dealDetailedProgress.ToTime != dealBillingSummary.BatchStart.AddMinutes(30))
                return false;

            if (dealDetailedProgress.DealID != dealBillingSummary.DealId)
                return false;

            if (dealDetailedProgress.ZoneGroupNb != dealBillingSummary.DealZoneGroupNb)
                return false;

            if (dealDetailedProgress.ReachedDurationInSeconds != dealBillingSummary.DurationInSeconds)
                return false;

            if (dealDetailedProgress.TierNb != dealBillingSummary.DealTierNb)
                return false;

            if (dealDetailedProgress.RateTierNb != dealBillingSummary.DealRateTierNb)
                return false;

            return true;
        }

        #endregion
    }
}