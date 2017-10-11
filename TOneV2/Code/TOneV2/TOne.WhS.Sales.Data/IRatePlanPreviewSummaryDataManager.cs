using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanPreviewSummaryDataManager : IDataManager
    {
        long ProcessInstanceId { set; }

        RatePlanPreviewSummary GetCustomerRatePlanPreviewSummary(RatePlanPreviewQuery query);
        RatePlanPreviewSummary GetProductRatePlanPreviewSummary(RatePlanPreviewQuery query);
        
        void ApplyRatePlanPreviewSummaryToDB(RatePlanPreviewSummary summary);
    }
}
