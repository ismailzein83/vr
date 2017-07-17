using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Retail.Cost.Data;
using Retail.Cost.Entities;

namespace Retail.Cost.Business
{
    public class CDRCostManager
    {
        #region Public Methods

        /// <summary>
        /// used in data transformation
        /// </summary>
        public void AssignCostToCDR(List<dynamic> cdrs, string attemptDateTimeFieldName, string cgpnFieldName, string cdpnFieldName, string durationInSecondsFieldName, CDRCostFieldNames cdrCostFieldNames)
        {
            if (cdrCostFieldNames == null)
                return;

            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = new ConfigManager().GetCDRCostTechnicalSettingData();
            if (!cdrCostTechnicalSettingData.IsCostIncluded)
                return;

            RecordFilterManager recordFilterManager = new RecordFilterManager();
            RecordFilterGroup recordFilterGroup = cdrCostTechnicalSettingData.FilterGroup;

            if (!cdrCostTechnicalSettingData.DataRecordTypeId.HasValue)
                throw new NullReferenceException("cdrCostTechnicalSettingData.DataRecordTypeId");

            Guid dataRecordTypeId = cdrCostTechnicalSettingData.DataRecordTypeId.Value;

            List<CDRCostRequest> cdrCostRequests = new List<CDRCostRequest>();

            foreach (var cdr in cdrs)
            {
                RemoveCDRCostData(cdr, cdrCostFieldNames);
                DataRecordFilterGenericFieldMatchContext filterContext = new DataRecordFilterGenericFieldMatchContext(cdr, dataRecordTypeId);
                if (!recordFilterManager.IsFilterGroupMatch(recordFilterGroup, filterContext))
                    continue;

                CDRCostRequest cdrCostRequest = new CDRCostRequest()
                {
                    OriginalCDR = cdr,
                    AttemptDateTime = cdr.GetFieldValue(attemptDateTimeFieldName),
                    CGPN = cdr.GetFieldValue(cgpnFieldName),
                    CDPN = cdr.GetFieldValue(cdpnFieldName),
                    Duration = cdr.GetFieldValue(durationInSecondsFieldName)
                };

                cdrCostRequests.Add(cdrCostRequest);
            }

            FillCost(cdrCostRequests, cdrCostFieldNames);
        }

        public void UpadeOverridenCostCDRAfterDate(DateTime? fromTime)
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            cdrCostDataManager.UpadeOverridenCostCDRAfterDate(fromTime);
        }

        public HashSet<DateTime> GetDistinctDatesAfterId(long? cdrCostId)
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            return cdrCostDataManager.GetDistinctDatesAfterId(cdrCostId);
        }

        public long? GetMaxCDRCostId()
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            return cdrCostDataManager.GetMaxCDRCostId();
        }

        #endregion

        #region Private Methods

        private void FillCost(List<CDRCostRequest> cdrCostRequests, CDRCostFieldNames cdrCostFieldNames)
        {
            if (cdrCostRequests == null || cdrCostRequests.Count == 0)
                return;

            ConfigManager configManager = new ConfigManager();
            int maxBatchDurationInMinutes = configManager.GetMaxBatchDurationInMinutes();
            TimeSpan attemptDateTimeMargin = configManager.GetAttemptDateTimeMargin();
            TimeSpan attemptDateTimeOffset = configManager.GetAttemptDateTimeOffset();
            TimeSpan durationMargin = configManager.GetDurationMargin();
            decimal durationMarginInSeconds = Convert.ToDecimal(durationMargin.TotalSeconds);

            IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests = cdrCostRequests.Select(itm => BuildCDRCostRequest(itm, attemptDateTimeOffset)).OrderBy(itm => itm.AttemptDateTime);

            List<CDRCostBatchRequest> cdrCostBatchRequests = BuildCDRCostBatchRequests(orderedCDRCostRequests, attemptDateTimeMargin, maxBatchDurationInMinutes);

            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();


            foreach (var cdrCostBatchRequest in cdrCostBatchRequests)
            {
                List<CDRCost> cdrCostList = cdrCostDataManager.GetCDRCostByCDPNs(cdrCostBatchRequest);
                if (cdrCostList == null || cdrCostList.Count == 0)
                    continue;

                Dictionary<UniqueCDRCostKeys, List<CDRCost>> uniqueCDRCostKeysDict = BuildUniqueCDRCostKeysDict(cdrCostList);
                foreach (CDRCostRequest cdrCostRequest in cdrCostBatchRequest.CDRCostRequests)
                {
                    List<CDRCost> tempCDRCostList;
                    if (!uniqueCDRCostKeysDict.TryGetValue(new UniqueCDRCostKeys() { CGPN = cdrCostRequest.CGPN, CDPN = cdrCostRequest.CDPN }, out tempCDRCostList))
                        continue;

                    CDRCost matchingCDRCost = null; ;
                    foreach (var cdrCostItem in tempCDRCostList)
                    {
                        if (!IsCDRCostMatch(cdrCostItem, cdrCostRequest, durationMarginInSeconds, attemptDateTimeMargin))
                            continue;

                        if (matchingCDRCost == null || cdrCostItem.CDRCostId > matchingCDRCost.CDRCostId)
                            matchingCDRCost = cdrCostItem;
                    }

                    if (matchingCDRCost != null) //matching cost is found
                        FillCDRCostData(cdrCostRequest.OriginalCDR, matchingCDRCost, cdrCostFieldNames);
                }
            }
        }

        private Dictionary<UniqueCDRCostKeys, List<CDRCost>> BuildUniqueCDRCostKeysDict(List<CDRCost> cdrCostList)
        {
            Dictionary<UniqueCDRCostKeys, List<CDRCost>> uniqueCDRCostKeysDict = new Dictionary<UniqueCDRCostKeys, List<CDRCost>>();

            foreach (var cdrCost in cdrCostList)
            {
                UniqueCDRCostKeys uniqueCDRCostKey = new UniqueCDRCostKeys() { CDPN = cdrCost.CDPN, CGPN = cdrCost.CGPN };
                List<CDRCost> cdrCosts = uniqueCDRCostKeysDict.GetOrCreateItem(uniqueCDRCostKey);
                cdrCosts.Add(cdrCost);
            }

            return uniqueCDRCostKeysDict;
        }

        private void FillCDRCostData(dynamic cdr, CDRCost cdrCost, CDRCostFieldNames cdrCostFieldNames)
        {
            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostAmount))
                cdr.SetFieldValue(cdrCostFieldNames.CostAmount, cdrCost.Amount);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostRate))
                cdr.SetFieldValue(cdrCostFieldNames.CostRate, cdrCost.Rate);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.SupplierName))
                cdr.SetFieldValue(cdrCostFieldNames.SupplierName, cdrCost.SupplierName);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCurrency))
                cdr.SetFieldValue(cdrCostFieldNames.CostCurrency, cdrCost.CurrencyId);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CDRCostId))
                cdr.SetFieldValue(cdrCostFieldNames.CDRCostId, cdrCost.CDRCostId);
        }

        private void RemoveCDRCostData(dynamic cdr, CDRCostFieldNames cdrCostFieldNames)
        {
            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostAmount))
                cdr.SetFieldValue(cdrCostFieldNames.CostAmount, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostRate))
                cdr.SetFieldValue(cdrCostFieldNames.CostRate, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.SupplierName))
                cdr.SetFieldValue(cdrCostFieldNames.SupplierName, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCurrency))
                cdr.SetFieldValue(cdrCostFieldNames.CostCurrency, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CDRCostId))
                cdr.SetFieldValue(cdrCostFieldNames.CDRCostId, null);
        }

        private CDRCostRequest BuildCDRCostRequest(CDRCostRequest itm, TimeSpan attemptDateTimeOffset)
        {
            return new CDRCostRequest()
            {
                OriginalCDR = itm.OriginalCDR,
                AttemptDateTime = itm.AttemptDateTime.Add(attemptDateTimeOffset),
                CGPN = itm.CGPN,
                CDPN = itm.CDPN,
                Duration = itm.Duration
            };
        }

        private List<CDRCostBatchRequest> BuildCDRCostBatchRequests(IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests, TimeSpan attemptDateTimeMargin, int maxBatchDurationInMinutes)
        {
            CDRCostBatchRequest batchRequest = null;
            DateTime nextAttemptDateTimeThreshold = default(DateTime);
            List<CDRCostBatchRequest> cdrCostBatchRequests = new List<CDRCostBatchRequest>();

            foreach (var cdrCostRequest in orderedCDRCostRequests)
            {
                if (cdrCostRequest.AttemptDateTime < nextAttemptDateTimeThreshold)
                {
                    batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
                    batchRequest.CDRCostRequests.Add(cdrCostRequest);
                    batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.Add(attemptDateTimeMargin);
                }
                else
                {
                    batchRequest = new CDRCostBatchRequest()
                    {
                        CDPNs = new List<string>() { cdrCostRequest.CDPN },
                        BatchStart = cdrCostRequest.AttemptDateTime.Subtract(attemptDateTimeMargin),
                        BatchEnd = cdrCostRequest.AttemptDateTime.Add(attemptDateTimeMargin),
                        CDRCostRequests = new List<CDRCostRequest>() { cdrCostRequest }
                    };
                    nextAttemptDateTimeThreshold = cdrCostRequest.AttemptDateTime.AddMinutes(maxBatchDurationInMinutes);
                    cdrCostBatchRequests.Add(batchRequest);
                }
            }

            return cdrCostBatchRequests;
        }

        private bool IsCDRCostMatch(CDRCost cdrCost, CDRCostRequest cdrCostRequest, decimal durationMarginInSeconds, TimeSpan attemptDateTimeMargin)
        {
            if (!cdrCost.AttemptDateTime.HasValue || !cdrCost.DurationInSeconds.HasValue)
                return false;

            if (Math.Abs((cdrCost.AttemptDateTime.Value - cdrCostRequest.AttemptDateTime).TotalSeconds) > attemptDateTimeMargin.TotalSeconds
                || Math.Abs((cdrCost.DurationInSeconds.Value - cdrCostRequest.Duration)) > durationMarginInSeconds)
                return false;

            return true;
        }

        #endregion

        #region Private Classes

        private struct UniqueCDRCostKeys
        {
            public string CGPN { get; set; }

            public string CDPN { get; set; }
        }

        #endregion
    }

    public class CDRCostFieldNames
    {
        public string CDRCostId { get; set; }
        public string SupplierName { get; set; }
        public string CostRate { get; set; }
        public string CostAmount { get; set; }
        public string CostCurrency { get; set; }
    }
}