using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ISaleZoneRoutingProductPreviewDataManager : IDataManager, IBulkApplyDataManager<ZoneRoutingProductPreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewZonesRoutingProductsToDB(object preparedRates);

        IEnumerable<ZoneRoutingProductPreview> GetFilteredZonesRoutingProductsPreview(SPLPreviewQuery query);
    }
}
