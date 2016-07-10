using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanPreviewSummaryManager
    {
        public RatePlanPreviewSummary GetRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            var dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanPreviewSummaryDataManager>();
            return dataManager.GetRatePlanPreviewSummary(query);
        }
    }
}
