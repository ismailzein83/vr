using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierZoneServicePreviewDataManager : IDataManager, IBulkApplyDataManager<ZoneServicePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewZonesServicesToDB(object preparedZonesServices); 
        
        IEnumerable<ZoneServicePreview> GetFilteredZonesServicesPreview(SPLPreviewQuery query);
    }
}
