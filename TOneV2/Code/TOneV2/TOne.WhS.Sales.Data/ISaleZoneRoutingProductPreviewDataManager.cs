using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface ISaleZoneRoutingProductPreviewDataManager : IDataManager, IBulkApplyDataManager<SaleZoneRoutingProductPreview>
    {
        long ProcessInstanceId { set; }

        IEnumerable<SaleZoneRoutingProductPreview> GetSaleZoneRoutingProductPreviews(RatePlanPreviewQuery query);

        void ApplySaleZoneRoutingProductPreviewsToDB(IEnumerable<SaleZoneRoutingProductPreview> saleZoneRoutingProductPreviews);
    }
}
