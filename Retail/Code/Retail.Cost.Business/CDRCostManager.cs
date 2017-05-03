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
        double _maxBatchDurationInMinutes = 10;
        double _durationMarginInSeconds = 5;
        double _attemptDateTimeOffsetInMinutes = 0;
        double _attemptDateTimeMarginInSeconds = 5;

        public static List<dynamic> AssignCostToCDR(List<dynamic> cdrs, string AttemptDateTimeFieldName, string CGPNFieldName, string CDPNFieldName, string durationInSecondsFieldName)
        {
            List<CDRCostRequest> cdrCostRequests = new List<CDRCostRequest>();

            foreach (var cdr in cdrs)
            {
                CDRCostRequest cdrCostRequest = new CDRCostRequest()
                {
                    OriginalCDR = cdr,
                    AttemptDateTime = Vanrise.Common.Utilities.GetPropValueReader(AttemptDateTimeFieldName).GetPropertyValue(cdr),
                    CGPN = Vanrise.Common.Utilities.GetPropValueReader(CGPNFieldName).GetPropertyValue(cdr),
                    CDPN = Vanrise.Common.Utilities.GetPropValueReader(CDPNFieldName).GetPropertyValue(cdr),
                    Duration = Vanrise.Common.Utilities.GetPropValueReader(durationInSecondsFieldName).GetPropertyValue(cdr)
                };

                cdrCostRequests.Add(cdrCostRequest);
            }

            List<CDRCostResponse> cdrCostResponses = new CDRCostManager().CalculateCost(cdrCostRequests);
            if (cdrCostResponses == null)
                return null;

            foreach (var cdrCostResponse in cdrCostResponses)
            {
                dynamic currentRawCDR = cdrCostResponse.Request.OriginalCDR;
                currentRawCDR.CostAmount = (decimal?)cdrCostResponse.CostAmount;
                currentRawCDR.CostRate = (decimal)cdrCostResponse.CostRate;
            }

            return cdrs;
        }

        public List<CDRCostResponse> CalculateCost(List<CDRCostRequest> cdrCostRequests)
        {
            if (cdrCostRequests == null || cdrCostRequests.Count == 0)
                return null;

            IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests = cdrCostRequests.Select(itm => new CDRCostRequest()
                                                                                        {
                                                                                            OriginalCDR = itm.OriginalCDR,
                                                                                            AttemptDateTime = itm.AttemptDateTime.AddMinutes(_attemptDateTimeOffsetInMinutes),
                                                                                            CGPN = itm.CGPN,
                                                                                            CDPN = itm.CDPN,
                                                                                            Duration = itm.Duration,
                                                                                        }).OrderBy(itm => itm.AttemptDateTime);

            List<CDRCostBatchRequest> cdrCostBatchRequests = BuildCDRCostBatchRequests(orderedCDRCostRequests);

            //Transforming cdrCostRequests to cdrCostRequestsByExactMatchKeys
            Dictionary<CDRCostExactMatchKeys, List<CDRCostRequest>> cdrCostRequestsByExactMatchKeys = new Dictionary<CDRCostExactMatchKeys, List<CDRCostRequest>>();
            foreach (var itm in orderedCDRCostRequests)
            {
                List<CDRCostRequest> currentCDRCostRequest = cdrCostRequestsByExactMatchKeys.GetOrCreateItem(new CDRCostExactMatchKeys() { CGPN = itm.CGPN, CDPN = itm.CDPN });
                currentCDRCostRequest.Add(itm);
            }

            List<CDRCostResponse> results = new List<CDRCostResponse>();
            IProcessedCDRCostDataManager processedCDRCostDataManager = CostDataManagerFactory.GetDataManager<IProcessedCDRCostDataManager>();

            foreach (var cdrCostBatchRequest in cdrCostBatchRequests)
            {
                List<ProcessedCDRCost> processedCDRsCost = processedCDRCostDataManager.GetProcessedCDRCostByCDPNs(cdrCostBatchRequest);
                foreach (var processedCDRCost in processedCDRsCost)
                {
                    List<CDRCostRequest> tempCDRCostRequest;
                    if (cdrCostRequestsByExactMatchKeys.TryGetValue(new CDRCostExactMatchKeys() { CGPN = processedCDRCost.CGPN, CDPN = processedCDRCost.CDPN }, out tempCDRCostRequest))
                    {
                        foreach (var cdrCostRequest in tempCDRCostRequest)
                        {
                            if (IsProcessedCDRCostMatch(processedCDRCost, cdrCostRequest))
                            {
                                results.Add(new CDRCostResponse() { Request = cdrCostRequest, CostAmount = processedCDRCost.Amount, CostRate = processedCDRCost.Rate });
                                break;
                            }
                        }
                    }
                }
            }

            return results;
        }

        private List<CDRCostBatchRequest> BuildCDRCostBatchRequests(IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests)
        {
            CDRCostRequest firstCDRCostRequests = orderedCDRCostRequests.ElementAt(0);
            CDRCostBatchRequest batchRequest = new CDRCostBatchRequest();
            batchRequest.CDPNs = new List<string>();
            batchRequest.BatchStart = firstCDRCostRequests.AttemptDateTime.AddSeconds(-_attemptDateTimeMarginInSeconds);
            //batchRequest.BatchEnd = firstCDRCostRequests.AttemptDateTime.AddSeconds(_attemptDateTimeMarginInSeconds);
            DateTime nextAttemptDateTimeThreshold = firstCDRCostRequests.AttemptDateTime.AddMinutes(_maxBatchDurationInMinutes);

            List<CDRCostBatchRequest> cdrCostBatchRequests = new List<CDRCostBatchRequest>();
            cdrCostBatchRequests.Add(batchRequest);

            foreach (var cdrCostRequest in orderedCDRCostRequests)
            {
                if (cdrCostRequest.AttemptDateTime < nextAttemptDateTimeThreshold)
                {
                    batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
                    batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.AddSeconds(_attemptDateTimeMarginInSeconds);
                }
                else
                {
                    batchRequest = new CDRCostBatchRequest();
                    batchRequest.CDPNs = new List<string>();
                    batchRequest.BatchStart = cdrCostRequest.AttemptDateTime.AddSeconds(-_attemptDateTimeMarginInSeconds);
                    batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.AddSeconds(_attemptDateTimeMarginInSeconds);
                    nextAttemptDateTimeThreshold = cdrCostRequest.AttemptDateTime.AddMinutes(_maxBatchDurationInMinutes);
                    batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
                    cdrCostBatchRequests.Add(batchRequest);
                }
            }

            return cdrCostBatchRequests;
        }

        private bool IsProcessedCDRCostMatch(ProcessedCDRCost processedCDRCost, CDRCostRequest cdrCostRequest)
        {
            if (Math.Abs((processedCDRCost.AttemptDateTime - cdrCostRequest.AttemptDateTime).TotalSeconds) <= _attemptDateTimeMarginInSeconds &&
                Math.Abs((double)(processedCDRCost.DurationInSeconds - cdrCostRequest.Duration)) <= _durationMarginInSeconds)
                return true;
            return false;
        }

        private struct CDRCostExactMatchKeys
        {
            public string CGPN { get; set; }

            public string CDPN { get; set; }
        }
    }
}
