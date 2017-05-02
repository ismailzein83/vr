using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Business
{
    public class CDRCostManager
    {
        //double _maxBatchDurationInMinutes = 10;
        //double _durationMarginInSeconds = 1;
        //TimeSpan _attemptDateTimeOffset = new TimeSpan(0, 5, 0);
        //TimeSpan _attemptDateTimeMargin = new TimeSpan(0, 0, 1);

        //public List<CDRCostResponse> CalculateCost(List<CDRCostRequest> cdrCostRequests)
        //{
        //    IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests =  cdrCostRequests.OrderBy(itm => itm.AttemptDateTime);
        //    if (orderedCDRCostRequests != null && orderedCDRCostRequests.Count() > 0)
        //    {
        //        List<CDRCostBatchRequest> cdrCostBatchRequests = new List<CDRCostBatchRequest>();

        //        CDRCostRequest firstCDRCostRequests = orderedCDRCostRequests.ElementAt(0);
        //        CDRCostBatchRequest batchRequest = new CDRCostBatchRequest();
        //        batchRequest.BatchStart = firstCDRCostRequests.AttemptDateTime;
        //        batchRequest.BatchEnd = firstCDRCostRequests.AttemptDateTime.AddMinutes(_maxBatchDurationInMinutes);
        //        cdrCostBatchRequests.Add(batchRequest);

        //        foreach (var cdrCostRequest in orderedCDRCostRequests)
        //        {
        //            if (cdrCostRequest.AttemptDateTime <= batchRequest.BatchEnd)
        //            {
        //                batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
        //            }
        //            else
        //            {
        //                batchRequest = new CDRCostBatchRequest();
        //                batchRequest.BatchStart = cdrCostRequest.AttemptDateTime;
        //                batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.AddMinutes(_maxBatchDurationInMinutes);
        //                batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
        //                cdrCostBatchRequests.Add(batchRequest);
        //            }
        //        }
        //    }

        //    return null;
        //}
    }
}
