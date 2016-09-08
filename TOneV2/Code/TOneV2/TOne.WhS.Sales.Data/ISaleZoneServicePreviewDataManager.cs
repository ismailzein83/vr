using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;

namespace TOne.WhS.Sales.Data
{
    public interface ISaleZoneServicePreviewDataManager : IDataManager, IBulkApplyDataManager<SaleZoneServicePreview>
    {
        long ProcessInstanceId { set; }

        IEnumerable<SaleZoneServicePreview> GetSaleZoneServicePreviews(RatePlanPreviewQuery query);

        void ApplySaleZoneServicePreviewsToDB(IEnumerable<SaleZoneServicePreview> saleZoneServicePreviews);
    }
}
