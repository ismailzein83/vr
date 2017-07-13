using Retail.Cost.Data;
using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Retail.Cost.Business
{
    public class CDRCostManager
    {
        int _maxBatchDurationInMinutes = 10;
        TimeSpan _durationMargin = new TimeSpan(0, 0, 5);
        TimeSpan _attemptDateTimeMargin = new TimeSpan(0, 0, 5);
        TimeSpan _attemptDateTimeOffset = new TimeSpan(0, 0, 0);

        /// <summary>
        /// used in data transformation
        /// </summary>
        public void AssignCostToCDR(List<dynamic> cdrs, string attemptDateTimeFieldName, string cgpnFieldName, string cdpnFieldName, string durationInSecondsFieldName, CDRCostFieldNames cdrCostFieldNames)
        {
            if (cdrCostFieldNames == null)
                return;

            List<CDRCostRequest> cdrCostRequests = new List<CDRCostRequest>();

            foreach (var cdr in cdrs)
            {
                CDRCostRequest cdrCostRequest = new CDRCostRequest()
                {
                    OriginalCDR = cdr,
                    AttemptDateTime = cdr.GetFieldValue(attemptDateTimeFieldName),  // Vanrise.Common.Utilities.GetPropValueReader(AttemptDateTimeFieldName).GetPropertyValue(cdr),
                    CGPN = cdr.GetFieldValue(cgpnFieldName),    //Vanrise.Common.Utilities.GetPropValueReader(CGPNFieldName).GetPropertyValue(cdr),
                    CDPN = cdr.GetFieldValue(cdpnFieldName),    //Vanrise.Common.Utilities.GetPropValueReader(CDPNFieldName).GetPropertyValue(cdr),
                    Duration = cdr.GetFieldValue(durationInSecondsFieldName)    //Vanrise.Common.Utilities.GetPropValueReader(durationInSecondsFieldName).GetPropertyValue(cdr)
                };

                cdrCostRequests.Add(cdrCostRequest);
            }

            FillCost(cdrCostRequests, cdrCostFieldNames);
        }

        private void FillCost(List<CDRCostRequest> cdrCostRequests, CDRCostFieldNames cdrCostFieldNames)
        {
            if (cdrCostRequests == null || cdrCostRequests.Count == 0)
                return;

            IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests = cdrCostRequests.Select(itm => BuildCDRCostRequest(itm, _attemptDateTimeOffset)).OrderBy(itm => itm.AttemptDateTime);

            List<CDRCostBatchRequest> cdrCostBatchRequests = BuildCDRCostBatchRequests(orderedCDRCostRequests);

            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();

            decimal durationMarginInSeconds = Convert.ToDecimal(_durationMargin.TotalSeconds);

            foreach (var cdrCostBatchRequest in cdrCostBatchRequests)
            {
                List<CDRCost> cdrCostList = cdrCostDataManager.GetCDRCostByCDPNs(cdrCostBatchRequest);
                if (cdrCostList == null || cdrCostList.Count == 0)
                    continue;

                Dictionary<UniqueCDRCostKeys, List<CDRCost>> uniqueCDRCostKeysDict = null;
                foreach (CDRCostRequest cdrCostRequest in cdrCostBatchRequest.CDRCostRequests)
                {
                    List<CDRCost> tempCDRCostList;
                    if (!uniqueCDRCostKeysDict.TryGetValue(new UniqueCDRCostKeys() { CGPN = cdrCostRequest.CGPN, CDPN = cdrCostRequest.CDPN }, out tempCDRCostList))
                        continue;

                    CDRCost matchingCDRCost = null; ;
                    foreach (var cdrCostItem in tempCDRCostList)
                    {
                        if (!IsCDRCostMatch(cdrCostItem, cdrCostRequest, durationMarginInSeconds, _attemptDateTimeMargin))
                            continue;

                        if (matchingCDRCost == null || cdrCostItem.CDRCostId > matchingCDRCost.CDRCostId)
                            matchingCDRCost = cdrCostItem;
                    }

                    if (matchingCDRCost != null) //matching cost is found
                        FillCDRCostData(cdrCostRequest.OriginalCDR, matchingCDRCost, cdrCostFieldNames);
                }
            }
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

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCDRId))
                cdr.SetFieldValue(cdrCostFieldNames.CostCDRId, cdrCost.CDRCostId);
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

        private List<CDRCostBatchRequest> BuildCDRCostBatchRequests(IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests)
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
                    batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.Add(_attemptDateTimeMargin);
                }
                else
                {
                    batchRequest = new CDRCostBatchRequest()
                    {
                        CDPNs = new List<string>() { cdrCostRequest.CDPN },
                        BatchStart = cdrCostRequest.AttemptDateTime.Subtract(_attemptDateTimeMargin),
                        BatchEnd = cdrCostRequest.AttemptDateTime.Add(_attemptDateTimeMargin),
                        CDRCostRequests = new List<CDRCostRequest>() { cdrCostRequest }
                    };
                    nextAttemptDateTimeThreshold = cdrCostRequest.AttemptDateTime.AddMinutes(_maxBatchDurationInMinutes);
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

        private struct UniqueCDRCostKeys
        {
            public string CGPN { get; set; }

            public string CDPN { get; set; }
        }
    }

    public class CDRCostFieldNames
    {
        public string CostAmount { get; set; }
        public string CostRate { get; set; }
        public string SupplierName { get; set; }
        public string CostCurrency { get; set; }
        public string CostCDRId { get; set; }
    }
}